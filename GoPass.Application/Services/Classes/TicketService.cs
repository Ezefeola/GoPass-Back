using GoPass.Application.Services.Interfaces;
using GoPass.Application.Utilities.Mappers;
using GoPass.Domain.DTOs.Request.PaginationDTOs;
using GoPass.Domain.DTOs.Request.ResaleRequestDTOs;
using GoPass.Domain.DTOs.Response.TicketFakerResponseDTOs;
using GoPass.Domain.DTOs.Response.TicketResponseDTOs;
using GoPass.Domain.Models;
using GoPass.Infrastructure.UnitOfWork;

namespace GoPass.Application.Services.Classes;

public class TicketService : GenericService<Ticket>, ITicketService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGopassHttpClientService _gopassHttpClientService;
    private readonly ISmappfter _smappfter;

    public TicketService
        (
            IUnitOfWork unitOfWork,
            IGopassHttpClientService gopassHttpClientService,
            ISmappfter smappfter
        ) : base(unitOfWork.TicketRepository, unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _gopassHttpClientService = gopassHttpClientService;
        _smappfter = smappfter;
    }

    public async Task<List<TicketResponseDto>> GetAllTicketsAsync()
    {
        List<Ticket> ticketsFromDb = await _genericRepository.GetAll();
        List<TicketResponseDto> ticketsFromDbResponse = _smappfter.Map<List<Ticket>, List<TicketResponseDto>>(ticketsFromDb);

        return ticketsFromDbResponse;
    }
    public async Task<List<TicketResponseDto>> GetAllTicketsWithPaginationAsync(PaginationDto paginationDto)
    {
        List<Ticket> ticketsFromDb = await _genericRepository.GetAllWithPagination(paginationDto);
        List<TicketResponseDto> ticketsFromDbResponse = _smappfter.Map<List<Ticket>, List<TicketResponseDto>>(ticketsFromDb);

        return ticketsFromDbResponse;
    }

    public async Task<bool> VerifyQrCodeAsync(string qrCode)
    {
        bool ticketQrCode = await _unitOfWork.TicketRepository.VerifyQrCodeExists(qrCode);

        return ticketQrCode!;
    }
    public async Task<List<Ticket>> GetTicketsInResaleByUserIdAsync(int userId)
    {
        List<Ticket> ticketsInresale = await _unitOfWork.TicketRepository.GetTicketsInResaleByUserId(userId);

        return ticketsInresale;
    }

    public async Task<TicketInFakerResponseDto> GetTicketFromFakerByQr(string QrCode)
    {
        PublishTicketRequestDto obtainedTicketFromFaker = await _gopassHttpClientService.GetTicketByQrAsync(QrCode);
        obtainedTicketFromFaker.IsTicketVerified = true;

        TicketInFakerResponseDto ticketInFakerResponseDto = _smappfter.Map<PublishTicketRequestDto, TicketInFakerResponseDto>(obtainedTicketFromFaker);
        return ticketInFakerResponseDto;
    }
}
