using GoPass.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoPass.Infrastructure.Configurations;

public class UsuarioConfigurations : EntityTypeBaseConfiguration<Usuario>
{
    protected override void ConfigurateConstraints(EntityTypeBuilder<Usuario> builder)
    {
        builder.HasKey(u => u.Id);
        builder.HasMany(e => e.Entrada).WithOne(r => r.Usuario).HasForeignKey(e => e.UsuarioId);
        builder.HasMany(e => e.Reventa).WithOne(r => r.Usuario).HasForeignKey(e => e.CompradorId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(e => e.Reventa).WithOne(r => r.Usuario).HasForeignKey(e => e.VendedorId).OnDelete(DeleteBehavior.Restrict);
    }

    protected override void ConfigurateProperties(EntityTypeBuilder<Usuario> builder)
    {
        builder.Property(u => u.Nombre).HasMaxLength(200).HasColumnType("varchar");
        builder.Property(u => u.NumeroTelefono).HasMaxLength(26).HasColumnType("varchar");
        builder.Property(u => u.DNI).HasMaxLength(26).HasColumnType("varchar");
        builder.Property(u => u.Verificado).HasColumnType("bit");
        builder.Property(u => u.Email).IsRequired().HasMaxLength(50);
        builder.Property(u => u.Password).IsRequired();
        builder.Property(u => u.Country).HasMaxLength(30);
        builder.Property(u => u.City).HasMaxLength(30);
    }

    protected override void ConfigurateTableName(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");
    }
}
