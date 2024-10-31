using GoPass.Domain.DTOs.Request.NotificationDTOs;

namespace GoPass.Application.Notifications.Classes;

public class EmailNotificationObserver : Interfaces.IObserver<NotificationEmailRequestDto>
{
    private readonly EmailNotificationBackgroundService _emailNotificationBackgroundService;

    public EmailNotificationObserver(EmailNotificationBackgroundService emailNotificationBackgroundService)
    {
        _emailNotificationBackgroundService = emailNotificationBackgroundService;
    }


    public void  Update(NotificationEmailRequestDto notificationEmailRequestDto)
    {
        notificationEmailRequestDto.Subject = notificationEmailRequestDto.Subject;

        _emailNotificationBackgroundService.EnqueueNotification(notificationEmailRequestDto);
    }

}
