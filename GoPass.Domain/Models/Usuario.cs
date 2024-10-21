using System.ComponentModel.DataAnnotations;

namespace GoPass.Domain.Models;

public class Usuario : BaseModel
{
    [EmailAddress]
    public string Email { get; set; } = default!;

    [DataType(DataType.Password)]
    public string Password { get; set; } = default!;
    public string? Nombre { get; set; } = default!;
    public string? DNI { get; set; } = default!;
    public string? NumeroTelefono { get; set; } = default!;
    public string? Image { get; set; }
    public string? City { get; set; } = default!;
    public string? Country { get; set; } = default!;
    public bool Verificado { get; set; } = false;
    public bool VerificadoEmail { get; set; } = false;
    public bool VerificadoSms { get; set; } = false;
    public bool Restablecer { get; set; } = false;

    public string? Token { get; set; }

    //Navigation Properties

    public List<Entrada>? Entrada { get; set; }
    public List<Reventa>? Reventa { get; set; }
}
