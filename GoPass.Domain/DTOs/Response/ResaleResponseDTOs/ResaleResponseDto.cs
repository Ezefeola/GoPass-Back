namespace GoPass.Domain.DTOs.Response.ResaleResponseDTOs;

public class ResaleResponseDto
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public int SellerId { get; set; }
    public int BuyerId { get; set; }
    public string ResaleDetail { get; set; }
    public DateTime ResaleStartDate { get; set; }
    public decimal Price { get; set; }

}
