using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace Broadcast.Dashboard
{
	/// <summary>
	/// <<see cref="IDashboardDispatcher"/> for the dashboard
	/// </summary>
	public class AspNetCoreDashboardContext : IDashboardContext
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="httpContext"></param>
		/// <param name="store"></param>
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
		/// Gets the <see cref="Match"/>
		/// </summary>
		public Match UriMatch { get; set; }

		/// <summary>
		/// Gets the <see cref="DashboardResponse"/>
		/// </summary>
		public DashboardResponse Response { get; }
	}
}
