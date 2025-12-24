namespace Broadcast
{
    public interface IMessageHandler : IDisposable
    {
    }

    public interface IMessageHandler<in T> : IMessageHandler
    {
        void Handle(T @event);
    }
}
