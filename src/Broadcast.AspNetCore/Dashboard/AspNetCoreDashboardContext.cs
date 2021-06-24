using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace Broadcast.Dashboard
{
	public class AspNetCoreDashboardContext : IDashboardContext
	{
		public AspNetCoreDashboardContext(HttpContext httpContext, ITaskStore store)
		{
			TaskStore = store;
			Response = new DashboardResponse(httpContext);
		}

		/// <summary>
		/// Gets the <see cref="ITaskStore"/>
		/// </summary>
		public ITaskStore TaskStore { get; }

		/// <summary>
		/// Gets the <see cref="Math"/>
		/// </summary>
		public Match UriMatch { get; set; }

		/// <summary>
		/// Gets the <see cref="DashboardResponse"/>
		/// </summary>
		public DashboardResponse Response { get; }
	}
}
