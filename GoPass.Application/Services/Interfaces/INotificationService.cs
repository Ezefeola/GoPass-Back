using GoPass.Domain.Models;

namespace GoPass.Application.Services.Interfaces
{
    public interface INotificationService
    {
        Task NotifyBuyerAndSellerAsync(HistorialCompraVenta historialCompraVenta, Entrada ticket, CancellationToken cancellationToken);
    }
}