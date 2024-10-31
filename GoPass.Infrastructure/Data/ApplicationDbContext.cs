using GoPass.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;


namespace GoPass.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<User> Users{ get; set; }
    public DbSet<Resale> Resales { get; set; }
    public DbSet<TicketResaleHistory> TicketsResalesHistory { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
