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
        private object _lockHandle = new object();

        readonly List<ITask> _store;

		/// <summary>
		/// Creates a new TaskStore
		/// </summary>
        public TaskStore()
        {
            _store = new List<ITask>();
        }
		
        /// <summary>
        /// Adds a new Task to the queue to be processed
        /// </summary>
        /// <param name="task"></param>
        public void Add(ITask task)
        {
            lock (_lockHandle)
            {
	            _store.Add(task);
            }

            task.State = TaskState.Queued;
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
