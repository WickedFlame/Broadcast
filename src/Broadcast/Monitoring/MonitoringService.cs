using System;
using System.Collections.Generic;
using System.Linq;
using Broadcast.Server;
using Broadcast.Storage;

namespace Broadcast.Monitoring
{
	public class MonitoringService
	{
		private readonly ITaskStore _store;

		public MonitoringService(ITaskStore store)
		{
			_store = store ?? throw new ArgumentNullException(nameof(store));
		}



		public IEnumerable<ServerDescription> GetServers()
		{
			return _store.Servers.Select(s => new ServerDescription
			{
				Id = s.Id,
				Name = s.Name,
				Heartbeat = s.Heartbeat
			});
		}

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

		public IEnumerable<RecurringTaskDescription> GetRecurringTasks()
		{
			IEnumerable<RecurringTaskDescription> recurring = null;
			_store.Storage(s =>
			{
				var keys = s.GetKeys(new StorageKey($"tasks:recurring:"));

				recurring = keys.Select(k => s.Get<RecurringTask>(new StorageKey(k)))
					.Select(m => new RecurringTaskDescription
					{
						Name = m.Name,
						NextExecution = m.NextExecution
					});
			});

			return recurring;
		}
	}
}
