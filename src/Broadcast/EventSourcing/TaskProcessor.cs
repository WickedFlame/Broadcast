using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Broadcast.EventSourcing
{
    public abstract class TaskProcessorBase : IDisposable
    {
        private readonly ITaskStore _store;
        protected ITaskStore Store
        {
            get
            {
                return _store;
            }
        }

        public TaskProcessorBase(ITaskStore store)
        {
            _store = store;
        }

        protected virtual void ProcessItem(BackgroundTask job)
        {
            Store.SetInprocess(job);

            job.Task.Compile().Invoke();

            Store.SetProcessed(job);
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
        public TaskProcessor(ITaskStore store)
            : base(store)
        {
        }

        public void Process(BackgroundTask job)
        {
            Store.Add(job);

            ProcessItem(job);
        }
    }

    /// <summary>
    /// Runs all jobs in a backgroundthread
    /// </summary>
    public class BackgroundTaskProcessor : TaskProcessorBase, ITaskProcessor
    {
        private static object ProcessorLock = new object();

        public BackgroundTaskProcessor(ITaskStore store)
            : base(store)
        {
        }

        public void Process(BackgroundTask task)
        {
            Store.Add(task);

            Task.Run(() => ProcessTasks());
        }

        bool _inProcess = false;

        private void ProcessTasks()
        {
            lock (ProcessorLock)
            {
                if (_inProcess)
                    return;

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
        public AsyncTaskProcessor(ITaskStore store)
            : base(store)
        {
        }

        public void Process(BackgroundTask task)
        {
            Store.Add(task);

            Task.Run(() => ProcessItem(task));
        }
    }
}
