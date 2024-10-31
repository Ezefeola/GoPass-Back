namespace GoPass.Application.Notifications.Interfaces;

public interface IObserver<T>
{
    void Update(T subject);
}
