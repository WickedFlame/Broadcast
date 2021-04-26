using System;

namespace Broadcast.Diagnostics
{
	/// <summary>
	/// 
	/// </summary>
	public class LogMessage : ILogMessage
	{
		/// <summary>
		/// Creates a new instance of a LogMessage
		/// </summary>
		/// <param name="message"></param>
		/// <param name="level"></param>
		public LogMessage(string message, LogLevel level)
		{
			Timestamp = DateTime.Now;
			Message = message;
			Level = level;
		}

		/// <summary>
		/// Gets the message
		/// </summary>
		public string Message { get; }

		/// <summary>
		/// Gets the timestamp of the log
		/// </summary>
		public DateTime Timestamp { get; }

		/// <summary>
		/// Gets the <see cref="LogLevel"/> of the log
		/// </summary>
		public LogLevel Level { get; }
	}
}
