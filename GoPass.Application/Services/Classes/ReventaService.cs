using GoPass.Application.Services.Interfaces;
using GoPass.Application.Utilities.Mappers;
using GoPass.Domain.Models;
using GoPass.Infrastructure.Repositories.Interfaces;
using GoPass.Infrastructure.UnitOfWork;

namespace GoPass.Application.Services.Classes
{
    public class ReventaService : GenericService<Reventa>, IReventaService
    {
        //private readonly IReventaRepository _reventaRepository;
        //private readonly IEntradaRepository _entradaRepository;
        //private readonly IHistorialCompraVentaRepository _historialCompraVentaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ReventaService(IUnitOfWork unitOfWork) : base(unitOfWork.ReventaRepository)
        {
            _unitOfWork = unitOfWork;
        }
        //public ReventaService(IReventaRepository reventaRepository, IEntradaRepository entradaRepository, 
        //    IHistorialCompraVentaRepository historialCompraVentaRepository) : base(reventaRepository)
        //{
        //    _reventaRepository = reventaRepository;
        //    _entradaRepository = entradaRepository;
        //    _historialCompraVentaRepository = historialCompraVentaRepository;
        //}

        public async Task<Reventa> PublishTicketAsync(Reventa reventa, int sellerId)        
        {
            return await _unitOfWork.ReventaRepository.Publish(reventa, sellerId);
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

        public async Task<HistorialCompraVenta> BuyTicketAsync(int reventaId, int compradorId)
        {
            Reventa resale = await _unitOfWork.ReventaRepository.GetById(reventaId);

            Entrada ticket = await _unitOfWork.EntradaRepository.GetById(resale.EntradaId);

            HistorialCompraVenta historialCompraVenta = await CreateHistorialCompraVenta(ticket, resale, compradorId);

            return historialCompraVenta;
        }

        public async Task<HistorialCompraVenta> CreateHistorialCompraVenta(Entrada ticket, Reventa resale, int compradorId)
        {
            HistorialCompraVenta historialCompraVentaToCreate = HistorialCompraVentaMappers.MapToHistorialCompraVenta(ticket, resale, compradorId);

            await _unitOfWork.HistorialCompraVentaRepository.Create(historialCompraVentaToCreate);

            await _unitOfWork.ReventaRepository.Delete(resale.Id);
            await _unitOfWork.EntradaRepository.Delete(resale.EntradaId);

            return historialCompraVentaToCreate;
        }
    }
}
