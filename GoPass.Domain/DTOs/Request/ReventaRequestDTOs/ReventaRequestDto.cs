namespace GoPass.Domain.DTOs.Request.ReventaRequestDTOs;

public class ReventaRequestDto
{
    public int EntradaId { get; set; }
    public int VendedorId { get; set; }
    public int CompradorId { get; set; }
    public string ResaleDetail { get; set; } = default!;
    public DateTime FechaReventa { get; set; }
    public decimal Precio { get; set; }
}
