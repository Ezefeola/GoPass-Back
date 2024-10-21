namespace GoPass.Domain.DTOs.Response.AuthResponseDTOs;

public class UsuarioResponseDto
{
    public required string Nombre { get; set; }
    public required string DNI { get; set; }
    public required string NumeroTelefono { get; set; }
    public bool Verificado { get; set; }
}
