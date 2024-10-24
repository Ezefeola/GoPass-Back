using GoPass.Domain.Models;

namespace GoPass.Application.Services.Interfaces;


public interface IReventaService : IGenericService<Reventa> 
{
    Task<Reventa> PublishResaleAsync(Reventa reventa, int sellerId, CancellationToken cancellationToken);
    Task<Reventa> GetResaleByEntradaIdAsync(int entradaId);
    Task<HistorialCompraVenta> BuyTicketAsync(int reventaId, int compradorId, CancellationToken cancellationToken);
    Task<List<HistorialCompraVenta>> GetBoughtTicketsByCompradorIdAsync(int compradorId);
}
