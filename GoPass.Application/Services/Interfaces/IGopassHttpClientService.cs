using GoPass.Domain.Models;

namespace GoPass.Application.Services.Interfaces;

public interface IGopassHttpClientService
{
    Task<Entrada> GetTicketByQrAsync(string qrCode);
}
