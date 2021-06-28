using System;
using Microsoft.AspNetCore.Http;

namespace Broadcast.Dashboard
{
	/// <summary>
	/// Default options for the dashboard. This is configured during startup.
	/// </summary>
	public class DashboardOptions
	{
		private DashboardOptions(){}

		/// <summary>
		/// Singleton of the <see cref="DashboardOptions"/>
		/// </summary>
		public static DashboardOptions Default { get; } = new DashboardOptions
		{
			RouteTemplate = "broadcast/",
			RouteBasePath = "~/broadcast",
			AuthorizeRequest = request => true
		};

		/// <summary>
		/// Gets the template path
		/// </summary>
		public string RouteTemplate { get; set; }

		/// <summary>
		/// Gets the base path of the dashboard route
		/// </summary>
		public string RouteBasePath { get; set; }

		/// <summary>
		/// Gets the check if the request is authorized
		/// </summary>
		public Func<HttpRequest, bool> AuthorizeRequest { get; set; }
	}
}
