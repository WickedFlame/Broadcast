using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Broadcast.EventSourcing;

namespace Broadcast.Processing
{
    /// <summary>
    /// A class that can process different kinds of delegates and notifications
    /// </summary>
    public abstract class TaskProcessorBase : ITaskProcessor, IDisposable
    {
        private readonly ITaskStore _store;
        private readonly INotificationHandlerStore _handlers;

        public TaskProcessorBase(ITaskStore store, INotificationHandlerStore handlers)
        {
            _store = store;
            _handlers = handlers;
        }

        protected ITaskStore Store => _store;

        protected INotificationHandlerStore Handlers => _handlers;

        /// <summary>
        /// Add a delegate handler to the store
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        public void AddHandler<T>(Action<T> target) where T : INotification
        {
            Handlers.AddHandler(target);
        }

        /// <summary>
        /// Process the delegate task
        /// </summary>
        /// <param name="task"></param>
        public abstract void Process(DelegateTask task);

        protected virtual void ProcessItem(DelegateTask task)
        {
            Store.SetInprocess(task);

            try
            {
                task.Task.Invoke();
            }
            catch (Exception ex)
            {
                //TODO: set taskt to faulted
                //TODO: log exception
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

            Store.SetProcessed(task);
        }

        /// <summary>
        /// Process the delegate task
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="notification"></param>
        public virtual void Process<T>(DelegateTask<T> task) where T : INotification
        {
            Store.Add(task);

            // mark the task to be in process
            Store.SetInprocess(task);

            List<Action<INotification>> handlers;

            try
            {
                // get the job item from the task
                var item = task.Task.Invoke();

                // try to find the handlers
                if (!Handlers.Handlers.TryGetValue(typeof(T), out handlers))
                {
                    // it could be that T is of a base/inherited type but the handler is of a object type
                    if (!Handlers.Handlers.TryGetValue(item.GetType(), out handlers))
                    {
                        return;
                    }
                }

                // run all handlers with the value
                foreach (var handler in handlers)
                {
                    handler(item);
                }
            }
            catch (Exception ex)
            {
                //TODO: set taskt to faulted
                //TODO: log exception
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

            // set the task to processed state
            Store.SetProcessed(task);
        }

        /// <summary>
        /// Processes a task without sending it to the notification handlers
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="task">The task</param>
        /// <returns>The result of the task</returns>
        public T ProcessUnhandled<T>(DelegateTask<T> task)
        {
            Store.Add(task);

            // mark the task to be in process
            Store.SetInprocess(task);
            T item = default(T);

            try
            {
                // get the job item from the task
                item = task.Task.Invoke();
            }
            catch (Exception ex)
            {
                //TODO: set taskt to faulted
                //TODO: log exception
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }

            // set the task to processed state
            Store.SetProcessed(task);

            return item;
        }

        public void Dispose()
        {
        }
    }

    /// <summary>
    /// Runs all jobs in the main thread
    /// </summary>
    public class TaskProcessor : TaskProcessorBase, ITaskProcessor
    {
        public TaskProcessor(ITaskStore store, INotificationHandlerStore handlers)
            : base(store, handlers)
        {
        }

        /// <summary>
        /// Process the delegate task
        /// </summary>
        /// <param name="task">The task to process</param>
        public override void Process(DelegateTask task)
        {
            Store.Add(task);

            ProcessItem(task);
        }
    }

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
        public override void Process(DelegateTask task)
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
                    var queue = new Queue<DelegateTask>(Store.CopyQueue().OfType<DelegateTask>().Where(t => t.State == TaskState.Queued));
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
