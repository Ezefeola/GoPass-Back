using GoPass.Application.Services.Interfaces;
using GoPass.Domain.DTOs.Request.NotificationDTOs;

namespace GoPass.Application.Notifications.Classes;

public class SellerEmailNotificationObserver : Interfaces.IObserver<NotificationEmailRequestDto>

{
    private readonly IEmailService _emailService;

    public SellerEmailNotificationObserver(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Update(NotificationEmailRequestDto notificationEmailRequestDto)
    {
        string subject = "Tu entrada ha sido vendida";
        string message = $"Estimado {notificationEmailRequestDto.UserName}, la entrada con código {notificationEmailRequestDto.TicketQrCode} ha sido comprada.";

        notificationEmailRequestDto.Subject = subject;
        notificationEmailRequestDto.Message = message;

        await _emailService.SendNotificationEmailAsync(notificationEmailRequestDto);
    }
}
