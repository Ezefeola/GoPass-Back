namespace GoPass.Domain.DTOs.Request.PaymentRequestDTOs;

public class PaymentStatusDto
{
    public string PaymentId { get; set; } = default!;
    public string Status { get; set; } = default!;
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
}
