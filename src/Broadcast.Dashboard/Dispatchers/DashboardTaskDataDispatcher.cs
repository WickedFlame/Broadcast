﻿using System.Threading.Tasks;
using Broadcast.Dashboard.Dispatchers.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Broadcast.Dashboard.Dispatchers
{
	/// <summary>
	/// Dispatcher for the data of tasks
	/// </summary>
	public class DashboardTaskDataDispatcher : IDashboardDispatcher
	{
		/// <summary>
		/// Execution of the dispatcher
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public async Task Dispatch(IDashboardContext context)
		{
			var id = context.UriMatch.Groups["id"];

			var service = new StorageItemService(context.TaskStore);
			var task = service.GetTask(id.Value);

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
}
