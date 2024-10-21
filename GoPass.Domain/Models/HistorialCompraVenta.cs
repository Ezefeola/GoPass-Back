namespace GoPass.Domain.Models;

public class HistorialCompraVenta : BaseModel
{
    public string GameName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? Image { get; set; }
    public string Address { get; set; } = default!;
    public DateTime EventDate { get; set; } = default!;
    public string CodigoQR { get; set; } = default!;
    public bool Verificada { get; set; } = false;
    public int EntradaId { get; set; }
    public int VendedorId { get; set; }
    public int CompradorId { get; set; }
    public DateTime FechaReventa { get; set; } = DateTime.Now;
    public decimal Precio { get; set; }
    public string ResaleDetail { get; set; } = default!;
}
