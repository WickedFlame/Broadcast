using Broadcast.Processing;
using Broadcast.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.Configuration;

namespace Broadcast.EventSourcing
{
    /// <summary>
    /// Observe the <see cref="ITaskStore"/>
    /// </summary>
    public class StorageObserver : IDisposable
    {
        private readonly BackgroundServerProcess<ObserverContext> _process;
        private readonly WaitHandle _waitHandle;

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

            _waitHandle = new Semaphore(0, Int32.MaxValue);
            StartScheduler(this, options, _waitHandle);
        }

        private void StartScheduler(StorageObserver observer, Options options, WaitHandle waitHandle)
        {
            Task.Factory.StartNew(() => ExecuteScheduler(observer, options, waitHandle),
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.Default);
        }

        private void ExecuteScheduler(StorageObserver observer, Options options, WaitHandle waitHandle)
        {
            // loop until the waithandle is disposed
            while (!waitHandle.SafeWaitHandle.IsClosed)
            {
                // start a new dispatcher to cleanup the storage
                observer.Start(new StorageCleanupDispatcher(options));

                // Delay the thread to avoid high CPU usage with the infinite loop
                waitHandle.WaitOne(TimeSpan.FromSeconds(options.StorageCleanupInterval), true);
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
            _waitHandle.Dispose();
        }
    }

    public class StorageCleanupDispatcher : IStorageObserver
    {
        private readonly Options _options;

        public StorageCleanupDispatcher(Options options)
        {
            _options = options;
        }

        public void Execute(ObserverContext context)
        {
            var tasks = GetTasks(context.Store).ToList();
            foreach (var task in tasks)
            {
                context.Store.Storage(s => s.Delete(new Storage.StorageKey($"task:{task.Id}")));
            }

            //TODO: delete servers
        }

        private IEnumerable<ITask> GetTasks(ITaskStore store)
        {
            return store.Where(t => (t.State == TaskState.Processed ||
                                     t.State == TaskState.Faulted ||
                                     t.State == TaskState.Deleted) && 
                                    t.StateChanges[t.State] < DateTime.Now.Subtract(TimeSpan.FromMilliseconds(_options.StorageLifetimeDuration)));
        }
    }
}
