namespace GoPass.Domain.Models;

public class Resale : BaseModel
{
    public int TicketId { get; set; }
    public int SellerId { get; set; }
    public int BuyerId { get; set; }
    public DateTime ResaleStartDate { get; set; } = DateTime.Now;
    public decimal Price { get; set; }
    public string ResaleDetail { get; set; } = default!;

    //Navigation Properties
    public virtual User? User { get; set; }
    public virtual Ticket? Ticket { get; set; }
}
