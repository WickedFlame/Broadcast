
namespace Broadcast.Server
{
	/// <summary>
	/// BackgroundServerProcess is used to create dispatchers that run in backgroundthreads
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IBackgroundServerProcess<out T> where T : class, IServerContext
	{
		/// <summary>
		/// Start the <see cref="IBackgroundDispatcher{T}"/> in a new thread and run the Execute method
		/// </summary>
		/// <param name="dispatcher"></param>
		void StartNew(IBackgroundDispatcher<T> dispatcher);

		/// <summary>
		/// Wait for all threads in the serverprocess to end
		/// </summary>
		void WaitAll();
	}
}
