namespace GoodJobsEmployed.API.ML;

public class Perfil
{
    public int IdPerfil { get; set; }
    public string? Nombre { get; set; }

    // Navigation Property
    public List<PerfilCaracteristica>? PerfilCaracteristicas { get; set; } = new List<PerfilCaracteristica>();
}
