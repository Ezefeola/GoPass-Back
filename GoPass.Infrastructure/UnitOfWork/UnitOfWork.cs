using GoPass.Infrastructure.Data;
using GoPass.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace GoPass.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    public IReventaRepository ReventaRepository { get; }

    public IEntradaRepository EntradaRepository { get; }

    public IHistorialCompraVentaRepository HistorialCompraVentaRepository { get; }

    public IUsuarioRepository UsuarioRepository { get; }

    public UnitOfWork(ApplicationDbContext dbContext,
        IReventaRepository reventaRepository,
        IEntradaRepository entradaRepository,
        IHistorialCompraVentaRepository historialCompraVentaRepository,
        IUsuarioRepository usuarioRepository
        )
    {
        _dbContext = dbContext;
        ReventaRepository = reventaRepository;
        EntradaRepository = entradaRepository;
        HistorialCompraVentaRepository = historialCompraVentaRepository;
        UsuarioRepository = usuarioRepository;
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
