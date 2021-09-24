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
	}
}
