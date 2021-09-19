using System;
using Broadcast.Configuration;
using Broadcast.Storage;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Broadcast.Diagnostics;
using Broadcast.Server;

namespace Broadcast.EventSourcing
{
	/// <summary>
	/// Represents a store conatining all Tasks
	/// </summary>
	public class TaskStore : ITaskStore, IEnumerable<ITask>
    {
        private readonly IStorage _storage;
        private readonly Options _options;
        private readonly DispatcherStorage _dispatchers;
		private readonly IDictionary<string, ServerModel> _registeredServers;

        private static readonly ItemFactory<ITaskStore> ItemFactory = new ItemFactory<ITaskStore>(() => new TaskStore());
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
		/// Uses the instance of the default <see cref="Options"/> and  the <see cref="InmemoryStorage"/> as storage
		/// </summary>
		public TaskStore() : this(new Options(), new InmemoryStorage())
		{
		}

		/// <summary>
		/// Creates a new instance of the TaskStore.
		/// Uses the instance of the default <see cref="Options"/>
		/// </summary>
		public TaskStore(IStorage storage) : this(new Options(), storage)
		{
		}

		/// <summary>
		/// Creates a new TaskStore
		/// </summary>
		/// <param name="options"></param>
		/// <param name="storage"></param>
		public TaskStore(Options options, IStorage storage)
        {
			_options = options ?? throw new ArgumentNullException(nameof(options));
			_storage = storage ?? throw new ArgumentNullException(nameof(storage));

			_storage.RegisterSubscription(new ServerHeartbeatSubscriber(this));
			_storage.RegisterSubscription(new EnqueuedTaskSubscriber(this));

            _dispatchers = new DispatcherStorage();
            _registeredServers = new Dictionary<string, ServerModel>();

            _logger = LoggerFactory.Create();
			_logger.Write("Starting new Storage");
		}

		/// <summary>
		/// Gets a enumeration of all registered <see cref="IBroadcaster"/> servers
		/// </summary>
		public IEnumerable<ServerModel> Servers => _registeredServers.Values;

		/// <summary>
		/// Adds a new Task to the queue to be processed
		/// </summary>
		/// <param name="task"></param>
		public void Add(ITask task)
		{
			_logger.Write($"Add task {task.Id} to storage");

			// serializeable tasks are propagated to all registered servers through the storage
			_storage.AddToList(new StorageKey("tasks:enqueued"), task.Id);
			_storage.Set(new StorageKey($"task:{task.Id}"), task);
		}

		/// <summary>
		/// Start dispatching all new tasks.
		/// Uses a round robin implementation to select the <see cref="IBroadcaster"/> that processes the tasks
		/// </summary>
		public void DispatchTasks()
		{
			while (_storage.TryFetchNext(new StorageKey("tasks:enqueued"), new StorageKey("tasks:dequeued"), out var id))
			{
				_logger.Write($"Dequeued task {id} for dispatchers", LogLevel.Info, Category.Log);

				// eager fetching of the data
				// first TaskStore to fetch gets to execute the Task
				var task = _storage.Get<BroadcastTask>(new StorageKey($"task:{id}"));

				if (task == null)
				{
					_logger.Write($"Could not fetch task {id} for dispatchers because the task is not in the storage", LogLevel.Warning, Category.Log);
					continue;
				}
				

				// use round robin to get the next set of dispatchers
				var dispatchers = _dispatchers.GetNext();
				foreach (var dispatcher in dispatchers)
				{
					dispatcher.Execute(task);
				}
			}
		}
		
		/// <summary>
		/// Register a set of <see cref="IDispatcher"/> to the TaskStore.
		/// Dispatchers are executed when a new Task is added to the TaskStore to notify clients of the changes.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="dispatchers"></param>
		public void RegisterDispatchers(string id, IEnumerable<IDispatcher> dispatchers)
        {
	        _logger.Write($"Register a new set of dispatchers to storage for {id}");
			_dispatchers.Add(id, dispatchers);
        }

		/// <summary>
		/// Unregister all <see cref="IDispatcher"/> that are registered for the given key.
		/// </summary>
		/// <param name="id"></param>
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
			var keys = _storage.GetKeys(new StorageKey("task"));
			foreach (var key in keys)
			{
				_storage.Delete(new StorageKey(key));
			}
		}

		/// <summary>
		/// Returns a delegate that allows accessing the connected <see cref="IStorage"/>.
		/// This is used when a component needs to store data in the <see cref="IStorage"/>.
		/// </summary>
		/// <returns></returns>
		public void Storage(Action<IStorage> action)
		{
			action(_storage);
		}

		/// <summary>
		/// Executes a delegate that allows accessing the connected <see cref="IStorage"/>.
		/// This is used when a component needs to store data in the <see cref="IStorage"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="action"></param>
		/// <returns></returns>
		public T Storage<T>(Func<IStorage, T> action)
		{
			return action(_storage);
		}

		/// <summary>
		/// Propagate the Server to the TaskStore.
		/// Poropagation is done during registration and heartbeat.
		/// </summary>
		/// <param name="server"></param>
		public void PropagateServer(ServerModel server)
		{
			lock(_registeredServers)
			{
				_registeredServers[server.Id] = server;

				// cleanup dead servers
				var expiration = DateTime.Now.Subtract(TimeSpan.FromMilliseconds(_options.HeartbeatInterval));
				var deadServers = _registeredServers.Where(item => item.Value.Heartbeat < expiration).Select(item => item.Key).ToList();
				foreach (var key in deadServers)
				{
					_registeredServers.Remove(key);
				}
			}
		}

		/// <summary>
		/// Gets the enumerator
		/// </summary>
		/// <returns></returns>
		public IEnumerator<ITask> GetEnumerator()
		{
			return GetStoredTasks().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

		private IEnumerable<ITask> GetStoredTasks()
		{
			var keys = _storage.GetKeys(new StorageKey("task:")).ToList();

			foreach (var key in keys)
			{
				yield return _storage.Get<BroadcastTask>(new StorageKey(key));
			}
		}
    }
}
