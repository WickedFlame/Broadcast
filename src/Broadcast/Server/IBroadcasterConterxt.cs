
namespace Broadcast.Server
{
	/// <summary>
	/// Context object for the <see cref="IBackgroundServerProcess{T}"/> of the <see cref="IBroadcaster"/>
	/// </summary>
	public interface IBroadcasterConterxt : IServerContext
	{
		/// <summary>
		/// Gets a boolean indicating if the <see cref="IBroadcaster"/> is running
		/// </summary>
		bool IsRunning { get; }

		/// <summary>
		/// Gets the Id of the <see cref="IBroadcaster"/>
		/// </summary>
		string Id { get; }
	}
}
