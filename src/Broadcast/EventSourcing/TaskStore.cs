using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Broadcast.EventSourcing
{
    /// <summary>
    /// Represents a store conatining all Tasks
    /// </summary>
    public class TaskStore : ITaskStore, IEnumerable<BroadcastTask>
    {
        static object QueueLock = new object();

        readonly List<BroadcastTask> _queue;
        readonly List<BroadcastTask> _store;

        public TaskStore()
        {
            _queue = new List<BroadcastTask>();
            _store = new List<BroadcastTask>();
        }

        /// <summary>
        /// Copies all Tasks that have been Queued to ne new List
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BroadcastTask> CopyQueue()
        {
            lock (QueueLock)
            {
                return _queue.ToList();
            }
        }

        /// <summary>
        /// Counts all unprocessed Tasks that are contained in the Queue
        /// </summary>
        public int CountQueue
        {
            get
            {
                return _queue.Count;
            }
        }

        /// <summary>
        /// Adds a new Task to the queue to be processed
        /// </summary>
        /// <param name="task"></param>
        public void Add(BroadcastTask task)
        {
            lock (QueueLock)
            {
                _queue.Add(task);
            }

            task.State = TaskState.Queued;
        }

        /// <summary>
        /// Sets tha task to InProcess mode
        /// </summary>
        /// <param name="task"></param>
        public void SetInprocess(BroadcastTask task)
        {
            task.State = TaskState.InProcess;
        }

        /// <summary>
        /// Sets the task to Processed mode and removes it from the process queue
        /// </summary>
        /// <param name="task"></param>
        public void SetProcessed(BroadcastTask task)
        {
            lock (QueueLock)
            {
                if (_queue.Contains(task))
                    _queue.Remove(task);
            }

            _store.Add(task);

            task.CloseTask();
            task.State = TaskState.Processed;
        }

        public IEnumerator<BroadcastTask> GetEnumerator()
        {
            return _store.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _store.GetEnumerator();
        }
    }
}
