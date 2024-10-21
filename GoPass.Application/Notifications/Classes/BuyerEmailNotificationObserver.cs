using GoPass.Application.Services.Interfaces;
using GoPass.Domain.DTOs.Request.NotificationDTOs;

namespace GoPass.Application.Notifications.Classes;

public class BuyerEmailNotificationObserver : Interfaces.IObserver<NotificationEmailRequestDto>
{
    private readonly IEmailService _emailService;

    public BuyerEmailNotificationObserver(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Update(NotificationEmailRequestDto buyerNotificationEmailRequestDto)
    {
        string subject = "Confirmacion de compra de entrada";
        string message = $"Estimado {buyerNotificationEmailRequestDto.UserName}, has comprado la entrada con codigo {buyerNotificationEmailRequestDto.TicketQrCode}";

        buyerNotificationEmailRequestDto.Subject = subject;
        buyerNotificationEmailRequestDto.Message = message;

        await _emailService.SendNotificationEmailAsync(buyerNotificationEmailRequestDto);
    }
}