namespace GoPass.Domain.DTOs.Request.AuthRequestDTOs;

public class ConfirmPasswordResetRequestDto
{
    public string Password { get; set; } = default!;
    public string Email { get; set; } = default!;
}
