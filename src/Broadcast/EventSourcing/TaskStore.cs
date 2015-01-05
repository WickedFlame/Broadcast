using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Broadcast.EventSourcing
{
    public class TaskStore : ITaskStore, IEnumerable<WorkerTask>
    {
        static object QueueLock = new object();

        readonly List<WorkerTask> _queue;
        readonly List<WorkerTask> _store;

        public TaskStore()
        {
            _queue = new List<WorkerTask>();
            _store = new List<WorkerTask>();
        }

        public IEnumerable<WorkerTask> CopyQueue()
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

        public void Add(WorkerTask job)
        {
            lock (QueueLock)
            {
                _queue.Add(job);
            }

            job.State = TaskState.Queued;
        }

        public void SetInprocess(WorkerTask job)
        {
            job.State = TaskState.InProcess;
        }

        public void SetProcessed(WorkerTask job)
        {
            lock (QueueLock)
            {
                if (_queue.Contains(job))
                    _queue.Remove(job);
            }

            _store.Add(job);

            job.State = TaskState.Processed;
        }

        public IEnumerator<WorkerTask> GetEnumerator()
        {
            return _store.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _store.GetEnumerator();
        }
    }
}
