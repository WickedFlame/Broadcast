namespace Broadcast
{
    public interface IDispatcher<T> : IDisposable
    {
        void Register<Tc>(IMessageHandler<T> handler) where Tc : class, T;

        void Send<Tevent>(Tevent @event);

        void SendAsync<Tc>(Tc @event) where Tc : class, T;

        void Close();
    }
}
