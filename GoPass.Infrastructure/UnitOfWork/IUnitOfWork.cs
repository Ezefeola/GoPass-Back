using GoPass.Infrastructure.Repositories.Interfaces;

namespace GoPass.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IReventaRepository ReventaRepository { get; }
        IEntradaRepository EntradaRepository { get; }
        IHistorialCompraVentaRepository HistorialCompraVentaRepository { get; }

        Task<int> SaveChangesAsync();
    }
}
