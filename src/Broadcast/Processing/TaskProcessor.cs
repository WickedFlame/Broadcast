using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Broadcast.EventSourcing;

namespace Broadcast.Processing
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

        public abstract void Process(DelegateTask task);

        protected virtual void ProcessItem(DelegateTask task)
        {
            Store.SetInprocess(task);

            task.Task.Compile().Invoke();

            Store.SetProcessed(task);
        }

        public void Process<T>(DelegateTask<T> task) where T : INotification
        {
            Store.Add(task);

            Store.SetInprocess(task);

            List<Action<INotification>> handlers;

            if (!Handlers.Handlers.TryGetValue(typeof(T), out handlers))
            {
                return;
            }

            foreach (var handler in handlers)
            {
                handler(task.Task.Compile().Invoke());
            }

            Store.SetProcessed(task);
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
        static object ProcessorLock = new object();

        bool _inProcess = false;

        public BackgroundTaskProcessor(ITaskStore store, INotificationHandlerStore handlers)
            : base(store, handlers)
        {
        }

        public override void Process(DelegateTask task)
        {
            Store.Add(task);

            // check if a thread is allready processing the queue
            if (_inProcess)
                return;

            // start new background thread to process all queued tasks
            Task.Run(() => ProcessTasks());
        }

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

        public override void Process(DelegateTask task)
        {
            Store.Add(task);

            Task.Run(() => ProcessItem(task));
        }
    }
}
