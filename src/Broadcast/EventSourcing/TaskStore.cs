using System;
using Broadcast.Configuration;
using Broadcast.Storage;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Broadcast.Diagnostics;
using Broadcast.Processing;
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
        private readonly IDispatcherStorage _dispatchers;
		private readonly IDictionary<string, ServerModel> _registeredServers;

        private static readonly ItemFactory<ITaskStore> ItemFactory = new ItemFactory<ITaskStore>(() => new TaskStore());
        private readonly ILogger _logger;

        private readonly ManualResetEventSlim _event;
        private readonly BackgroundServerProcess<IStorageContext> _process;
        private readonly DispatcherLock _dispatcherLock;
		private readonly StorageObserver _storageObserver;

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

			_event = new ManualResetEventSlim();
			_event.Set();

			_dispatcherLock = new DispatcherLock();
			var context = new StorageDispatcherContext
			{
				Dispatchers = _dispatchers,
				ResetEvent = _event
			};
			_process = new BackgroundServerProcess<IStorageContext>(context);

			_storageObserver = new StorageObserver(this);
        }

		/// <summary>
		/// Gets a enumeration of all registered <see cref="IBroadcaster"/> servers
		/// </summary>
		public IEnumerable<ServerModel> Servers
		{
			get
			{
				lock (_registeredServers)
				{
					return _registeredServers.Values;
				}
			}
		}

		/// <summary>
		/// Gets a <see cref="System.Threading.WaitHandle"/> that is used to wait for the event to be set.
		/// </summary>
		/// <remarks>
		/// <see cref="WaitHandle"/> should only be used if it's needed for integration with code bases that rely on having a WaitHandle.
		/// </remarks>
		public WaitHandle WaitHandle => _event.WaitHandle;

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
			_storage.PropagateEvent(new StorageKey($"task:{task.Id}"));

			// reset the event to be signaled
			_event.Reset();
		}

		/// <summary>
		/// Delete a Task from the queue and mark it as deleted in the storage
		/// </summary>
		/// <param name="id"></param>
		public void Delete(string id)
		{
			_logger.Write($"Delete task {id} from storage");

			// to delete the task we just mark it as deleted
			var taskKey = new StorageKey($"task:{id}");
			var task = _storage.Get<DataObject>(taskKey);

			//TODO: Is it correct that tasks in State 'Processing' can be deleted?

			task["State"] = TaskState.Deleted;
			_storage.Set<DataObject>(taskKey, task);

			// remove from queue!
			_storage.RemoveFromList(new StorageKey("tasks:enqueued"), id);
			_storage.RemoveFromList(new StorageKey("tasks:dequeued"), id);

			if (task["IsRecurring"].ToBool())
			{
				// remove the recurring task that is associated with this task
				var recurringTasks = _storage.GetKeys(new StorageKey("tasks:recurring:"));
				foreach (var recurringKey in recurringTasks)
				{
					// get the recurring task that references the task that is to be deleted
					var recurring = _storage.Get<RecurringTask>(new StorageKey(recurringKey));
					if (recurring?.ReferenceId == id)
					{
						_storage.Delete(new StorageKey(recurringKey));
						break;
					}
				}
			}
		}

		/// <summary>
		/// Start dispatching all new tasks.
		/// Uses a round robin implementation to select the <see cref="IBroadcaster"/> that processes the tasks
		/// </summary>
		public void DispatchTasks()
		{
			// check if a thread is allready processing the queue
			if (_dispatcherLock.IsLocked())
			{
				return;
			}

			// start new background thread to process all queued tasks
			_process.StartNew(new TaskStoreDispatcher(_dispatcherLock, _storage));
		}
		
		/// <summary>
		/// Register a set of <see cref="IDispatcher"/> to the TaskStore.
		/// Dispatchers are executed when a new Task is added to the TaskStore to notify clients of the changes.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="dispatchers"></param>
		public void RegisterDispatchers(string id, IEnumerable<IDispatcher> dispatchers)
        {
	        _logger.Write($"Register a new set of	dispatchers to storage for {id}");
			_dispatchers.Add(id, dispatchers);

			_storageObserver.Start(new ReschedulingDispatcher());
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

			_event.Reset();
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
				// servers are propagated each HeartbeatInterval
				// we remove servers that are not propagated at the double time
				var expiration = DateTime.Now.Subtract(TimeSpan.FromMilliseconds(_options.HeartbeatInterval * 2));
				var deadServers = _registeredServers.Where(item => item.Value.Heartbeat < expiration).Select(item => item.Key).ToList();
				foreach (var key in deadServers)
				{
					_registeredServers.Remove(key);
					_storage.Delete(new StorageKey($"server:{server.Name}:{server.Id}"));
				}
			}
		}

		/// <summary>
		/// Remove a server from the TaskStore.
		/// </summary>
		/// <param name="server"></param>
		public void RemoveServer(ServerModel server)
		{
			lock (_registeredServers)
			{
				if (_registeredServers.ContainsKey(server.Id))
				{
					_registeredServers.Remove(server.Id);
				}

				_storage.Delete(new StorageKey($"server:{server.Name}:{server.Id}"));
			}
		}

		/// <summary>
		/// Wait for all enqueued Tasks to be passed to the dispatchers
		/// </summary>
		/// <returns></returns>
		public bool WaitAll()
		{
			// first we wait for all enqueued tasks to change state to dequeued
			while (_dispatcherLock.IsLocked())
			{
				System.Diagnostics.Trace.WriteLine("Wait for TaskStore");
				WaitHandle.WaitOne(50);
			}

			// then we wait for all dequeued tasks to be dispatched to all dispatchers
			// all tasks are passed to the dispatchers in a own thread
			// so we wait for these to end
			_process.WaitAll();

			return true;
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
