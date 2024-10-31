using GoPass.Application.Services.Interfaces;
using GoPass.Domain.DTOs.Request.NotificationDTOs;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;

public class EmailNotificationBackgroundService : BackgroundService
{
    private readonly ConcurrentQueue<NotificationEmailRequestDto> _notificationQueue = new();
    private readonly IEmailService _emailService;

    public EmailNotificationBackgroundService(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public void EnqueueNotification(NotificationEmailRequestDto notification)
    {
        _notificationQueue.Enqueue(notification);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_notificationQueue.TryDequeue(out var notification))
            {
                // Envía el correo electrónico
                await SendEmailNotificationAsync(notification);
            }
            else
            {
                await Task.Delay(1000, stoppingToken); // Espera 1 segundo si no hay notificaciones
            }
        }
    }

    private async Task SendEmailNotificationAsync(NotificationEmailRequestDto notification)
    {
        await _emailService.SendNotificationEmailAsync(notification);
    }
}
