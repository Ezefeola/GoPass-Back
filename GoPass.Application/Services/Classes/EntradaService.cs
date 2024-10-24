using GoPass.Application.Services.Interfaces;
using GoPass.Domain.Models;
using GoPass.Infrastructure.UnitOfWork;

namespace GoPass.Application.Services.Classes;

public class EntradaService : GenericService<Entrada>, IEntradaService
{
    private readonly IUnitOfWork _unitOfWork;

    public EntradaService(IUnitOfWork unitOfWork) : base(unitOfWork.EntradaRepository, unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Entrada> PublishTicketAsync(Entrada entrada, CancellationToken cancellationToken)
    {
        return await _unitOfWork.EntradaRepository.Create(entrada);
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
