﻿namespace GoPass.Domain.Models;

public class TicketResaleHistory : BaseModel
{
    public string GameName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? Image { get; set; }
    public string Address { get; set; } = default!;
    public DateTime EventDate { get; set; } = default!;
    public string QRCode { get; set; } = default!;
    public bool IsTicketVerified { get; set; } = false;
    public int TicketId { get; set; }
    public int SellerId { get; set; }
    public int BuyerId { get; set; }
    public DateTime ResaleStartDate { get; set; }
    public DateTime ResaleEndDate { get; set; } = DateTime.Now;
    public decimal Price { get; set; }
    public string ResaleDetail { get; set; } = default!;
}
