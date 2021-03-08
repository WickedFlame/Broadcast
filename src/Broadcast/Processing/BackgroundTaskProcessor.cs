using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Broadcast.EventSourcing;

namespace Broadcast.Processing
{
    /// <summary>
    /// Runs all jobs in a backgroundthread
    /// </summary>
    public class BackgroundTaskProcessor : TaskProcessorBase, ITaskProcessor
    {
        private static readonly object ProcessorLock = new object();

        private bool _inProcess = false;

        public BackgroundTaskProcessor(ITaskStore store, INotificationHandlerStore handlers)
            : base(store, handlers)
        {
        }

        /// <summary>
        /// Process the delegate task
        /// </summary>
        /// <param name="task">The task to process</param>
        public override void Process(ITask task)
        {
            Store.Add(task);

            // check if a thread is allready processing the queue
            if (_inProcess)
            {
                return;
            }

            // start new background thread to process all queued tasks
            Task.Run(() => ProcessTasks());
        }

        private void ProcessTasks()
        {
            // check if a thread is allready processing the queue
            if (_inProcess)
            {
                return;
            }

            lock (ProcessorLock)
            {
                _inProcess = true;

                while (Store.CountQueue > 0)
                {
                    var queue = new Queue<ExpressionTask>(Store.CopyQueue().OfType<ExpressionTask>().Where(t => t.State == TaskState.Queued));
                    while (queue.Any())
                    {
                        var task = queue.Dequeue();

                        ProcessItem(task);
                    }
                }

                _inProcess = false;
            }
        }
    }
}
