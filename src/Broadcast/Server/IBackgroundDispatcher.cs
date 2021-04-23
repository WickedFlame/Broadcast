
namespace Broadcast.Server
{
	/// <summary>
	/// Dispatcher that runs in a own thread each
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IBackgroundDispatcher<in T> where T : IServerContext
	{
		/// <summary>
		/// Execute the dispatcher
		/// </summary>
		/// <param name="context"></param>
		void Execute(T context);
	}
}
