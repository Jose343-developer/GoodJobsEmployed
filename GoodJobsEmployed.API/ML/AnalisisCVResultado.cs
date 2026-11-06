namespace GoodJobsEmployed.API.ML;

public class EvaluacionCaracteristica
{
    public int IdCaracteristica { get; set; }
    public string NombreCaracteristica { get; set; } = string.Empty;
    public string DescripcionCaracteristica { get; set; } = string.Empty;
    public byte PesoPrioridad { get; set; }
    public string CriterioEspecifico { get; set; } = string.Empty;

    // Campos evaluados por la IA:
    public bool Cumple { get; set; }
    public double PuntajeObtenido { get; set; }
}

public class AnalisisCVResultado
{
    public int IdPerfil { get; set; }
    public string NombrePerfil { get; set; } = string.Empty;
    public double PorcentajeCompatibilidad { get; set; }
    public string JustificacionGeneral { get; set; } = string.Empty;
    public List<EvaluacionCaracteristica> EvaluacionDetallada { get; set; } = new List<EvaluacionCaracteristica>();
}
