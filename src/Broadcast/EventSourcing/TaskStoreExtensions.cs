using System.Collections.Generic;
using System.Linq;
using Broadcast.Storage;

namespace Broadcast.EventSourcing
{
	public static class TaskStoreExtensions
	{
		public static IEnumerable<string> GetEnqueuedTasks(this ITaskStore store)
		{
			return store.Storage(s => s.GetEnqueuedTasks());
		}
		public static IEnumerable<string> GetEnqueuedTasks(this IStorage storage)
		{
			return storage.GetList(new StorageKey("tasks:enqueued"));
		}

		public static IEnumerable<string> GetFetchedTasks(this ITaskStore store)
		{
			return store.Storage(s => s.GetFetchedTasks());
		}

		public static IEnumerable<string> GetFetchedTasks(this IStorage storage)
		{
			return storage.GetList(new StorageKey("tasks:dequeued"));
		}

		/// <summary>
		/// Get the Server Id based on the queue that is registered to the task
		/// </summary>
		/// <param name="storage"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		internal static string GetServerIdFromQueue(this IStorage storage, string id)
		{
			var queue = storage.Get<DataObject>(new StorageKey($"tasks:values:{id}"))?["Queue"];
			
			if (!string.IsNullOrEmpty(queue?.ToString()))
			{
				// Get the Id of the server
				var key = storage.GetKeys(new StorageKey($"server:{queue}:")).FirstOrDefault();
				if (key != null)
				{
					return storage.Get<DataObject>(new StorageKey(key))["Id"]?.ToString();
				}
			}

			return null;
		}

		/// <summary>
		/// Assign the task to the queue it is processed on
		/// </summary>
		/// <param name="store"></param>
		/// <param name="taskId"></param>
		/// <param name="queue">The name of the Server that the task is processed on</param>
		internal static void AssignTaskToQueue(this ITaskStore store, string taskId, string queue)
		{
			store.Storage(s =>
			{
				// set the servername where the queue is working on
				s.SetValues(new StorageKey($"tasks:values:{taskId}"), new DataObject
				{
					{"Queue", queue}
				});

				// assign the task to the queue
				s.AddToList(new StorageKey($"queue:{queue}"), taskId);
			});
		}

		/// <summary>
		/// Removes the task from list of tasks that are processed in the queue. The assignment on the task will not be removed.
		/// </summary>
		/// <param name="store"></param>
		/// <param name="taskId"></param>
		/// <param name="queue">The name of the Server that the task was processed on</param>
		internal static void RemoveTaskFromQueue(this ITaskStore store, string taskId, string queue)
		{
			store.Storage(s => s.RemoveFromList(new StorageKey($"queue:{queue}"), taskId));
		}
	}
}
