using GoPass.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoPass.Infrastructure.Configurations;

public class TicketConfigurations : EntityTypeBaseConfiguration<Ticket>
{
    protected override void ConfigurateConstraints(EntityTypeBuilder<Ticket> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasOne(e => e.Resale).WithOne(e => e.Ticket).HasForeignKey<Resale>(u => u.TicketId);
        builder.HasOne(u => u.User).WithMany(e => e.Tickets).HasForeignKey(u => u.UserId);
    }

    protected override void ConfigurateProperties(EntityTypeBuilder<Ticket> builder)
    {
        builder.Property(e => e.GameName).IsRequired().HasMaxLength(80);
        builder.Property(e => e.Description).IsRequired().HasMaxLength(150);
        builder.Property(e => e.Address).IsRequired().HasMaxLength(80);
        builder.Property(e => e.EventDate).IsRequired();
        builder.Property(e => e.QrCode).IsRequired().HasMaxLength(7089); //caracteres segun QR numerico a consultar
        builder.Property(e => e.IsTicketVerified).IsRequired().HasColumnType("bit");

    }

    protected override void ConfigurateTableName(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets");
    }
}
