using Broadcast.Server;
using System.Threading;

namespace Broadcast.EventSourcing
{
	/// <summary>
	/// Context object that is passed to the <see cref="TaskStoreDispatcher"/>
	/// </summary>
	public interface IStorageContext : IServerContext
	{
		/// <summary>
		/// Gets the instance of the <see cref="DispatcherStorage"/> registered in the <see cref="ITaskStore"/>
		/// </summary>
		IDispatcherStorage Dispatchers { get; }

		/// <summary>
		/// Gets the event that is set when a task is dispatched
		/// </summary>
		ManualResetEventSlim ResetEvent { get; }
	}
}
