namespace GoPass.Domain.DTOs.Response.ResaleResponseDTOs;

public class PublishResaleResponseDto
{
    public string ResaleDetail { get; set; } = default!;
    public string QrCode { get; set; } = default!;
    public decimal Price { get; set; }
    public DateTime ResaleStartDate { get; set; }
}
