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

				var item = new StorageItem
				{
					Key = id.ToString(),
					Title = values.FirstOrDefault(v => v.Key == "Name")?.Value,
					Values = values
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

		public IEnumerable<StorageProperty> Values { get; set; }
		public string Title { get; set; }
	}

	public class StorageProperty
	{
		public StorageProperty(string key, object value)
			: this(key, value.ToString())
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
}
