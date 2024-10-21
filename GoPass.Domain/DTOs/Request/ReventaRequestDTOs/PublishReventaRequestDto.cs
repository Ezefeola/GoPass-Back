
namespace GoPass.Domain.DTOs.Request.ReventaRequestDTOs;

public class PublishReventaRequestDto
{
    public string CodigoQR { get; set; } = default!;
    public string GameName { get; set; } = default!;
    public DateTime EventDate { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string ResaleDetail { get; set; } = default!;
    public decimal Precio { get; set; }
}
