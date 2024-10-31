using GoPass.Domain.Models;
using GoPass.Infrastructure.Data;
using GoPass.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GoPass.Infrastructure.Repositories.Classes;

public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
{
    public TicketRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        
    }

    public async Task<bool> VerifyQrCodeExists(string qrCode)
    {
        var qrCodeExist = await _dbSet.AnyAsync(u => u.QrCode == qrCode);

        return qrCodeExist;
    }

    public async Task<List<Ticket>> GetTicketsInResaleByUserId(int userId)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("El ID del vendedor no es válido.");
        }

        List<Ticket> ticketsInResale = await _dbSet.Where(x => x.UserId == userId).AsNoTracking().ToListAsync();

        return ticketsInResale;
    }
}
