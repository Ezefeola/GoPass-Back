namespace GoPass.Domain.DTOs.Request.ResaleRequestDTOs;

public class PublishResaleRequestDto
{
    public string QrCode { get; set; } = default!;
    public string GameName { get; set; } = default!;
    public DateTime EventDate { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string ResaleDetail { get; set; } = default!;
    public decimal Price { get; set; }
}
