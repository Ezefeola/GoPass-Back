using GoPass.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoPass.Infrastructure.Configurations;

public class EntradaConfigurations : EntityTypeBaseConfiguration<Entrada>
{
    protected override void ConfigurateConstraints(EntityTypeBuilder<Entrada> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasOne(e => e.Reventa).WithOne(e => e.Entrada).HasForeignKey<Reventa>(u => u.EntradaId);
        builder.HasOne(u => u.Usuario).WithMany(e => e.Entrada).HasForeignKey(u => u.UsuarioId);
    }

    protected override void ConfigurateProperties(EntityTypeBuilder<Entrada> builder)
    {
        builder.Property(e => e.GameName).IsRequired().HasMaxLength(80);
        builder.Property(e => e.Description).IsRequired().HasMaxLength(150);
        builder.Property(e => e.Address).IsRequired().HasMaxLength(80);
        builder.Property(e => e.EventDate).IsRequired();
        builder.Property(e => e.CodigoQR).IsRequired().HasMaxLength(7089); //caracteres segun QR numerico a consultar
        builder.Property(e => e.Verificada).IsRequired().HasColumnType("bit");

    }

    protected override void ConfigurateTableName(EntityTypeBuilder<Entrada> builder)
    {
        builder.ToTable("Entradas");
    }
}
