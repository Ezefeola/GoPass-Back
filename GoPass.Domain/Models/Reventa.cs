namespace GoPass.Domain.Models;

public class Reventa : BaseModel
{
    public int EntradaId { get; set; }
    public int VendedorId { get; set; }
    public int CompradorId { get; set; }
    public DateTime FechaReventa { get; set; } = DateTime.Now;
    public decimal Precio { get; set; }
    public string ResaleDetail { get; set; } = default!;

    //Navigation Properties
    public Usuario? Usuario { get; set; }
    public virtual Entrada? Entrada { get; set; }
}
