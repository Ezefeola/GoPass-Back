using GoPass.Application.Services.Interfaces;
using GoPass.Domain.Models;
using GoPass.Infrastructure.UnitOfWork;

namespace GoPass.Application.Services.Classes
{
    public class ResaleTicketTransactionService : IResaleTicketTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEntradaService _entradaService;
        private readonly IReventaService _reventaService;

        public ResaleTicketTransactionService(IUnitOfWork unitOfWork, IEntradaService entradaService, IReventaService reventaService)
        {
            _unitOfWork = unitOfWork;
            _entradaService = entradaService;
            _reventaService = reventaService;
        }

        public async Task<Reventa> PublishResaleTicketAsync(Entrada entrada, Reventa reventa, int userId, CancellationToken cancellationToken)
        {
            using var Transaction = await _unitOfWork.BeginTransaction(cancellationToken);
            try
            {
                entrada.UsuarioId = userId;
                Entrada entradaCreated = await _entradaService.PublishTicketAsync(entrada, cancellationToken);

                reventa.Entrada = entradaCreated;
                Reventa reventaCreated = await _reventaService.PublishResaleAsync(reventa, userId, cancellationToken);

                await _unitOfWork.Complete(cancellationToken);
                await Transaction.CommitAsync(cancellationToken);

                return reventaCreated;
            }
            catch (Exception)
            {
                await Transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}