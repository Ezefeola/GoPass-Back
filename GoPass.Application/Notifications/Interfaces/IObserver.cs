namespace GoPass.Application.Notifications.Interfaces;

public interface IObserver<T>
{
    Task Update(T subject);
}
