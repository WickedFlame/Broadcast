using System.Threading.Tasks;
using Broadcast.EventSourcing;

namespace Broadcast.Processing
{
    /// <summary>
    /// Runs every job in a own thread
    /// </summary>
    public class AsyncTaskProcessor : TaskProcessorBase, ITaskProcessor
    {
        public AsyncTaskProcessor(ITaskStore store, INotificationHandlerStore handlers)
            : base(store, handlers)
        {
        }

        /// <summary>
        /// Process the delegate task
        /// </summary>
        /// <param name="task">The task to process</param>
        public override void Process(DelegateTask task)
        {
            ProcessAsync(task);
        }

        public override void Process<T>(DelegateTask<T> task)
        {
            ProcessAsync(task);
        }

        /// <summary>
        /// Process the delegate task on a new Thread
        /// </summary>
        /// <param name="task">The task to process</param>
        public Task ProcessAsync(DelegateTask task)
        {
            Store.Add(task);

            return Task.Factory.StartNew(() => ProcessItem(task));
        }

        /// <summary>
        /// Process the delegate task on a new Thread
        /// </summary>
        /// <param name="task">The task to process</param>
        public Task ProcessAsync<T>(DelegateTask<T> task) where T : INotification
        {
            //Store.Add(task);

            return Task.Factory.StartNew(() => base.Process(task));
        }

        /// <summary>
        /// Processes a task without sending it to the notification handlers
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="task">The task</param>
        /// <returns>The result of the task</returns>
        public Task<T> ProcessUnhandledAsync<T>(DelegateTask<T> task)
        {
            return Task.Factory.StartNew<T>(() => base.ProcessUnhandled(task));
        }
    }
}
