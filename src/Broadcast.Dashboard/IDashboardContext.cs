using System;
using System.Text.RegularExpressions;

namespace Broadcast.Dashboard
{
	/// <summary>
	/// Context for creating the dashboard
	/// </summary>
	public interface IDashboardContext
	{
		/// <summary>
		/// Gets the <see cref="ITaskStore"/>
		/// </summary>
		ITaskStore TaskStore { get; }

		/// <summary>
		/// Gets the <see cref="Math"/>
		/// </summary>
		Match UriMatch { get; set; }

		/// <summary>
		/// Gets the <see cref="IDashboardResponse"/>
		/// </summary>
		IDashboardResponse Response { get; }
	}
}
