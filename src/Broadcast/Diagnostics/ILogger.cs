
namespace Broadcast.Diagnostics
{
	/// <summary>
	/// The logger that delegates logmessages and events to the registered <see cref="ILogWriter"/>
	/// </summary>
	public interface ILogger
	{
		/// <summary>
		/// Send a event to all registered <see cref="ILogWriter"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="event"></param>
		/// <param name="category"></param>
		void Write<T>(T @event, Category category) where T : ILogEvent;
	}
}
