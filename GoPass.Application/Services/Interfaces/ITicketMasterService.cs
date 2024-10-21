using GoPass.Domain.Models;

namespace GoPass.Application.Services.Interfaces;

public interface ITicketMasterService
{
    Task<Entrada> VerificarEntrada(string ticketId);
}
