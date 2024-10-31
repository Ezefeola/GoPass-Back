namespace GoPass.Domain.DTOs.Request.ResaleRequestDTOs;

public class TicketRequestDto
{
    public string? Image { get; set; }
    public DateTime EventDate { get; set; } = default!;
    public string Address { get; set; } = default!;
    public int UserId { get; set; }
    public string QrCode { get; set; } = default!;
    public bool IsTicketVerified { get; set; }
}
