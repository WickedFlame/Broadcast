using System.Threading;

namespace Broadcast.Server
{
	/// <summary>
	/// Context object for the <see cref="IBackgroundServerProcess{T}"/> of the <see cref="IBroadcaster"/>
	/// </summary>
	public class BroadcasterConterxt : IBroadcasterConterxt
	{
		/// <summary>
		/// Create a new context
		/// </summary>
        public BroadcasterConterxt()
        {
            ThreadWait = new ThreadWait();
        }

		/// <summary>
		/// Gets the <see cref="ThreadWait"/> that stops the broadcaster
		/// </summary>
		public ThreadWait ThreadWait { get; }

		/// <summary>
		/// Gets the Id of the <see cref="IBroadcaster"/>
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// End the processing thread
		/// </summary>
        public void Stop()
        {
            ThreadWait.Close();
        }
    }
}
