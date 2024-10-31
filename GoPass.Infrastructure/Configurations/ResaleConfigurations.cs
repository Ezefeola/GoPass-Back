using GoPass.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoPass.Infrastructure.Configurations;

public class ResaleConfigurations : EntityTypeBaseConfiguration<Resale>
{
    protected override void ConfigurateConstraints(EntityTypeBuilder<Resale> builder)
    {
        builder.HasKey(r => r.Id);
        //builder.HasOne(e => e.Entrada).WithMany(r => r.Reventa).HasForeignKey(e => e.EntradaId);
        builder.HasOne(e => e.Ticket).WithOne(r => r.Resale).HasForeignKey<Resale>(e => e.TicketId);
        builder.HasOne(e => e.User).WithMany(r => r.Resales).HasForeignKey(e => e.BuyerId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.User).WithMany(r => r.Resales).HasForeignKey(e => e.SellerId).OnDelete(DeleteBehavior.Restrict);
    }

    protected override void ConfigurateProperties(EntityTypeBuilder<Resale> builder)
    {
        builder.Property(r => r.TicketId).IsRequired();
        builder.Property(r => r.SellerId).IsRequired();
        builder.Property(r => r.BuyerId).IsRequired();
        builder.Property(r => r.ResaleStartDate).IsRequired();
        builder.Property(r => r.Price).IsRequired().HasPrecision(18,2);
        builder.Property(r => r.ResaleDetail).IsRequired().HasMaxLength(100);
    }

    protected override void ConfigurateTableName(EntityTypeBuilder<Resale> builder)
    {
        builder.ToTable("Resales");
    }
}
