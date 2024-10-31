using GoPass.Domain.DTOs.Request.TicketResaleHistoryRequestDTOs;
using GoPass.Domain.DTOs.Response.ResaleResponseDTOs;
using GoPass.Domain.DTOs.Response.TicketResaleHistoryDTOs;
using GoPass.Domain.Models;

namespace GoPass.Application.Services.Interfaces;


public interface IResaleService : IGenericService<Resale> 
{
    Task<ResaleResponseDto> GetResaleByTicketIdAsync(int ticketId);
    Task<TicketResaleHistoryResponseDto> BuyTicketAsync(int resaleId, int buyerId, CancellationToken cancellationToken);
    Task<List<TicketResaleHistoryResponseDto>> GetBoughtTicketsByBuyerIdAsync(int buyerId);
}
