using GoPass.Domain.DTOs.Request.NotificationDTOs;
using System.Collections.Concurrent;

namespace GoPass.Application.Notifications.Classes
{
    public class NotificationQueue
    {
        private readonly ConcurrentQueue<NotificationEmailRequestDto> _notifications = new();

        public void Enqueue(NotificationEmailRequestDto notification)
        {
            _notifications.Enqueue(notification);
        }

        public bool TryDequeue(out NotificationEmailRequestDto notification)
        {
            return _notifications.TryDequeue(out notification!);
        }
    }
}
