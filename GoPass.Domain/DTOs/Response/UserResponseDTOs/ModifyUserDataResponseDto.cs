namespace GoPass.Domain.DTOs.Response.UserResponseDTOs;

public class ModifyUserDataResponseDto
{
    public string Email { get; set; } = default!;
    public string? Name { get; set; } = default!;
    public string? DNI { get; set; } = default!;
    public string? PhoneNumber { get; set; } = default!;
    public string? Image { get; set; }
    public string? City { get; set; } = default!;
    public string? Country { get; set; } = default!;
}
