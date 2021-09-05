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
			return _store.Servers.Select(s => new ServerDescription
			{
				Id = s.Id,
				Name = s.Name,
				Heartbeat = s.Heartbeat
			});
		}

		/// <summary>
		/// Gets a list of all tasks in the store
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TaskDescription> GetAllTasks()
		{
			return _store.Select(t => new TaskDescription
			{
				Id = t.Id,
				Name = t.Name,
				IsRecurring = t.IsRecurring,
				State = t.State,
				Time = t.Time
			});
		}

		/// <summary>
		/// Get all recurring tasks
		/// </summary>
		/// <returns></returns>
		public IEnumerable<RecurringTaskDescription> GetRecurringTasks()
		{
			IEnumerable<RecurringTaskDescription> recurring = null;
			_store.Storage(s =>
			{
				var keys = s.GetKeys(new StorageKey($"tasks:recurring:"));

				recurring = keys.Select(k => s.Get<RecurringTask>(new StorageKey(k)))
					.Select(m => new RecurringTaskDescription
					{
						ReferenceId = m.ReferenceId,
						Name = m.Name,
						NextExecution = m.NextExecution,
						Interval = m.Interval
					});
			});

			return recurring;
		}
	}
}
