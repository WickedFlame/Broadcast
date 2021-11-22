using System;
using System.Collections.Generic;
using System.Linq;
using Broadcast.Configuration;

namespace Broadcast.EventSourcing
{
    /// <summary>
    /// Dispatcher to cleanup the storage from old tasks
    /// </summary>
    public class StorageCleanupDispatcher : IStorageObserver
    {
        private readonly Options _options;

        /// <summary>
        /// Creates a new instance of a StorageCleanupDispatcher
        /// </summary>
        /// <param name="options"></param>
        public StorageCleanupDispatcher(Options options)
        {
            _options = options;
        }

        /// <summary>
        /// Execute the dispatcher
        /// </summary>
        /// <param name="context"></param>
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
