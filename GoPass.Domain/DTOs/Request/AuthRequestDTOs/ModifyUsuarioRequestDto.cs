namespace GoPass.Domain.DTOs.Request.AuthRequestDTOs;

public class ModifyUsuarioRequestDto
{
    public string Name { get; set; } = default!;
    public string DNI { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string? Image { get; set; }
    public string City { get; set; } = default!;
    public string Country { get; set; } = default!;
}
