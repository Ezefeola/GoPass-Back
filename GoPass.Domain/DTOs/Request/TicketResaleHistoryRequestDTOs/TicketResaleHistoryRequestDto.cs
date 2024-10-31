namespace GoPass.Domain.DTOs.Request.TicketResaleHistoryRequestDTOs;

public class TicketResaleHistoryRequestDto
{
    public string GameName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? Image { get; set; }
    public string Address { get; set; } = default!;
    public DateTime EventDate { get; set; } = default!;
    public string QrCode { get; set; } = default!;
    public bool IsTicketVerified { get; set; } = false;
    public int TicketId { get; set; }
    public int SellerId { get; set; }
    public int BuyerId { get; set; }
    public decimal Price { get; set; }
    public string ResaleDetail { get; set; } = default!;
}
