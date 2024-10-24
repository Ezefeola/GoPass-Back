using GoPass.Application.Services.Interfaces;
using GoPass.Application.Utilities.Mappers;
using GoPass.Domain.Models;
using GoPass.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore.Storage;

namespace GoPass.Application.Services.Classes;

public class ReventaService : GenericService<Reventa>, IReventaService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReventaService(IUnitOfWork unitOfWork) : base(unitOfWork.ReventaRepository, unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Reventa> PublishResaleAsync(Reventa reventa, int sellerId, CancellationToken cancellationToken)        
    {
        reventa.VendedorId = sellerId;
        Reventa resale = await _unitOfWork.ReventaRepository.Publish(reventa);
        return resale;
    }

    public async Task<Reventa> GetResaleByEntradaIdAsync(int entradaId)
    {
        return await _unitOfWork.ReventaRepository.GetResaleByEntradaId(entradaId);
    }

    public async Task<List<HistorialCompraVenta>> GetBoughtTicketsByCompradorIdAsync(int compradorId)
    {
        List<HistorialCompraVenta> ticketsInresale = await _unitOfWork.HistorialCompraVentaRepository.GetBoughtTicketsByCompradorId(compradorId);

        return ticketsInresale;
    }

    public async Task<HistorialCompraVenta> BuyTicketAsync(int reventaId, int compradorId, CancellationToken cancellationToken)
    {
        Reventa resale = await _unitOfWork.ReventaRepository.GetById(reventaId);

        Entrada ticket = await _unitOfWork.EntradaRepository.GetById(resale.EntradaId);

        HistorialCompraVenta historialCompraVenta = await CreateHistorialCompraVenta(ticket, resale, compradorId, cancellationToken);



        return historialCompraVenta;
    }

    public async Task<HistorialCompraVenta> CreateHistorialCompraVenta(Entrada ticket, Reventa resale, int compradorId, CancellationToken cancellationToken)
    {
        using IDbContextTransaction transaction = await _unitOfWork.BeginTransaction(cancellationToken);

        try
        {
            HistorialCompraVenta historialCompraVentaToCreate = HistorialCompraVentaMappers.MapToHistorialCompraVenta(ticket, resale, compradorId);


            await _unitOfWork.HistorialCompraVentaRepository.Create(historialCompraVentaToCreate);

            await _unitOfWork.ReventaRepository.Delete(resale.Id);
            await _unitOfWork.EntradaRepository.Delete(resale.EntradaId);

            await _unitOfWork.Complete(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return historialCompraVentaToCreate;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
