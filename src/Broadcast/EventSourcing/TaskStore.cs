﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Broadcast.EventSourcing
{
    /// <summary>
    /// Represents a store conatining all Tasks
    /// </summary>
    public class TaskStore : ITaskStore, IEnumerable<ITask>
    {
        private readonly object _lockHandle = new object();

        readonly ITaskStore _store;

		/// <summary>
		/// Creates a new TaskStore
		/// </summary>
        public TaskStore()
        {
            _store = new InmemoryStorage();
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
        }

		/// <summary>
		/// Gets the enumerator
		/// </summary>
		/// <returns></returns>
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
