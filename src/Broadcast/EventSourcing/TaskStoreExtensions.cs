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
	}
}
