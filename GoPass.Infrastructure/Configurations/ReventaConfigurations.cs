using GoPass.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoPass.Infrastructure.Configurations;

public class ReventaConfigurations : EntityTypeBaseConfiguration<Reventa>
{
    protected override void ConfigurateConstraints(EntityTypeBuilder<Reventa> builder)
    {
        builder.HasKey(r => r.Id);
        //builder.HasOne(e => e.Entrada).WithMany(r => r.Reventa).HasForeignKey(e => e.EntradaId);
        builder.HasOne(e => e.Entrada).WithOne(r => r.Reventa).HasForeignKey<Reventa>(e => e.EntradaId);
        builder.HasOne(e => e.Usuario).WithMany(r => r.Reventa).HasForeignKey(e => e.CompradorId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Usuario).WithMany(r => r.Reventa).HasForeignKey(e => e.VendedorId).OnDelete(DeleteBehavior.Restrict);
    }

    protected override void ConfigurateProperties(EntityTypeBuilder<Reventa> builder)
    {
        builder.Property(r => r.EntradaId).IsRequired();
        builder.Property(r => r.VendedorId).IsRequired();
        builder.Property(r => r.CompradorId).IsRequired();
        builder.Property(r => r.FechaReventa).IsRequired();
        builder.Property(r => r.Precio).IsRequired().HasPrecision(18,2);
        builder.Property(r => r.ResaleDetail).IsRequired().HasMaxLength(100);
    }

    protected override void ConfigurateTableName(EntityTypeBuilder<Reventa> builder)
    {
        builder.ToTable("Reventas");
    }
}
