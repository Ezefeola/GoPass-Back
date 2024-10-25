using GoPass.Application.Services.Interfaces;
using GoPass.Domain.DTOs.Request.NotificationDTOs;
using System.Net.Mail;

namespace GoPass.Application.Notifications.Classes;

public class EmailNotificationObserver : Interfaces.IObserver<NotificationEmailRequestDto>
{
    private readonly IEmailService _emailService;

    public EmailNotificationObserver(IEmailService emailService)
    {
        _emailService = emailService;
    }


    public async Task Update(NotificationEmailRequestDto notificationEmailRequestDto)
    {
        string message = $"Estimado {notificationEmailRequestDto.UserName}, {notificationEmailRequestDto.Subject}, el codigoQR es {notificationEmailRequestDto.TicketQrCode}";

        notificationEmailRequestDto.Subject = notificationEmailRequestDto.Subject;
        notificationEmailRequestDto.Message = message;

        await _emailService.SendNotificationEmailAsync(notificationEmailRequestDto);
    }
    
}

