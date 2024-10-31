
namespace GoPass.Domain.DTOs.Request.AuthRequestDTOs;

public class RegisterRequestDto
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}
