
namespace Broadcast.Server
{
	public interface IBackgroundServerProcess<T> where T : IServerContext
	{
		void StartNew(IBackgroundDispatcher<T> dispatcher);
	}
}
