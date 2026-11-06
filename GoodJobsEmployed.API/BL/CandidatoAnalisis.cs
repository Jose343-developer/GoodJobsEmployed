using System.Text;
using System.Text.Json;
using GoodJobsEmployed.API.Data;
using GoodJobsEmployed.API.ML;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using UglyToad.PdfPig;

namespace GoodJobsEmployed.API.BL;

public class CandidatoAnalisis
{
    private readonly AppDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;

    public CandidatoAnalisis(AppDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ML.Result> AnalizarCVPdfAsync(int idPerfil, IFormFile file)
    {
        var result = new ML.Result();

        try
        {
            if (idPerfil <= 0)
            {
                result.Correct = false;
                result.ErrorMessage = "El identificador del perfil debe ser mayor a 0.";
                return result;
            }

            if (file == null || file.Length == 0)
            {
                result.Correct = false;
                result.ErrorMessage = "Debe subir un archivo de CV en formato PDF.";
                return result;
            }

            if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                result.Correct = false;
                result.ErrorMessage = "Solo se permiten archivos en formato PDF.";
                return result;
            }

            if (file.Length > 1.5 * 1024 * 1024)
            {
                result.Correct = false;
                result.ErrorMessage = "El archivo excede el tamaño máximo permitido de 1.5 MB.";
                return result;
            }

            // 1. Obtener el Perfil y sus Características de la BD
            var perfil = await _context.Perfils
                .Include(p => p.PerfilCaracteristicas)
                    .ThenInclude(pc => pc.IdCaracteristicaNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdPerfil == idPerfil);

            if (perfil == null)
            {
                result.Correct = false;
                result.ErrorMessage = "El perfil solicitado no existe en la base de datos.";
                return result;
            }

            if (!perfil.PerfilCaracteristicas.Any())
            {
                result.Correct = false;
                result.ErrorMessage = "El perfil seleccionado no tiene características asociadas para evaluar.";
                return result;
            }

            // 2. Extraer texto del PDF con PdfPig
            string textoCV = ExtraerTextoDePdf(file);
            if (string.IsNullOrWhiteSpace(textoCV))
            {
                result.Correct = false;
                result.ErrorMessage = "No se pudo extraer texto legible del PDF suministrado (puede tratarse de una imagen escaneada).";
                return result;
            }

            // 3. Obtener API Key de Gemini desde .env o variable de entorno
            string apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? string.Empty;
            string modelo = Environment.GetEnvironmentVariable("GEMINI_MODEL") ?? "gemini-2.5-flash";

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                result.Correct = false;
                result.ErrorMessage = "No se encontró configurada la clave GEMINI_API_KEY en el archivo .env o en el entorno del sistema.";
                return result;
            }

            // 4. Invocar a Gemini API y evaluar compatibilidad
            var analisisResultado = await EvaluarConGeminiAsync(apiKey, modelo, perfil, textoCV);

            result.Correct = true;
            result.Object = analisisResultado;
            return result;
        }
        catch (TaskCanceledException)
        {
            result.Correct = false;
            result.ErrorMessage = "El tiempo de espera para el análisis con la IA se agotó (timeout de 30s). Por favor, intenta de nuevo.";
            return result;
        }
        catch (HttpRequestException ex)
        {
            result.Correct = false;
            result.ErrorMessage = $"Error de conectividad al comunicarse con el servicio de IA: {ex.Message}";
            result.Ex = ex;
            return result;
        }
        catch (Exception ex)
        {
            result.Correct = false;
            result.Ex = ex;
            result.ErrorMessage = ex.Message;
            return result;
        }
    }

    private string ExtraerTextoDePdf(IFormFile file)
    {
        var sb = new StringBuilder();
        using (var stream = file.OpenReadStream())
        using (var document = PdfDocument.Open(stream))
        {
            foreach (var page in document.GetPages())
            {
                sb.AppendLine(page.Text);
            }
        }
        return sb.ToString().Trim();
    }

    private async Task<AnalisisCVResultado> EvaluarConGeminiAsync(string apiKey, string modelo, Data.Models.Perfil perfil, string textoCV)
    {
        var httpClient = _httpClientFactory.CreateClient("GeminiClient");
        string url = $"https://generativelanguage.googleapis.com/v1beta/models/{modelo}:generateContent?key={apiKey}";

        var listaCriterios = perfil.PerfilCaracteristicas.Select(pc => new
        {
            idCaracteristica = pc.IdCaracteristica,
            nombre = pc.IdCaracteristicaNavigation?.NombreCaracteristica ?? "",
            descripcion = pc.IdCaracteristicaNavigation?.DescripcionCaracteristica ?? "",
            peso = pc.PesoPrioridad,
            criterioEspecifico = pc.CriterioEspecifico ?? ""
        }).ToList();

        string criteriosJson = JsonSerializer.Serialize(listaCriterios, new JsonSerializerOptions { WriteIndented = true });

        var promptBuilder = new StringBuilder();
        promptBuilder.AppendLine("Eres un experto reclutador técnico y evaluador de talento.");
        promptBuilder.AppendLine($"Tu tarea es analizar el siguiente Curriculum Vitae (CV) y contrastarlo minuciosamente con el perfil del rol profesional: \"{perfil.Nombre}\".");
        promptBuilder.AppendLine();
        promptBuilder.AppendLine("A continuación están los criterios y características que el perfil exige con sus pesos de prioridad:");
        promptBuilder.AppendLine(criteriosJson);
        promptBuilder.AppendLine();
        promptBuilder.AppendLine("TEXTO DEL CURRICULUM VITAE:");
        promptBuilder.AppendLine(textoCV);
        promptBuilder.AppendLine();
        promptBuilder.AppendLine("INSTRUCCIONES DE RESPUESTA:");
        promptBuilder.AppendLine("- Evalúa cada característica basándote en la evidencia real del CV, asignando a cada una si cumple (true/false) y un puntaje de 0 a 100.");
        promptBuilder.AppendLine("- Redacta una ÚNICA justificación general abarcando el cumplimiento global de las características.");
        promptBuilder.AppendLine("- REGLA ESTRICTA DE LONGITUD: La justificación general DEBE tener como MÁXIMO 280 caracteres en total.");
        promptBuilder.AppendLine("- Responde ÚNICAMENTE con un JSON con la estructura: { \"justificacionGeneral\": \"...\", \"evaluaciones\": [ { \"idCaracteristica\": 1, \"cumple\": true, \"puntajeObtenido\": 85 } ] }");

        var payload = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = promptBuilder.ToString() } } }
            },
            generationConfig = new
            {
                responseMimeType = "application/json",
                temperature = 0.2,
                maxOutputTokens = 600
            }
        };

        var httpContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(url, httpContent);

        if (!response.IsSuccessStatusCode)
        {
            string errorBody = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error al consultar Gemini ({response.StatusCode}): {errorBody}");
        }

        string jsonResponse = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(jsonResponse);
        string textContent = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? "{}";

        var aiParsed = JsonSerializer.Deserialize<GeminiParsedResponse>(textContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Truncado de seguridad garantizado a max 280 caracteres
        string justificacion = (aiParsed?.JustificacionGeneral ?? "Evaluación completada.").Trim();
        if (justificacion.Length > 280)
        {
            justificacion = justificacion.Substring(0, 277) + "...";
        }

        var resultadoFinal = new AnalisisCVResultado
        {
            IdPerfil = perfil.IdPerfil,
            NombrePerfil = perfil.Nombre,
            JustificacionGeneral = justificacion
        };

        double sumaPonderada = 0;
        double sumaPesos = 0;

        foreach (var pc in perfil.PerfilCaracteristicas)
        {
            var evaluacionAi = aiParsed?.Evaluaciones?.FirstOrDefault(e => e.IdCaracteristica == pc.IdCaracteristica);

            double score = evaluacionAi?.PuntajeObtenido ?? (evaluacionAi != null && evaluacionAi.Cumple ? 100 : 0);
            byte peso = pc.PesoPrioridad ?? 1;

            sumaPonderada += score * peso;
            sumaPesos += peso;

            resultadoFinal.EvaluacionDetallada.Add(new EvaluacionCaracteristica
            {
                IdCaracteristica = pc.IdCaracteristica,
                NombreCaracteristica = pc.IdCaracteristicaNavigation?.NombreCaracteristica ?? "",
                DescripcionCaracteristica = pc.IdCaracteristicaNavigation?.DescripcionCaracteristica ?? "",
                PesoPrioridad = peso,
                CriterioEspecifico = pc.CriterioEspecifico ?? "",
                Cumple = evaluacionAi?.Cumple ?? (score >= 60),
                PuntajeObtenido = Math.Round(score, 2)
            });
        }

        resultadoFinal.PorcentajeCompatibilidad = sumaPesos > 0
            ? Math.Round(sumaPonderada / sumaPesos, 2)
            : 0;

        return resultadoFinal;
    }

    private class GeminiParsedResponse
    {
        public string? JustificacionGeneral { get; set; }
        public List<GeminiParsedItem>? Evaluaciones { get; set; }
    }

    private class GeminiParsedItem
    {
        public int IdCaracteristica { get; set; }
        public bool Cumple { get; set; }
        public double PuntajeObtenido { get; set; }
    }
}
