using System;
using System.Collections.Generic;
using System.Linq;
using Broadcast.Server;
using Broadcast.Storage;

namespace Broadcast.Monitoring
{
	/// <summary>
	/// Service class for monitoring broadcast metrics
	/// </summary>
	public class MonitoringService
	{
		private readonly ITaskStore _store;

		/// <summary>
		/// Creates a new instance of the MonitoringService
		/// </summary>
		/// <param name="store"></param>
		public MonitoringService(ITaskStore store)
		{
			_store = store ?? throw new ArgumentNullException(nameof(store));
		}

		/// <summary>
		/// Gets a list of <see cref="ServerDescription"/> of all servers that are active
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ServerDescription> GetServers()
		{
			// ensure it is a copy with ToList()
			return _store.Servers.Select(s => new ServerDescription
			{
				Id = s.Id,
				Name = s.Name,
				Heartbeat = s.Heartbeat
			}).ToList();
		}

		/// <summary>
		/// Gets a list of all tasks in the store
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TaskDescription> GetAllTasks()
		{
			var tasks = _store.Select(t => new TaskDescription
			{
				Id = t.Id,
				Name = t.Name,
				IsRecurring = t.IsRecurring,
				State = t.State,
				Time = t.Time?.TotalMilliseconds
			}).ToList();

			var queued = _store.Storage(s => s.GetList(new StorageKey("tasks:enqueued")));
			var fetched = _store.Storage(s => s.GetList(new StorageKey("tasks:dequeued")));

			foreach (var task in tasks)
			{
				task.Queued = queued.Any(t => t == task.Id);
				task.Fetched = fetched.Any(t => t == task.Id);

				var data = _store.Storage(s => s.Get<DataObject>(new StorageKey($"tasks:values:{task.Id}")));
				if (data == null)
				{
					continue;
				}

				task.Server = data["Server"]?.ToString();
				if(long.TryParse(data["ExecutionTime"]?.ToString(), out var duration))
				{
					task.Duration = duration;
				}

				if(DateTime.TryParse(data["InProcessAt"]?.ToString(), out var start))
				{
					task.Start = start;
				}
			}

			// ensure it is a copy with ToList()
			return tasks.OrderBy(t => t.Start == null).ThenBy(t => t.Start);
		}

		/// <summary>
		/// Get all recurring tasks
		/// </summary>
		/// <returns></returns>
		public IEnumerable<RecurringTaskDescription> GetRecurringTasks()
		{
			var recurring = _store.Storage(s =>
			{
				// ensure it is a copy with ToList()
				var keys = s.GetKeys(new StorageKey($"tasks:recurring:")).ToList();

				// ensure it is a copy with ToList()
				return keys.Select(k => s.Get<RecurringTask>(new StorageKey(k)))
					.Select(m => new RecurringTaskDescription
					{
						ReferenceId = m.ReferenceId,
						Name = m.Name,
						NextExecution = m.NextExecution,
						Interval = m.Interval?.TotalMilliseconds
					}).ToList();
			});

			return recurring;
		}
	}
}
