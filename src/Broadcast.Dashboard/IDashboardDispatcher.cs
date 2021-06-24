using System.Threading.Tasks;

namespace Broadcast.Dashboard
{
	/// <summary>
	/// Interface for Dispatchers used for the dashboard
	/// </summary>
	public interface IDashboardDispatcher
	{
		/// <summary>
		/// Execute the dispatcher
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		Task Dispatch(IDashboardContext context);
	}
}
