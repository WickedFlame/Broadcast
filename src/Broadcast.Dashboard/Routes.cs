using System.Reflection;
using Broadcast.Dashboard.Dispatchers;

namespace Broadcast.Dashboard
{
	/// <summary>
	/// Collection of all routes for the Dashboard with all associated <see cref="IDashboardDispatcher"/>
	/// </summary>
	public static class Routes
	{
		static Routes()
		{
			RouteCollection = new RouteCollection();

			RouteCollection.Add("/dashboard/metrics", new ConsoleMetricsDispatcher());
			RouteCollection.Add("/dashboard/data/task/(?<id>.+)", new DashboardTaskDataDispatcher());
			//RouteCollection.Add("/dashboard/data/task", new DashboardTaskDataDispatcher());

			RouteCollection.Add("/js/broadcast-base", new EmbeddedResourceDispatcher("application/javascript", GetExecutingAssembly(), GetContentResourceName("js", "broadcast-base.js")));
			RouteCollection.Add("/js/broadcast-console", new EmbeddedResourceDispatcher("application/javascript", GetExecutingAssembly(), GetContentResourceName("js", "broadcast-console.js")));

			//RouteCollection.Add("/js/broadcast-base.js", new EmbeddedResourceDispatcher("application/javascript", GetExecutingAssembly(), GetContentResourceName("js", "broadcast-base.js")));
			//RouteCollection.Add("/js/broadcast-console.js", new EmbeddedResourceDispatcher("application/javascript", GetExecutingAssembly(), GetContentResourceName("js", "broadcast-console.js")));

			RouteCollection.Add("/css/broadcast-console", new EmbeddedResourceDispatcher("text/css", GetExecutingAssembly(), GetContentResourceName("css", "broadcast-console.min.css")));
		}

		/// <summary>
		/// The collection of routes
		/// </summary>
		public static RouteCollection RouteCollection { get; }

		private static Assembly GetExecutingAssembly()
		{
			return typeof(Routes).GetTypeInfo().Assembly;
		}

		internal static string GetContentResourceName(string contentFolder, string resourceName)
		{
			return $"{typeof(Routes).Namespace}.Assets.{contentFolder}.{resourceName}";
		}
	}
}
