using Azure.Core;
using GoPass.Application.Services.Interfaces;
using GoPass.Application.Utilities.Mappers;
using GoPass.Domain.DTOs.Request.ReventaRequestDTOs;
using GoPass.Domain.Models;
using GoPass.Infrastructure.Repositories.Classes;
using GoPass.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GoPass.Application.Services.Classes
{
    public class ReventaService : GenericService<Reventa>, IReventaService
    {
        private readonly IReventaRepository _reventaRepository;
        private readonly IEntradaRepository _entradaRepository;
        private readonly IHistorialCompraVentaRepository _historialCompraVentaRepository;

        public ReventaService(IReventaRepository reventaRepository, IEntradaRepository entradaRepository, 
            IHistorialCompraVentaRepository historialCompraVentaRepository) : base(reventaRepository)
        {
            _reventaRepository = reventaRepository;
            _entradaRepository = entradaRepository;
            _historialCompraVentaRepository = historialCompraVentaRepository;
        }

        public async Task<Reventa> PublishTicketAsync(Reventa reventa, int sellerId)        
        {
            return await _reventaRepository.Publish(reventa, sellerId);
        }

        public async Task<Reventa> GetResaleByEntradaIdAsync(int entradaId)
        {
            return await _reventaRepository.GetResaleByEntradaId(entradaId);
        }

        public async Task<List<HistorialCompraVenta>> GetBoughtTicketsByCompradorIdAsync(int compradorId)
        {
            List<HistorialCompraVenta> ticketsInresale = await _historialCompraVentaRepository.GetBoughtTicketsByCompradorId(compradorId);

            return ticketsInresale;
        }

        public async Task<HistorialCompraVenta> BuyTicketAsync(int reventaId, int compradorId)
        {
            Reventa resale = await _reventaRepository.GetById(reventaId);

            Entrada ticket = await _entradaRepository.GetById(resale.EntradaId);

            HistorialCompraVenta historialCompraVenta = await CreateHistorialCompraVenta(ticket, resale, compradorId);

            return historialCompraVenta;
        }

        public async Task<HistorialCompraVenta> CreateHistorialCompraVenta(Entrada ticket, Reventa resale, int compradorId)
        {
            HistorialCompraVenta historialCompraVentaToCreate = ReventaMappers.FromHistorialCompraVentaRequestToModel(ticket, resale);

            await _historialCompraVentaRepository.Create(historialCompraVentaToCreate);

            await _reventaRepository.Delete(resale.Id);
            await _entradaRepository.Delete(resale.EntradaId);

            return historialCompraVentaToCreate;
        }
    }
}
