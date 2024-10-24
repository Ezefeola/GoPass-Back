using GoPass.Domain.DTOs.Request.ReventaRequestDTOs;
using GoPass.Domain.Models;

namespace GoPass.Application.Services.Interfaces;

public interface IEntradaService : IGenericService<Entrada>
{
    Task<Entrada> PublishTicketAsync(Entrada entrada, CancellationToken cancellationToken);
    Task<bool> VerifyQrCodeAsync(string qrCode);
    Task<List<Entrada>> GetTicketsInResaleByUserIdAsync(int userId);
}
