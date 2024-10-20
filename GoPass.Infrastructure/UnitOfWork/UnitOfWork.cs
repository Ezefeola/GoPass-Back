using GoPass.Infrastructure.Data;
using GoPass.Infrastructure.Repositories.Interfaces;

namespace GoPass.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public IReventaRepository ReventaRepository { get; }

        public IEntradaRepository EntradaRepository { get; }

        public IHistorialCompraVentaRepository HistorialCompraVentaRepository { get; }


        public UnitOfWork(ApplicationDbContext dbContext,
            IReventaRepository reventaRepository,
            IEntradaRepository entradaRepository,
            IHistorialCompraVentaRepository historialCompraVentaRepository
            )
        {
            _dbContext = dbContext;
            ReventaRepository = reventaRepository;
            EntradaRepository = entradaRepository;
            HistorialCompraVentaRepository = historialCompraVentaRepository;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
