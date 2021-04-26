
namespace Broadcast.Diagnostics
{
	/// <summary>
	/// Writes logs to a designated output
	/// </summary>
	public interface ILogWriter
	{
		/// <summary>
		/// Gets the <see cref="Category"/>
		/// </summary>
		Category Category { get; }

		/// <summary>
		/// Write a <see cref="ILogEvent"/> to a log destination
		/// </summary>
		/// <param name="event"></param>
		void Write(ILogEvent @event);
	}

	/// <summary>
	/// Writes logs to a designated output
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface ILogWriter<in T> : ILogWriter where T : ILogEvent
	{
		/// <summary>
		/// Write a derivate of <see cref="ILogEvent"/> to a log destination
		/// </summary>
		/// <param name="event"></param>
		void Write(T @event);
	}
}
