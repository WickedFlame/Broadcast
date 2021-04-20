using Broadcast.Configuration;
using Broadcast.Storage;
using System.Collections;
using System.Collections.Generic;

namespace Broadcast.EventSourcing
{
	/// <summary>
	/// Represents a store conatining all Tasks
	/// </summary>
	public class TaskStore : ITaskStore, IEnumerable<ITask>
    {
        private readonly object _lockHandle = new object();

        private readonly IStorage _store;
        private readonly Options _options;

        /// <summary>
		/// Creates a new TaskStore
		/// </summary>
		/// <param name="options"></param>
        public TaskStore(Options options)
        {
			_options = options;

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
	            _store.AddToList(new StorageKey("task", _options.ServerName), task);
            }
        }

		/// <summary>
		/// Gets the enumerator
		/// </summary>
		/// <returns></returns>
        public IEnumerator<ITask> GetEnumerator()
        {
			lock(_lockHandle)
			{
				return _store.GetList<ITask>(new StorageKey("task", _options.ServerName)).GetEnumerator();
			}
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
