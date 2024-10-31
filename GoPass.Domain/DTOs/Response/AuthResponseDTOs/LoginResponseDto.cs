namespace GoPass.Domain.DTOs.Response.AuthResponseDTOs;

public class LoginResponseDto
{
    public bool IsEmailVerified { get; set; }
    public string Email { get; set; } = default!;
    public string Token { get; set; } = default!;
}
