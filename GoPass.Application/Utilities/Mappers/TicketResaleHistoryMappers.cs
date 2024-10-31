using GoPass.Domain.Models;

namespace GoPass.Application.Utilities.Mappers;

public static class TicketResaleHistoryMappers
{
    public static TicketResaleHistory MapToHistorialCompraVenta(Ticket ticket, Resale resale, int buyerId)
    {
        return new TicketResaleHistory
        {
            GameName = ticket.GameName,
            Description = ticket.Description,
            Image = ticket.Image,
            Address = ticket.Address,
            EventDate = ticket.EventDate,
            QRCode = ticket.QrCode,
            IsTicketVerified = ticket.IsTicketVerified,
            TicketId = resale.TicketId,
            SellerId = resale.SellerId,
            BuyerId = buyerId,
            Price = resale.Price,
            ResaleDetail = resale.ResaleDetail
        };
    }
}
