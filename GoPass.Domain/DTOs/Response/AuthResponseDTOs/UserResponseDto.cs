namespace GoPass.Domain.DTOs.Response.AuthResponseDTOs;

public class UserResponseDto
{
    public required string Name { get; set; }
    public string Email { get; set; }
    public required string DNI { get; set; }
    public required string PhoneNumber { get; set; }
    public bool IsVerified { get; set; }
}
