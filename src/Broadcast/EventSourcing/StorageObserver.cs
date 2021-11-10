using Broadcast.Processing;
using Broadcast.Server;
using System;

namespace Broadcast.EventSourcing
{
    /// <summary>
    /// Observe the <see cref="ITaskStore"/>
    /// </summary>
    public class StorageObserver
    {
        private readonly ITaskStore _store;
        private readonly BackgroundServerProcess<ObserverContext> _process;
        private readonly DispatcherLock _dispatcherLock;

        /// <summary>
        /// Creates a new instance of the StorageObserver
        /// </summary>
        /// <param name="store"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public StorageObserver(ITaskStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _dispatcherLock = new DispatcherLock();

            var context = new ObserverContext(_dispatcherLock, _store);
            _process = new BackgroundServerProcess<ObserverContext>(context);
            
        }

        /// <summary>
        /// Start a new <see cref="IStorageObserver"/>
        /// </summary>
        /// <param name="observer"></param>
        public void Start(IStorageObserver observer)
        {
            _process.StartNew(observer);
        }
    }
}
