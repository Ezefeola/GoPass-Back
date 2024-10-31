using GoPass.Application.Services.Interfaces;
using GoPass.Application.Utilities.Mappers;
using GoPass.Domain.DTOs.Request.ResaleRequestDTOs;
using GoPass.Domain.DTOs.Response.ResaleResponseDTOs;
using GoPass.Domain.DTOs.Response.TicketResponseDTOs;
using GoPass.Domain.Models;
using GoPass.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

namespace GoPass.Application.Services.Classes
{
    public class ResaleTicketTransactionService : IResaleTicketTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITicketService _ticketService;
        private readonly IResaleService _resaleService;
        private readonly ISmappfter _smappfter;

        public ResaleTicketTransactionService
            (
                IUnitOfWork unitOfWork, 
                ITicketService ticketService, 
                IResaleService resaleService,
                ISmappfter smappfter
            )
        {
            _unitOfWork = unitOfWork;
            _ticketService = ticketService;
            _resaleService = resaleService;
            _smappfter = smappfter;
        }

        public async Task<PublishResaleResponseDto> PublishResaleTicketAsync(PublishTicketRequestDto publishTicketRequestDto, PublishResaleRequestDto publishResaleRequestDto, int userId, CancellationToken cancellationToken)
        {
            using var Transaction = await _unitOfWork.BeginTransaction(cancellationToken);
            try
            {
                Ticket ticketToCreate = _smappfter.Map<PublishTicketRequestDto, Ticket>(publishTicketRequestDto);
                ticketToCreate.UserId = userId;
                Ticket ticketCreated = await _unitOfWork.TicketRepository.Create(ticketToCreate, cancellationToken);

                Resale resaleToCreate = _smappfter.Map<PublishResaleRequestDto, Resale>(publishResaleRequestDto);
                resaleToCreate.SellerId = userId;
                resaleToCreate.Ticket = ticketCreated;
                Resale resaleCreated = await _unitOfWork.ResaleRepository.Create(resaleToCreate, cancellationToken);

                await _unitOfWork.Complete(cancellationToken);
                await Transaction.CommitAsync(cancellationToken);

                PublishResaleResponseDto publishResaleResponseDto = _smappfter.Map<Resale, PublishResaleResponseDto>(resaleToCreate);
                return publishResaleResponseDto;
            }
            catch (Exception)
            {
                await Transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}