namespace GoPass.Domain.DTOs.Request.PaymentRequestDTOs;

public class PaymentRequestDto
{
    public decimal Amount { get; set; }
    public string PaymentMethodId { get; set; } = default!;
    public string PaymentTypeId { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string ReturnUrl { get; set; } = default!;
}
