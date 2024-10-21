namespace GoPass.Domain.Models;

public class Entrada : BaseModel
{
    public string GameName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? Image { get; set; }
    public string Address { get; set; } = default!;
    public DateTime EventDate { get; set; } = default!;
    public string CodigoQR { get; set; } = default!;
    public bool Verificada { get; set; } = false;
    public int UsuarioId { get; set; }

    //Navigation Property

    public Usuario? Usuario { get; set; }
    public Reventa? Reventa { get; set; }

}
