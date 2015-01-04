using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Broadcast.EventSourcing
{
    public class TaskStore : ITaskStore, IEnumerable<BackgroundTask>
    {
        static object QueueLock = new object();

        readonly List<BackgroundTask> _queue;
        readonly List<BackgroundTask> _store;

        public TaskStore()
        {
            _queue = new List<BackgroundTask>();
            _store = new List<BackgroundTask>();
        }

        public IEnumerable<BackgroundTask> CopyQueue()
        {
            lock (QueueLock)
            {
                return _queue.ToList();
            }
        }

        public int CountQueue
        {
            get
            {
                return _queue.Count;
            }
        }

        public void Add(BackgroundTask job)
        {
            lock (QueueLock)
            {
                _queue.Add(job);
            }

            job.State = TaskState.Queued;
        }

        public void SetInprocess(BackgroundTask job)
        {
            job.State = TaskState.InProcess;
        }

        public void SetProcessed(BackgroundTask job)
        {
            lock (QueueLock)
            {
                if (_queue.Contains(job))
                    _queue.Remove(job);
            }

            _store.Add(job);

            job.State = TaskState.Processed;
        }

        public IEnumerator<BackgroundTask> GetEnumerator()
        {
            return _store.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _store.GetEnumerator();
        }
    }
}
