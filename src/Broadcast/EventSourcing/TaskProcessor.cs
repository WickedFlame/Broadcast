using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Broadcast.EventSourcing
{
    public abstract class TaskProcessorBase : ITaskProcessor, IDisposable
    {
        private readonly ITaskStore _store;
        protected ITaskStore Store
        {
            get
            {
                return _store;
            }
        }

        private readonly INotificationHandlerStore _handlers;
        protected INotificationHandlerStore Handlers
        {
            get
            {
                return _handlers;
            }
        }

        public TaskProcessorBase(ITaskStore store, INotificationHandlerStore handlers)
        {
            _store = store;
            _handlers = handlers;
        }

        public void AddHandler<T>(Action<T> target) where T : INotification
        {
            Handlers.AddHandler(target);
        }

        public abstract void Process(BackgroundTask task);

        protected virtual void ProcessItem(BackgroundTask task)
        {
            Store.SetInprocess(task);

            task.Task.Compile().Invoke();

            Store.SetProcessed(task);
        }

        public void Process<T>(NotificationTask<T> notification) where T : INotification
        {
            List<Action<INotification>> handlers;

            if (!Handlers.Handlers.TryGetValue(typeof(T), out handlers))
            {
                return;
            }

            foreach (var handler in handlers)
            {
                handler(notification.Task.Compile().Invoke());
            }
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

        public override void Process(BackgroundTask task)
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
        private static object ProcessorLock = new object();

        public BackgroundTaskProcessor(ITaskStore store, INotificationHandlerStore handlers)
            : base(store, handlers)
        {
        }

        public override void Process(BackgroundTask task)
        {
            Store.Add(task);

            // check if a thread is allready processing the queue
            if (_inProcess)
                return;

            // start new background thread to process all queued tasks
            Task.Run(() => ProcessTasks());
        }

        bool _inProcess = false;

        private void ProcessTasks()
        {
            // check if a thread is allready processing the queue
            if (_inProcess)
                return;

            lock (ProcessorLock)
            {
                _inProcess = true;

                while (Store.CountQueue > 0)
                {
                    var queue = new Queue<BackgroundTask>(Store.CopyQueue().Where(t => t.State == TaskState.Queued));
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

        public override void Process(BackgroundTask task)
        {
            Store.Add(task);

            Task.Run(() => ProcessItem(task));
        }
    }
}
