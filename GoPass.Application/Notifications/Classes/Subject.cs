using GoPass.Application.Notifications.Interfaces;

namespace GoPass.Application.Notifications.Classes;

public class Subject<T> : ISubject<T>
{
    private readonly List<Interfaces.IObserver<T>> _observers = new();

    public void Attach(Interfaces.IObserver<T> observer)
    {
        _observers.Add(observer);
    }

    public void Detach(Interfaces.IObserver<T> observer)
    {
        _observers.Remove(observer);
    }

    public async Task Notify(T data)
    {
        foreach (var observer in _observers)
        {
            await observer.Update(data);
        }
    }
}
