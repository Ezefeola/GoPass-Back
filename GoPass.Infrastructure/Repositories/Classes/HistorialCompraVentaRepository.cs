using GoPass.Domain.Models;
using GoPass.Infrastructure.Data;
using GoPass.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GoPass.Infrastructure.Repositories.Classes;

public class HistorialCompraVentaRepository : GenericRepository<HistorialCompraVenta>, IHistorialCompraVentaRepository
{
    public HistorialCompraVentaRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
            
    }

    public async Task<List<HistorialCompraVenta>> GetBoughtTicketsByCompradorId(int compradorId)
    {
        if (compradorId <= 0)
        {
            throw new ArgumentException("El ID del vendedor no es válido.");
        }

        List<HistorialCompraVenta> ticketsInResale = await _dbSet.Where(x => x.CompradorId == compradorId).ToListAsync();

        return ticketsInResale;
    }
}
