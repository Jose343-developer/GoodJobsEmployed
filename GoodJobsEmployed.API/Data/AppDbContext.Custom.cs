using Microsoft.EntityFrameworkCore;
using GoodJobsEmployed.API.Data.DTOs;

namespace GoodJobsEmployed.API.Data;

public partial class AppDbContext
{
    // Registra aquí los DbSet para tus Stored Procedures / DTOs
    public virtual DbSet<EjemploSPResult> EjemploSPResults { get; set; }

    // Este método es llamado automáticamente al final de OnModelCreating por Entity Framework
    // y NO será sobrescrito si vuelves a ejecutar el scaffolding.
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        // Configura los DTOs que no tienen llave primaria (resultados de SPs)
        modelBuilder.Entity<EjemploSPResult>().HasNoKey();
    }
}
