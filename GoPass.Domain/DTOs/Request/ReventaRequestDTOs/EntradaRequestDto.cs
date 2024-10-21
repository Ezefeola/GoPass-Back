
namespace GoPass.Domain.DTOs.Request.ReventaRequestDTOs;

public class EntradaRequestDto
{
    public string? Image { get; set; }
    public DateTime EventDate { get; set; } = default!;
    public string Address { get; set; } = default!;
    public int UsuarioId { get; set; }
    public string CodigoQR { get; set; } = default!;
    public bool Verificada { get; set; }
}
