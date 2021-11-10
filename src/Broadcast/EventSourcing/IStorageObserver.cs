using Broadcast.Server;

namespace Broadcast.EventSourcing
{
    /// <summary>
    /// Observer for watching the <see cref="ITaskStore"/>
    /// </summary>
    public interface IStorageObserver : IBackgroundDispatcher<ObserverContext>
    {

    }
}
