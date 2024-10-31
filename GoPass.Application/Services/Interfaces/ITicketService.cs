using GoPass.Domain.DTOs.Request.PaginationDTOs;
using GoPass.Domain.DTOs.Response.TicketFakerResponseDTOs;
using GoPass.Domain.DTOs.Response.TicketResponseDTOs;
using GoPass.Domain.Models;

namespace GoPass.Application.Services.Interfaces;

public interface ITicketService : IGenericService<Ticket>
{
    Task<List<TicketResponseDto>> GetAllTicketsAsync();
    Task<List<TicketResponseDto>> GetAllTicketsWithPaginationAsync(PaginationDto paginationDto);
    Task<bool> VerifyQrCodeAsync(string qrCode);
    Task<List<Ticket>> GetTicketsInResaleByUserIdAsync(int userId);
    Task<TicketInFakerResponseDto> GetTicketFromFakerByQr(string QrCode);

}
