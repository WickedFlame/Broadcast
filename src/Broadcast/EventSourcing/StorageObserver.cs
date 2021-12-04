using Broadcast.Processing;
using Broadcast.Server;
using System;
using System.Threading;
using Broadcast.Configuration;

namespace Broadcast.EventSourcing
{
    /// <summary>
    /// Observe the <see cref="ITaskStore"/>
    /// </summary>
    public class StorageObserver : IDisposable
    {
        private readonly BackgroundServerProcess<ObserverContext> _process;
        private readonly ThreadWait _threadWait;

        /// <summary>
        /// Creates a new instance of the StorageObserver
        /// </summary>
        /// <param name="store"></param>
        /// <param name="options"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public StorageObserver(ITaskStore store, Options options)
        {
            if(store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            var context = new ObserverContext(new DispatcherLock(), store);
            _process = new BackgroundServerProcess<ObserverContext>(context);

            _threadWait = new ThreadWait();
            StartScheduler(_process, options, _threadWait);
        }

        private void StartScheduler(BackgroundServerProcess<ObserverContext> process, Options options, ThreadWait threadWait)
        {
            // Task.Factory.StartNew : Starts a new task that will run in a thread pool thread or may run in the same thread.
            // If it is ran in a thread pool thread, the thread is returned to the pool when done.
            // Thread creation/destruction is an expensive process.
            //
            // new Thread().Start() : Will always run in a new thread
            //
            // Here we use a Thread instead of a task.
            // This way we can ensure that the thread is a backgroundthread
            // Because it is only a cleanup process, it is run with the lowest ThreadPriority
            //
            var thread = new Thread(() => ExecuteScheduler(process, options, threadWait))
            {
                IsBackground = true,
                Name = $"{nameof(StorageObserver)}",
                Priority = ThreadPriority.Lowest
            };
            thread.Start();
        }

        private async void ExecuteScheduler(BackgroundServerProcess<ObserverContext> process, Options options, ThreadWait threadWait)
        {
            // loop until the waithandle is disposed
            while (threadWait.IsOpen)
            {
                // Delay the thread to avoid high CPU usage with the infinite loop
                await threadWait.WaitOne(options.StorageCleanupInterval);

                // start a new dispatcher to cleanup the storage
                process.StartNew(new StorageCleanupDispatcher(options));
            }
        }

        /// <summary>
        /// Start a new <see cref="IStorageObserver"/>
        /// </summary>
        /// <param name="observer"></param>
        public void Start(IStorageObserver observer)
        {
            _process.StartNew(observer);
        }

        /// <summary>
        /// Dispose the StorageObserver
        /// </summary>
        public void Dispose()
        {
            _threadWait.Dispose();
        }
    }
}
