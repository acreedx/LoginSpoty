using System;
using System.Collections.Generic;
using LoginAplicacion.Model;
using Microsoft.EntityFrameworkCore;

namespace LoginAplicacion.Data;

public partial class SpotifyBdContext : DbContext
{
    public SpotifyBdContext()
    {
    }

    public SpotifyBdContext(DbContextOptions<SpotifyBdContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AudiUsuario> AudiUsuarios { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("workstation id=SpotifyBD.mssql.somee.com;packet size=4096;user id=Acreedx_SQLLogin_1;pwd=5mw7ab51z2;data source=SpotifyBD.mssql.somee.com;persist security info=False;initial catalog=SpotifyBD;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AudiUsuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Audi_Usu__3214EC07AE1A47B4");

            entity.ToTable("Audi_Usuarios");

            entity.Property(e => e.AccionRealizada).IsUnicode(false);
            entity.Property(e => e.UsuariosIdUsuario).HasColumnName("Usuarios_Id_Usuario");

            entity.HasOne(d => d.UsuariosIdUsuarioNavigation).WithMany(p => p.AudiUsuarios)
                .HasForeignKey(d => d.UsuariosIdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Audi_Usuarios_Usuarios_FK");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC07F7D7707A");

            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FechaDeActualizacion)
                .HasColumnName("Fecha_De_Actualizacion");
            entity.Property(e => e.IntentosPassword).HasColumnName("Intentos_password");
            entity.Property(e => e.NombreCompleto)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Nombre_Completo");
            entity.Property(e => e.NombreUsuario)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Nombre_Usuario");
            entity.Property(e => e.Password)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
