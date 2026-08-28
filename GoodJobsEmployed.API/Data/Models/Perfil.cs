using System;
using System.Collections.Generic;

namespace GoodJobsEmployed.API.Data.Models;

public partial class Perfil
{
    public int IdPerfil { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<PerfilCaracteristica> PerfilCaracteristicas { get; set; } = new List<PerfilCaracteristica>();
}
