namespace GoPass.Domain.DTOs.Request.ResaleRequestDTOs;

public class ResaleRequestDto
{
    public int TicketId { get; set; }
    public int SellerId { get; set; }
    public int BuyerId { get; set; }
    public string ResaleDetail { get; set; } = default!;
    public DateTime ResaleStartDate { get; set; }
    public decimal Price { get; set; }
}
