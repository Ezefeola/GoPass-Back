using GoPass.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace GoPass.Infrastructure.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IResaleRepository ResaleRepository { get; }
    ITicketRepository TicketRepository { get; }
    ITicketResaleHistoryRepository TicketResaleHistoryRepository { get; }
    IUserRepository UserRepository { get; }
    IAuthRepository AuthRepository { get; }

    Task<int> Complete(CancellationToken cancellationToken);
    Task<IDbContextTransaction> BeginTransaction(CancellationToken cancellationToken);
}
