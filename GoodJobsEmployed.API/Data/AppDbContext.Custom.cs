using Microsoft.EntityFrameworkCore;
using GoodJobsEmployed.API.Data.DTOs;

namespace GoodJobsEmployed.API.Data;

public partial class AppDbContext
{
    
    public virtual DbSet<EjemploSPResult> EjemploSPResults { get; set; }
    public virtual DbSet<DTO.PerfilGetAll> PerfilGetAlls { get; set; }
   
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<EjemploSPResult>().HasNoKey();
        modelBuilder.Entity<DTO.PerfilGetAll>().HasNoKey();
    }
}
