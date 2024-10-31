namespace GoPass.Domain.Models;

public class Ticket : BaseModel
{
    public int UserId { get; set; }
    public string GameName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? Image { get; set; }
    public string Address { get; set; } = default!;
    public DateTime EventDate { get; set; } = default!;
    public string QrCode { get; set; } = default!;
    public bool IsTicketVerified { get; set; } = false;

    //Navigation Property

    public virtual User? User { get; set; }
    public virtual Resale? Resale { get; set; }

}
