using System;
using System.Collections.Generic;

namespace GoodJobsEmployed.API.Data.Models;

public partial class PerfilCaracteristica
{
    public int IdPerfil { get; set; }

    public int IdCaracteristica { get; set; }

    public byte? PesoPrioridad { get; set; }

    public string? CriterioEspecifico { get; set; }

    public virtual Caracteristica IdCaracteristicaNavigation { get; set; } = null!;

    public virtual Perfil IdPerfilNavigation { get; set; } = null!;
}
