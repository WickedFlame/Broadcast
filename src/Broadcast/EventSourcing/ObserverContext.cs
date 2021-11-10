using Broadcast.Processing;
using Broadcast.Server;

namespace Broadcast.EventSourcing
{
    /// <summary>
    /// Context used by the <see cref="StorageObserver"/>
    /// </summary>
    public class ObserverContext : IServerContext
    {
        /// <summary>
        /// Creates a new instance of the context
        /// </summary>
        /// <param name="dispatcherLock"></param>
        /// <param name="store"></param>
        public ObserverContext(DispatcherLock dispatcherLock, ITaskStore store)
        {
            DispatcherLock = dispatcherLock;
            Store = store;
        }

        /// <summary>
        /// Gets the <see cref="DispatcherLock"/> for the <see cref="StorageObserver"/>
        /// </summary>
        public DispatcherLock DispatcherLock { get; }

        /// <summary>
        /// Gets the <see cref="ITaskStore"/> that the <see cref="StorageObserver"/> is related to
        /// </summary>
        public ITaskStore Store { get; }
    }
}
