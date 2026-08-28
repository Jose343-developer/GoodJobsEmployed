using System;
using System.Collections.Generic;
using GoodJobsEmployed.API.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GoodJobsEmployed.API.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Caracteristica> Caracteristicas { get; set; }

    public virtual DbSet<Perfil> Perfils { get; set; }

    public virtual DbSet<PerfilCaracteristica> PerfilCaracteristicas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost,1435;Database=TRACKING_WEB;User Id=sa;Password=P@ssw0rd2026!;Encrypt=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Caracteristica>(entity =>
        {
            entity.HasKey(e => e.IdCaracteristica).HasName("PK__Caracter__F27E521E98D75921");

            entity.ToTable("Caracteristica");

            entity.Property(e => e.IdCaracteristica).HasColumnName("idCaracteristica");
            entity.Property(e => e.DescripcionCaracteristica)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("descripcionCaracteristica");
            entity.Property(e => e.NombreCaracteristica)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombreCaracteristica");
        });

        modelBuilder.Entity<Perfil>(entity =>
        {
            entity.HasKey(e => e.IdPerfil).HasName("PK__Perfil__40F13B6021C08B63");

            entity.ToTable("Perfil");

            entity.Property(e => e.IdPerfil).HasColumnName("idPerfil");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<PerfilCaracteristica>(entity =>
        {
            entity.HasKey(e => new { e.IdPerfil, e.IdCaracteristica }).HasName("PK__Perfil_C__BFD6DE418764CB6A");

            entity.ToTable("Perfil_Caracteristica");

            entity.Property(e => e.IdPerfil).HasColumnName("idPerfil");
            entity.Property(e => e.IdCaracteristica).HasColumnName("idCaracteristica");
            entity.Property(e => e.CriterioEspecifico)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("criterioEspecifico");
            entity.Property(e => e.PesoPrioridad).HasColumnName("pesoPrioridad");

            entity.HasOne(d => d.IdCaracteristicaNavigation).WithMany(p => p.PerfilCaracteristicas)
                .HasForeignKey(d => d.IdCaracteristica)
                .HasConstraintName("FK__Perfil_Ca__idCar__3D5E1FD2");

            entity.HasOne(d => d.IdPerfilNavigation).WithMany(p => p.PerfilCaracteristicas)
                .HasForeignKey(d => d.IdPerfil)
                .HasConstraintName("FK__Perfil_Ca__idPer__3C69FB99");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
