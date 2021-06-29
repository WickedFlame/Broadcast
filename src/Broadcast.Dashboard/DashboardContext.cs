using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace Broadcast.Dashboard
{
	/// <summary>
	/// <<see cref="IDashboardDispatcher"/> for the dashboard
	/// </summary>
	public class DashboardContext : IDashboardContext
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="response"></param>
		/// <param name="store"></param>
		public DashboardContext(IDashboardResponse response, ITaskStore store)
		{
			TaskStore = store;
			Response = response;
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
		public IDashboardResponse Response { get; }
	}
}
