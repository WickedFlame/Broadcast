using System.Threading.Tasks;
using Broadcast.Monitoring;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Broadcast.Dashboard.Dispatchers
{
	/// <summary>
	/// Dispatcher for metrics used in the console
	/// </summary>
	public class ConsoleMetricsDispatcher : IDashboardDispatcher
	{
		/// <summary>
		/// Execute the dispatcher
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public async Task Dispatch(IDashboardContext context)
		{
			var monitoring = new MonitoringService(context.TaskStore);

			var monitor = new
			{
				Servers = monitoring.GetServers(),
				Tasks = monitoring.GetAllTasks(),
				RecurringTasks = monitoring.GetRecurringTasks()
			};

			var settings = new JsonSerializerSettings
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver(),
				Converters = new JsonConverter[] { new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() } }
			};
			var serialized = JsonConvert.SerializeObject(monitor, settings);

			context.Response.ContentType = "application/json";
			await context.Response.WriteAsync(serialized);
		}
	}
}
