using System;
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
        private readonly List<IDispatcher> _dispatchers;

        private static readonly ItemFactory<ITaskStore> ItemFactory = new ItemFactory<ITaskStore>(() => new TaskStore(Options.Default));

		/// <summary>
		/// Gets the default instance of the <see cref="ITaskStore"/>
		/// </summary>
        public static ITaskStore Default => ItemFactory.Factory();

		/// <summary>
		/// Setup a new instance for the default <see cref="ITaskStore"/>
		/// </summary>
		/// <param name="setup"></param>
		public static void Setup(Func<ITaskStore> setup)
		{
			ItemFactory.Factory = setup;
		}

		/// <summary>
		/// Creates a new instance of the TaskStore.
		/// Uses the instance of the default <see cref="Options"/> Default
		/// </summary>
		public TaskStore() : this(Options.Default)
		{
		}

		/// <summary>
		/// Creates a new TaskStore
		/// </summary>
		/// <param name="options"></param>
		public TaskStore(Options options)
        {
			_options = options;

            _store = new InmemoryStorage();
            _dispatchers = new List<IDispatcher>();
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

            foreach (var dispatcher in _dispatchers)
            {
	            dispatcher.Execute(task);
            }
        }

		/// <summary>
		/// Register a set of <see cref="IDispatcher"/> to the TaskStore.
		/// Dispatchers are executed when a new Task is added to the TaskStore to notify clients of the changes.
		/// All previously registered <see cref="IDispatcher"/> will be removed.
		/// </summary>
		/// <param name="dispatchers"></param>
		public void RegisterDispatchers(IEnumerable<IDispatcher> dispatchers)
        {
	        _dispatchers.Clear();
			_dispatchers.AddRange(dispatchers);
        }

		/// <summary>
		/// Clear all Tasks from the TaskStore
		/// </summary>
        public void Clear()
        {
			lock(_lockHandle)
			{
				_store.Delete(new StorageKey("task", _options.ServerName));
			}
        }

		/// <summary>
		/// Gets the enumerator
		/// </summary>
		/// <returns></returns>
		public IEnumerator<ITask> GetEnumerator()
        {
	        lock (_lockHandle)
	        {
		        var tasks = _store.GetList<ITask>(new StorageKey("task", _options.ServerName));
		        return tasks.GetEnumerator();
	        }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
