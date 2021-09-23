using System.Threading;

namespace Broadcast.EventSourcing
{
	/// <summary>
	/// Context object that is passed to the <see cref="TaskStoreDispatcher"/>
	/// </summary>
	public class StorageDispatcherContext : IStorageContext
	{
		/// <summary>
		/// Gets the instance of the <see cref="DispatcherStorage"/> registered in the <see cref="ITaskStore"/>
		/// </summary>
		public DispatcherStorage Dispatchers { get; set; }

		/// <summary>
		/// Gets the event that is set when a task is dispatched
		/// </summary>
		public ManualResetEventSlim ResetEvent { get; set; }
	}
}
