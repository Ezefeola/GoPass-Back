using GoPass.Domain.Models;

namespace GoPass.Infrastructure.Repositories.Interfaces;
public interface IHistorialCompraVentaRepository : IGenericRepository<HistorialCompraVenta>
{
    Task<List<HistorialCompraVenta>> GetBoughtTicketsByCompradorId(int compradorId);
}
