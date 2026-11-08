namespace GoodJobsEmployed.API.ML;

public class Caracteristica
{
    public int IdCaracteristica { get; set; }
    public string NombreCaracteristica { get; set; } = string.Empty;
    public string? DescripcionCaracteristica { get; set; }

    // Navigation Property
    public List<PerfilCaracteristica>? PerfilCaracteristicas { get; set; } = new List<PerfilCaracteristica>();
}
