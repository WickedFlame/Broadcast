using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Broadcast.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Broadcast.Dashboard.Dispatchers
{
	public class DashboardTaskDataDispatcher : IDashboardDispatcher
	{
		public async Task Dispatch(IDashboardContext context)
		{
			var id = context.UriMatch.Groups["id"];


			var task = context.TaskStore.Storage(s =>
			{
				var values = s.Get<DataObject>(new StorageKey($"task:{id}"))
					.Select(d => new StorageProperty(d.Key, d.Value))
					.ToList();

				var data = s.Get<DataObject>(new StorageKey($"tasks:values:{id.Value}"));
				if(data!= null)
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
							values.FirstOrDefault(v => v.Key == "State"),
							values.FirstOrDefault(v => v.Key == "Server"),
							new StorageProperty("Executed", values.FirstOrDefault(v => v.Key == "ExecutedAt")?.Value.ToFormattedDateTime()),
							new StorageProperty("Duration", values.FirstOrDefault(v => v.Key == "ExecutionTime")?.Value.ToDuration())
						}.Where(t => t != null && !string.IsNullOrEmpty(t.Value))
					},
					new StoragePropertyGroup
					{
						Title = "Task Execution",
						Values = new List<StorageProperty>
						{
							values.FirstOrDefault(v => v.Key == "Type"),
							values.FirstOrDefault(v => v.Key == "Method")
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

			var settings = new JsonSerializerSettings
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver(),
				Converters = new JsonConverter[] { new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() } }
			};
			var serialized = JsonConvert.SerializeObject(task, settings);

			context.Response.ContentType = "application/json";
			await context.Response.WriteAsync(serialized);
		}
	}

	public class StorageItem
	{
		public string Key { get; set; }

		public string Title { get; set; }

		public IEnumerable<StorageProperty> Values { get; set; }

		public IEnumerable<StoragePropertyGroup> Groups { get; set; }
	}

	public class StoragePropertyGroup
	{
		public string Title { get; set; }

		public IEnumerable<StorageProperty> Values { get; set; }
	}

	public class StorageProperty
	{
		public StorageProperty(string key, object value)
			: this(key, value?.ToString())
		{
		}

		public StorageProperty(string key, string value)
		{
			Key = key;
			Value = value;
		}

		public string Key { get; set; }

		public string Value { get; set; }
	}

	public static class ObjectExtensions
	{
		public static string ToDuration(this object value)
		{
			if (long.TryParse(value?.ToString(), out var duration))
			{
				return $"{TimeSpan.FromTicks(duration).TotalMilliseconds} ms";
			}

			return null;
		}

		public static string ToFormattedDateTime(this object value)
		{
			if (DateTime.TryParse(value?.ToString(), out var date))
			{
				return date.ToString("yyyy/MM/dd hh:mm:ss.fff");
			}

			return null;
		}
	}
}
