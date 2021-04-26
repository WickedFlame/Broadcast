
namespace Broadcast.Diagnostics
{
	/// <summary>
	/// Interface for a LogMessage
	/// </summary>
	public interface ILogMessage : ILogEvent
	{
		/// <summary>
		/// Gets or sets the <see cref="LogLevel"/>
		/// </summary>
		LogLevel Level { get; }
	}
}
