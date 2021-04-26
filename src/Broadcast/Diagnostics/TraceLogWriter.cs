using System.Diagnostics;

namespace Broadcast.Diagnostics
{
	/// <summary>
	/// Writes <see cref="LogMessage"/> to a Trace output
	/// </summary>
	public class TraceLogWriter : ILogWriter<LogMessage>
	{
		/// <summary>
		/// Gets the <see cref="Category"/>
		/// </summary>
		public Category Category => Category.Log;

		/// <summary>
		/// Write a <see cref="ILogEvent"/> to a Trace output
		/// </summary>
		/// <param name="event"></param>
		public void Write(ILogEvent @event)
		{
			if (@event is LogMessage e)
			{
				Write(e);
			}
			else
			{
				Trace.WriteLine($"[{@event.Timestamp}] {@event.Message}");
			}
		}

		/// <summary>
		/// Write a <see cref="LogMessage"/> to a Trace output
		/// </summary>
		/// <param name="event"></param>
		public void Write(LogMessage @event)
		{
			Trace.WriteLine($"[{@event.Timestamp}] [{@event.Level}] {@event.Message}");
		}
	}
}
