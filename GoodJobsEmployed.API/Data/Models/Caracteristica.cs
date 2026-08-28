using System;
using System.Collections.Generic;

namespace GoodJobsEmployed.API.Data.Models;

public partial class Caracteristica
{
    public int IdCaracteristica { get; set; }

    public string NombreCaracteristica { get; set; } = null!;

    public string? DescripcionCaracteristica { get; set; }

    public virtual ICollection<PerfilCaracteristica> PerfilCaracteristicas { get; set; } = new List<PerfilCaracteristica>();
}
