namespace GoPass.Domain.DTOs.Response;

public class TicketResponseDto
{
public string TicketId { get; set; } = default!;
    public bool IsValid { get; set; }
    public string EventName { get; set; } = default!;
public DateTime EventDate { get; set; }
}
