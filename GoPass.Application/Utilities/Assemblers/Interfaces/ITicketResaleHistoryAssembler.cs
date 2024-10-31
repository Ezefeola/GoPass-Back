using GoPass.Domain.Models;

namespace GoPass.Application.Utilities.Assemblers.Interfaces
{
    public interface ITicketResaleHistoryAssembler
    {
        TicketResaleHistory AssembleFrom(Ticket ticket, Resale resale, int buyerId);
    }
}