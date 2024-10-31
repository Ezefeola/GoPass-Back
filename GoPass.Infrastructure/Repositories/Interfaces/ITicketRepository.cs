using GoPass.Domain.Models;

namespace GoPass.Infrastructure.Repositories.Interfaces;

public interface ITicketRepository : IGenericRepository<Ticket>
{
    Task<bool> VerifyQrCodeExists(string qrCode);
    Task<List<Ticket>> GetTicketsInResaleByUserId(int userId);
}
