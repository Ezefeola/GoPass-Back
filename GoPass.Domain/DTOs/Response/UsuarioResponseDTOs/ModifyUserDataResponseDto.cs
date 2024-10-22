namespace GoPass.Domain.DTOs.Response.UsuarioResponseDTOs;

public class ModifyUserDataResponseDto
{
    public string Email { get; set; } = default!;
    public string? Nombre { get; set; } = default!;
    public string? DNI { get; set; } = default!;
    public string? NumeroTelefono { get; set; } = default!;
    public string? Image { get; set; }
    public string? City { get; set; } = default!;
    public string? Country { get; set; } = default!;
}
