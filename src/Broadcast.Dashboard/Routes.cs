using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Broadcast.Dashboard.Dispatchers;

namespace Broadcast.Dashboard
{
	public class Routes
	{
		static Routes()
		{
			RouteCollection = new RouteCollection();

			RouteCollection.Add("/dashboard/metrics", new ConsoleMetricsDispatcher());

			//RouteCollection.Add("/js[0-9]+", new EmbeddedResourceDispatcher(
			//RouteCollection.Add("/js/dashboard-console/js", new EmbeddedResourceDispatcher(
			RouteCollection.Add("/js/broadcast-base.js", new EmbeddedResourceDispatcher(
				"application/javascript",
				GetExecutingAssembly(),
				GetContentResourceName("js", "broadcast-base.js")));

			RouteCollection.Add("/js/broadcast-console.js", new EmbeddedResourceDispatcher(
				"application/javascript",
				GetExecutingAssembly(),
				GetContentResourceName("js", "broadcast-console.js")));

			RouteCollection.Add("/css/broadcast-console.min.css", new EmbeddedResourceDispatcher(
				"text/css",
				GetExecutingAssembly(),
				GetContentResourceName("css", "broadcast-console.min.css")));
		}

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
