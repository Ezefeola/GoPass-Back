using GoPass.Domain.Models;
using GoPass.Infrastructure.Data;
using GoPass.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GoPass.Infrastructure.Repositories.Classes;

public class TicketResaleHistoryRepository : GenericRepository<TicketResaleHistory>, ITicketResaleHistoryRepository
{
    public TicketResaleHistoryRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
            
    }

    public async Task<List<TicketResaleHistory>> GetBoughtTicketsByCompradorId(int buyerId)
    {
        if (buyerId <= 0)
        {
            throw new ArgumentException("El ID del vendedor no es válido.");
        }

        List<TicketResaleHistory> ticketsInResale = await _dbSet.Where(x => x.BuyerId == buyerId).ToListAsync();

        return ticketsInResale;
    }

    public async Task<TicketResaleHistory> GetTicketResakeHistoryByTicketId(int ticketId)
    {
        TicketResaleHistory? ticketResaleHistory = await _dbSet.Where(x => x.TicketId == ticketId).FirstOrDefaultAsync();

        return ticketResaleHistory!;
    }
}
