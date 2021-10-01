using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Broadcast.EventSourcing;
using Broadcast.Storage;

namespace Broadcast.Dashboard.Dispatchers.Models
{
	/// <summary>
	/// Service that provides stored data in form of <see cref="StorageItem"/>
	/// </summary>
	public class StorageItemService
	{
		private readonly ITaskStore _store;

		/// <summary>
		/// Creates a new instance of StorageItemService
		/// </summary>
		/// <param name="store"></param>
		public StorageItemService(ITaskStore store)
		{
			_store = store ?? throw new ArgumentNullException(nameof(store));
		}

		/// <summary>
		/// Get all data of a <see cref="ITask"/> as a <see cref="StorageItem"/>
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public StorageItem GetTask(string id)
		{
			var task = _store.Storage(s =>
			{
				var data = s.Get<DataObject>(new StorageKey($"task:{id}"));
				if (data == null)
				{
					return null;
				}

				var values = data.Select(d => new StorageProperty(d.Key, d.Value))
					.ToList();

				data = s.Get<DataObject>(new StorageKey($"tasks:values:{id}"));
				if (data != null)
				{
					values.AddRange(data.Select(d => new StorageProperty(d.Key, d.Value)));
				}

				var groups = new List<StoragePropertyGroup>
				{
					new StoragePropertyGroup
					{
						Title = "Task",
						Values = new List<StorageProperty>
						{
							values.FirstOrDefault(v => v.Key == "Id"),
							values.FirstOrDefault(v => v.Key == "Name"),
							values.FirstOrDefault(v => v.Key == "Type"),
							values.FirstOrDefault(v => v.Key == "Method")
						}.Where(t => t != null && !string.IsNullOrEmpty(t.Value))
					},
					new StoragePropertyGroup
					{
						Title = "State",
						Values = new List<StorageProperty>
						{
							values.FirstOrDefault(v => v.Key == "State"),
							new StorageProperty("Duration", values.FirstOrDefault(v => v.Key == "ExecutionTime")?.Value.ToDuration())
						}.Where(t => t != null && !string.IsNullOrEmpty(t.Value))
					},
					new StoragePropertyGroup
					{
						Title = "Processing",
						Values = new List<StorageProperty>
						{
							values.FirstOrDefault(v => v.Key == "Server"),
							new StorageProperty("Start", values.FirstOrDefault(v => v.Key == "InProcessAt")?.Value.ToFormattedDateTime()),
							new StorageProperty("End", values.FirstOrDefault(v => v.Key == "ExecutedAt")?.Value.ToFormattedDateTime())
						}.Where(t => t != null && !string.IsNullOrEmpty(t.Value))
					},
					new StoragePropertyGroup
					{
						Title = "Statechanges",
						Values = new List<StorageProperty>
						{
							new StorageProperty("Created at", values.FirstOrDefault(v => v.Key == "StateChanges:New")?.Value.ToFormattedDateTime()),
							new StorageProperty("Enqueued at", values.FirstOrDefault(v => v.Key == "StateChanges:Queued")?.Value.ToFormattedDateTime()),
							new StorageProperty("Dequeue at", values.FirstOrDefault(v => v.Key == "StateChanges:Dequeued")?.Value.ToFormattedDateTime()),
							new StorageProperty("Start processing at", values.FirstOrDefault(v => v.Key == "StateChanges:InProcess")?.Value.ToFormattedDateTime()),
							new StorageProperty("End processing at", values.FirstOrDefault(v => v.Key == "StateChanges:Processed")?.Value.ToFormattedDateTime())
						}.Where(t => t != null && !string.IsNullOrEmpty(t.Value))
					}
				};

				var item = new StorageItem
				{
					Key = id.ToString(),
					Title = values.FirstOrDefault(v => v.Key == "Name")?.Value,
					//Values = values,
					Groups = groups
				};

				return item;
			});

			return task;
		}

		/// <summary>
		/// Get all data of a Server as a <see cref="StorageItem"/>
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public StorageItem GetServer(string id)
		{
			var server = _store.Storage(s =>
			{
				var key = s.GetKeys(new StorageKey($"server:")).FirstOrDefault(k => k.EndsWith(id));
				if (string.IsNullOrEmpty(key))
				{
					return null;
				}

				var data = s.Get<DataObject>(new StorageKey(key));
				return new StorageItem
				{
					Key = id.ToString(),
					Title = data["Name"]?.ToString(),
					Groups = new List<StoragePropertyGroup>
					{
						new StoragePropertyGroup
						{
							Title = "Server",
							Values = new List<StorageProperty>
							{
								new StorageProperty("Id", data["Id"]),
								new StorageProperty("Name", data["Name"]),
								new StorageProperty("Heartbeat", data["Heartbeat"]?.ToFormattedDateTime())
							}.Where(t => t != null && !string.IsNullOrEmpty(t.Value))
						}
					}
				};
			});

			return server;
		}

		public StorageItem GetRecurringTask(string id)
		{
			var recurringTask = _store.Storage(s =>
			{
				var data = s.Get<DataObject>(new StorageKey($"tasks:recurring:{id}"));
				if (data == null)
				{
					return null;
				}

				return new StorageItem
				{
					Key = id,
					Title = data["Name"]?.ToString(),
					Groups = new List<StoragePropertyGroup>
					{
						new StoragePropertyGroup
						{
							Title = "Recurring Task",
							Values = new List<StorageProperty>
							{
								new StorageProperty("Name", data["Name"]),
								new StorageProperty("ReferenceId", data["ReferenceId"]),
								new StorageProperty("Next execution", data["NextExecution"]?.ToFormattedDateTime()),
								new StorageProperty("Interval", data["Interval"]?.ToDuration())
							}.Where(t => t != null && !string.IsNullOrEmpty(t.Value))
						}
					}
				};
			});

			return recurringTask;
		}

		public void DeleteTask(string id)
		{
			_store.Delete(id);
		}
	}
}
