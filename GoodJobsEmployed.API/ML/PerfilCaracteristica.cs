namespace GoodJobsEmployed.API.ML;

public class PerfilCaracteristica
{
    public int IdPerfil { get; set; }
    public int IdCaracteristica { get; set; }
    public int? PesoPrioridad { get; set; }
    public string? CriterioEspecifico { get; set; }

    // Navigation Properties (anulables para evitar validación 400 requerida al enviar payload)
    public Perfil? Perfil { get; set; }
    public Caracteristica? Caracteristica { get; set; }
}
