using GoPass.Domain.Models;

namespace GoPass.Infrastructure.Repositories.Interfaces;
public interface ITicketResaleHistoryRepository : IGenericRepository<TicketResaleHistory>
{
    Task<List<TicketResaleHistory>> GetBoughtTicketsByCompradorId(int compradorId);
    Task<TicketResaleHistory> GetTicketResakeHistoryByTicketId(int ticketId);
}
