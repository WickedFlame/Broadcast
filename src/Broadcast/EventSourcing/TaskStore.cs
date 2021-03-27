using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Broadcast.EventSourcing
{
    /// <summary>
    /// Represents a store conatining all Tasks
    /// </summary>
    public class TaskStore : ITaskStore, IEnumerable<ITask>
    {
        static object QueueLock = new object();

        readonly TaskQueue _queue;
        readonly List<ITask> _store;

        public TaskStore()
        {
            _queue = new TaskQueue();
            _store = new List<ITask>();
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
        public void Add(ITask task)
        {
            lock (QueueLock)
            {
	            _store.Add(task);
	            _queue.Enqueue(task);
            }

            task.State = TaskState.Queued;
        }

        public bool TryDequeue(out ITask task)
        {
	        return _queue.TryDequeue(out task);
        }

        /// <summary>
        /// Sets tha task to InProcess mode
        /// </summary>
        /// <param name="task"></param>
        public void SetInprocess(ITask task)
        {
            task.State = TaskState.InProcess;
        }

        /// <summary>
        /// Sets the task to Processed mode and removes it from the process queue
        /// </summary>
        /// <param name="task"></param>
        public void SetProcessed(ITask task)
        {
            task.CloseTask();
            task.State = TaskState.Processed;
        }

        public IEnumerator<ITask> GetEnumerator()
        {
            return _store.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _store.GetEnumerator();
        }
    }
}
