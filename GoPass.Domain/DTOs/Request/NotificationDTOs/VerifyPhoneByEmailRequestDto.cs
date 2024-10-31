namespace GoPass.Domain.DTOs.Request.NotificationDTOs;

public class VerifyPhoneByEmailRequestDto
{
    public string Name { get; set; } = default!;
    public string Message { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public int VerificationCode { get; set; }
}
