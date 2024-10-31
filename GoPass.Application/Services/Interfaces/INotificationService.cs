using GoPass.Domain.DTOs.Response.TicketResaleHistoryDTOs;
using GoPass.Domain.Models;

namespace GoPass.Application.Services.Interfaces
{
    public interface INotificationService
    {
        Task NotifyBuyerAndSellerAsync(TicketResaleHistoryResponseDto ticketResaleHistoryResponseDto, CancellationToken cancellationToken);
    }
}