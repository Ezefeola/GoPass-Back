
using GoPass.Domain.Models;

namespace GoPass.Infrastructure.Repositories.Interfaces;
public interface IResaleRepository : IGenericRepository<Resale>
{
    Task<Resale> Publish(Resale resale);
    Task<Resale> GetResaleByTicketId(int ticketId);
}
