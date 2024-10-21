namespace GoPass.Domain.DTOs.Request.AuthRequestDTOs;

public class EmailValidationRequestDto
{
    public string To { get; set; } = default!; 
    public string Subject { get; set; } = default!;
    public string Body { get; set; } = default!;
}
