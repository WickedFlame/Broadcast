using System;
using System.Text.RegularExpressions;

namespace Broadcast.Dashboard
{
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
		/// Gets the <see cref="DashboardResponse"/>
		/// </summary>
		DashboardResponse Response { get; }
	}
}
