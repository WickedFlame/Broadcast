using System.Threading;

namespace Broadcast.Server
{
	/// <summary>
	/// Context object for the <see cref="IBackgroundServerProcess{T}"/> of the <see cref="IBroadcaster"/>
	/// </summary>
	public interface IBroadcasterConterxt : IServerContext
	{
		/// <summary>
		/// Gets the <see cref="ThreadWait"/> that stops the broadcaster
		/// </summary>
		ThreadWait ThreadWait { get; }
		
		/// <summary>
		/// Gets the Id of the <see cref="IBroadcaster"/>
		/// </summary>
		string Id { get; }
	}
}
