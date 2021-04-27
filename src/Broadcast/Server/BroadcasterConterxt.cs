
namespace Broadcast.Server
{
	/// <summary>
	/// Context object for the <see cref="IBackgroundServerProcess{T}"/> of the <see cref="IBroadcaster"/>
	/// </summary>
	public class BroadcasterConterxt : IBroadcasterConterxt
	{
		/// <summary>
		/// Gets a boolean indicating if the <see cref="IBroadcaster"/> is running
		/// </summary>
		public bool IsRunning { get; set; }

		/// <summary>
		/// Gets the Id of the <see cref="IBroadcaster"/>
		/// </summary>
		public string Id { get; set; }
	}
}
