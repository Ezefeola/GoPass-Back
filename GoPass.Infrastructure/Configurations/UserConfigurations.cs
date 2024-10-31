using GoPass.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoPass.Infrastructure.Configurations;

public class UserConfigurations : EntityTypeBaseConfiguration<User>
{
    protected override void ConfigurateConstraints(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.HasMany(e => e.Tickets).WithOne(r => r.User).HasForeignKey(e => e.UserId);
        builder.HasMany(e => e.Resales).WithOne(r => r.User).HasForeignKey(e => e.BuyerId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(e => e.Resales).WithOne(r => r.User).HasForeignKey(e => e.SellerId).OnDelete(DeleteBehavior.Restrict);
    }

    protected override void ConfigurateProperties(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.Name).HasMaxLength(200).HasColumnType("varchar");
        builder.Property(u => u.PhoneNumber).HasMaxLength(26).HasColumnType("varchar");
        builder.Property(u => u.DNI).HasMaxLength(26).HasColumnType("varchar");
        builder.Property(u => u.IsVerified).HasColumnType("bit");
        builder.Property(u => u.Email).IsRequired().HasMaxLength(50);
        builder.Property(u => u.Password).IsRequired();
        builder.Property(u => u.Country).HasMaxLength(30);
        builder.Property(u => u.City).HasMaxLength(30);
    }

    protected override void ConfigurateTableName(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
    }
}
