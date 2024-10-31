using GoPass.Domain.DTOs.Request.ResaleRequestDTOs;
using GoPass.Domain.DTOs.Response;
using GoPass.Domain.Models;

namespace GoPass.Application.Services.Interfaces;

public interface IGopassHttpClientService
{
    Task<PublishTicketRequestDto> GetTicketByQrAsync(string qrCode);
}
