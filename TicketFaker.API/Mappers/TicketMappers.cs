using TicketFaker.API.DTOs;
using TicketFaker.API.Models;

namespace TicketFaker.API.Mappers
{
    public static class TicketMappers
    {
        public static TicketInFakerResponseDto ToResponseDto(this Ticket ticket)
        {
            return new TicketInFakerResponseDto
            {
                Address = ticket.Address,
                Asiento = ticket.Asiento,
                CodigoQR = ticket.CodigoQR,
                Description = ticket.Description,
                EventDate = ticket.EventDate,
                Fila = ticket.Fila,
                GameName = ticket.GameName,
                Image = ticket.Image,
                Puerta = ticket.Puerta
            };
        }
    }
}
