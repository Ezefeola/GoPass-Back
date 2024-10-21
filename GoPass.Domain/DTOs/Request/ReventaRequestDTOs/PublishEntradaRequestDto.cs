namespace GoPass.Domain.DTOs.Request.ReventaRequestDTOs;


public class PublishEntradaRequestDto
{
    public string GameName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? Image { get; set; }
    public string Address { get; set; } = default!;
    public DateTime EventDate { get; set; } = default!;
    public string CodigoQR { get; set; } = default!;
    public bool Verificada { get; set; }
}
