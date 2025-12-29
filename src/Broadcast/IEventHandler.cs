namespace Broadcast
{
    public interface IEventHandler : IDisposable
    {
    }

    public interface IEventHandler<in T> : IEventHandler
    {
        void Handle(T @event);
    }
}
