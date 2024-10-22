using GoPass.Domain.Models;
using GoPass.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace GoPass.Infrastructure.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IReventaRepository ReventaRepository { get; }
    IEntradaRepository EntradaRepository { get; }
    IHistorialCompraVentaRepository HistorialCompraVentaRepository { get; }
    IUsuarioRepository UsuarioRepository { get; }

    Task<int> Complete(CancellationToken cancellationToken);
    Task<IDbContextTransaction> BeginTransaction(CancellationToken cancellationToken);
}
