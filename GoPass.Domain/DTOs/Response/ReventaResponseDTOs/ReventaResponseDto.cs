namespace GoPass.Domain.DTOs.Response.ReventaResponseDTOs;

public class ReventaResponseDto
{

    public int EntradaId { get; set; }
    public int VendedorId { get; set; }
    public int CompradorId { get; set; }
    public string ResaleDetail { get; set; }
    public DateTime FechaReventa { get; set; }
    public decimal Precio { get; set; }

}
