
using GoPass.Domain.Models;

namespace GoPass.Infrastructure.Repositories.Interfaces;
public interface IReventaRepository : IGenericRepository<Reventa>
{
    Task<Reventa> Publish(Reventa reventa, int vendedorId);
    Task<Reventa> GetResaleByEntradaId(int entradaId);
    //Task<List<Reventa>> GetBoughtTicketsByCompradorId(int compradorId);
    //Task<Reventa> BuyTicket(int resaleId);
}
