namespace GoPass.Domain.DTOs.Response.TicketResponseDTOs
{
    public class TicketNotificationDataResponseDto
    {
        public int UserId { get; set; }
        public string GameName { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string? Image { get; set; }
        public string Address { get; set; } = default!;
        public DateTime EventDate { get; set; } = default!;
        public string QrCode { get; set; } = default!;
        public bool IsTicketVerified { get; set; } = false;
    }
}
