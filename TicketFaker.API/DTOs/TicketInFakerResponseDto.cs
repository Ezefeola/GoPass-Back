namespace TicketFaker.API.DTOs
{
    public class TicketInFakerResponseDto
    {
        public string GameName { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string? Image { get; set; }
        public string Address { get; set; } = default!;
        public DateTime EventDate { get; set; } = default!;
        public string CodigoQR { get; set; } = default!;
        public string Puerta { get; set; } = default!;
        public string Fila { get; set; } = default!;
        public string Asiento { get; set; } = default!;
    }
}
