using System;
using Microsoft.AspNetCore.Http;

namespace Broadcast.Dashboard
{
	public class DashboardOptions
	{
		private DashboardOptions(){}

		public static DashboardOptions Default { get; } = new DashboardOptions
		{
			RouteTemplate = "broadcast/",
			RouteBasePath = "~/broadcast",
			AuthorizeRequest = request => true
		};

		public string RouteTemplate { get; set; }

		public string RouteBasePath { get; set; }

		public Func<HttpRequest, bool> AuthorizeRequest { get; set; }
	}
}
