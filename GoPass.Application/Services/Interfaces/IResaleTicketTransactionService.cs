using GoPass.Domain.DTOs.Request.ReventaRequestDTOs;
using GoPass.Domain.Models;

namespace GoPass.Application.Services.Interfaces
{
    public interface IResaleTicketTransactionService
    {
        Task<Reventa> PublishResaleTicketAsync(Entrada entrada, Reventa reventa, int userId, CancellationToken cancellationToken);
    }
}