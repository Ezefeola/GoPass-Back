using TicketFacker.Api.DTOs;
using TicketFacker.Api.Models;

namespace TicketFacker.Api.Mappers
{
    public static class TicketMappers
    {
        public static TicketInFakerResponseDto ToResponseDto(this Ticket ticket)
        {
            return new TicketInFakerResponseDto
            {
                Address = ticket.Address,
                Sit = ticket.Sit,
                QrCode = ticket.QrCode,
                Description = ticket.Description,
                EventDate = ticket.EventDate,
                Row = ticket.Row,
                GameName = ticket.GameName,
                Image = ticket.Image,
                Door = ticket.Door
            };
        }
    }
}
