using GoPass.Application.Services.Interfaces;
using GoPass.Application.Utilities.Assemblers.Interfaces;
using GoPass.Application.Utilities.Mappers;
using GoPass.Domain.DTOs.Request.TicketResaleHistoryRequestDTOs;
using GoPass.Domain.DTOs.Response.ResaleResponseDTOs;
using GoPass.Domain.DTOs.Response.TicketResaleHistoryDTOs;
using GoPass.Domain.Models;
using GoPass.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore.Storage;
using System.Diagnostics;

namespace GoPass.Application.Services.Classes;

public class ResaleService : GenericService<Resale>, IResaleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITicketResaleHistoryAssembler _ticketResaleHistoryAssembler;
    private readonly IGopassHttpClientService _gopassHttpClientService;
    private readonly INotificationService _notificationService;
    private readonly ISmappfter _smappfter;

    public ResaleService
        (
            IUnitOfWork unitOfWork,
            ITicketResaleHistoryAssembler ticketResaleHistoryAssembler,
            IGopassHttpClientService gopassHttpClientService,
            INotificationService notificationService,
            ISmappfter smappfter
        ) : base(unitOfWork.ResaleRepository, unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _ticketResaleHistoryAssembler = ticketResaleHistoryAssembler;
        _gopassHttpClientService = gopassHttpClientService;
        _notificationService = notificationService;
        _smappfter = smappfter;
    }

    public async Task<ResaleResponseDto> GetResaleByTicketIdAsync(int ticketId)
    {
        Resale resaleInDb = await _unitOfWork.ResaleRepository.GetResaleByTicketId(ticketId);

        ResaleResponseDto resaleResponseDto = _smappfter.Map<Resale, ResaleResponseDto>(resaleInDb);

        return resaleResponseDto;
    }

    public async Task<List<TicketResaleHistoryResponseDto>> GetBoughtTicketsByBuyerIdAsync(int buyerId)
    {
        List<TicketResaleHistory> ticketsBoughtByUser = await _unitOfWork.TicketResaleHistoryRepository.GetBoughtTicketsByCompradorId(buyerId);

        List<TicketResaleHistoryResponseDto> ticketsBoughtByUserResponse = _smappfter.Map<List<TicketResaleHistory>, List<TicketResaleHistoryResponseDto>>(ticketsBoughtByUser);

        return ticketsBoughtByUserResponse;
    }

    public async Task<TicketResaleHistoryResponseDto> BuyTicketAsync(int ticketId, int userId, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        Ticket ticketDb = await _unitOfWork.TicketRepository.GetById(ticketId);
        stopwatch.Stop();
        Console.WriteLine($"Tiempo para obtener Ticket: {stopwatch.ElapsedMilliseconds} ms");

        stopwatch.Restart();

        Resale resaleDb = await _unitOfWork.ResaleRepository.GetResaleByTicketId(ticketId);
        stopwatch.Stop();
        Console.WriteLine($"Tiempo para obtener Reventa: {stopwatch.ElapsedMilliseconds} ms");

        TicketResaleHistory ticketResaleHistoryData = _ticketResaleHistoryAssembler.AssembleFrom(ticketDb, resaleDb, userId);

        CreateTicketResaleHistoryRequestDto ticketResaleHistoryToCreate = _smappfter.Map<TicketResaleHistory, CreateTicketResaleHistoryRequestDto>(ticketResaleHistoryData);

        TicketResaleHistoryResponseDto buyedTicketData = await CreateTicketResaleHistory(ticketResaleHistoryToCreate, resaleDb.Id, ticketDb.Id, cancellationToken);

        return buyedTicketData;
    }

    public async Task<TicketResaleHistoryResponseDto> CreateTicketResaleHistory(CreateTicketResaleHistoryRequestDto createTicketResaleHistoryRequestDto, int resaleId,
            int ticketId, CancellationToken cancellationToken)
    {
        using IDbContextTransaction transaction = await _unitOfWork.BeginTransaction(cancellationToken);

        try
        {
            TicketResaleHistory ticketResaleHistoryToCreate = _smappfter.Map<CreateTicketResaleHistoryRequestDto, TicketResaleHistory>(createTicketResaleHistoryRequestDto); 
            TicketResaleHistory ticketResaleHistoryCreated =  await _unitOfWork.TicketResaleHistoryRepository.Create(ticketResaleHistoryToCreate, cancellationToken);

            await _unitOfWork.ResaleRepository.Delete(resaleId);
            await _unitOfWork.TicketRepository.Delete(ticketId);

            await _unitOfWork.Complete(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            TicketResaleHistoryResponseDto createdTicketResaleHistoryResponse = _smappfter.Map<TicketResaleHistory, TicketResaleHistoryResponseDto>(ticketResaleHistoryCreated);
            return createdTicketResaleHistoryResponse;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

}
