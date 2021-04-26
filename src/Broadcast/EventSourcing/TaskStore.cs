using System;
using Broadcast.Configuration;
using Broadcast.Storage;
using System.Collections;
using System.Collections.Generic;
using Broadcast.Diagnostics;

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
        private readonly DispatcherStorage _dispatchers;

        private static readonly ItemFactory<ITaskStore> ItemFactory = new ItemFactory<ITaskStore>(() => new TaskStore(Options.Default));
        private readonly ILogger _logger;

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
            _dispatchers = new DispatcherStorage();

            _logger = LoggerFactory.Create();
			_logger.Write("Starting new Storage");
		}
		
        /// <summary>
        /// Adds a new Task to the queue to be processed
        /// </summary>
        /// <param name="task"></param>
        public void Add(ITask task)
        {
	        _logger.Write($"Add task {task.Id} to storage");
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
		/// <param name="id"></param>
		/// <param name="dispatchers"></param>
		public void RegisterDispatchers(string id, IEnumerable<IDispatcher> dispatchers)
        {
	        _logger.Write($"Register a new set of dispatchers to storage for {id}");
			_dispatchers.Add(id, dispatchers);
        }

		public void UnregisterDispatchers(string id)
		{
			_logger.Write($"Remove all dispatchers for {id}");
			_dispatchers.Remove(id);
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
