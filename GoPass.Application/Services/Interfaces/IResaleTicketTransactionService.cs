using GoPass.Domain.DTOs.Request.ResaleRequestDTOs;
using GoPass.Domain.DTOs.Response.ResaleResponseDTOs;
using GoPass.Domain.Models;

namespace GoPass.Application.Services.Interfaces
{
    public interface IResaleTicketTransactionService
    {
        Task<PublishResaleResponseDto> PublishResaleTicketAsync(PublishTicketRequestDto publishTicketRequestDto, PublishResaleRequestDto publishResaleRequestDto, int userId, CancellationToken cancellationToken);
    }
}