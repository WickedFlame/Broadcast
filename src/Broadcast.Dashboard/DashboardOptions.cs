using System;
using System.Collections.Generic;
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
			//RouteTemplate = "broadcast/",
			RouteBasePath = "/broadcast",
			AuthorizeRequest = request => true,

			TemplateParameters = new Dictionary<string, string>
			{
				{"%(RouteBasePath)", "/broadcast"}
			}
		};

		/// <summary>
		/// Gets the base path of the dashboard route
		/// </summary>
		public string RouteBasePath { get; set; }

		/// <summary>
		/// Parameters that are injected or replaced in the templates
		/// </summary>
		public IDictionary<string, string> TemplateParameters = new Dictionary<string, string>();

		/// <summary>
		/// Gets the check if the request is authorized
		/// </summary>
		public Func<HttpRequest, bool> AuthorizeRequest { get; set; }
	}
}
