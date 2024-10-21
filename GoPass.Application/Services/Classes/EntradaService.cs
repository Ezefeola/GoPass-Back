using GoPass.Application.Services.Interfaces;
using GoPass.Application.Utilities.Mappers;
using GoPass.Domain.DTOs.Request.ReventaRequestDTOs;
using GoPass.Domain.Models;
using GoPass.Infrastructure.UnitOfWork;

namespace GoPass.Application.Services.Classes;

public class EntradaService : GenericService<Entrada>, IEntradaService
{
    private readonly IUnitOfWork _unitOfWork;

    public EntradaService(IUnitOfWork unitOfWork) : base(unitOfWork.EntradaRepository)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Entrada> PublishTicket(PublishEntradaRequestDto publishEntradaRequestDto, int userId, CancellationToken cancellationToken)
    {

        Entrada entradaExistingFaker = publishEntradaRequestDto.FromPublishEntradaRequestToModel();
        Entrada entradaToCreate = publishEntradaRequestDto.FromEntradaRequestToModel(entradaExistingFaker, userId);

        await _unitOfWork.EntradaRepository.Create(entradaToCreate);

        await _unitOfWork.Complete(cancellationToken);

        return entradaToCreate;
    }

    public async Task<bool> VerifyQrCodeAsync(string qrCode)
    {
        bool ticketQrCode = await _unitOfWork.EntradaRepository.VerifyQrCodeExists(qrCode);

        return ticketQrCode!;
    }
    public async Task<List<Entrada>> GetTicketsInResaleByUserIdAsync(int userId)
    {
        List<Entrada> ticketsInresale = await _unitOfWork.EntradaRepository.GetTicketsInResaleByUserId(userId);

        return ticketsInresale;
    }
}
