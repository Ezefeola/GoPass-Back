using GoPass.Infrastructure.Data;
using GoPass.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace GoPass.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    public IResaleRepository ResaleRepository { get; }
    public ITicketRepository TicketRepository { get; }
    public ITicketResaleHistoryRepository TicketResaleHistoryRepository { get; }
    public IUserRepository UserRepository { get; }
    public IAuthRepository AuthRepository { get; set; }

    public UnitOfWork(ApplicationDbContext dbContext,
        IResaleRepository reventaRepository,
        IAuthRepository authRepository,
        ITicketRepository entradaRepository,
        ITicketResaleHistoryRepository historialCompraVentaRepository,
        IUserRepository usuarioRepository
        )
    {
        _dbContext = dbContext;
        ResaleRepository = reventaRepository;
        AuthRepository = authRepository;
        TicketRepository = entradaRepository;
        TicketResaleHistoryRepository = historialCompraVentaRepository;
        UserRepository = usuarioRepository;
    }

    public async Task<int> Complete(CancellationToken cacellationToken)
    {
        return await _dbContext.SaveChangesAsync(cacellationToken);
    }
    public void Dispose()
    {
        _dbContext.Dispose();
    }

    public async Task<IDbContextTransaction> BeginTransaction(CancellationToken cancellationToken)
    {
        return await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }
}
