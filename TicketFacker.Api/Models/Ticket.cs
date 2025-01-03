﻿namespace TicketFacker.Api.Models;

public class Ticket
{
    public int Id { get; set; }
    public string GameName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? Image { get; set; }
    public string Address { get; set; } = default!;
    public DateTime EventDate { get; set; } = default!;
    public string QrCode { get; set; } = default!;
    public string Door { get; set; } = default!;
    public string Row { get; set; } = default!;
    public string Sit { get; set; } = default!;
}
