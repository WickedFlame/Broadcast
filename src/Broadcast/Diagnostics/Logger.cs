using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Broadcast.Diagnostics
{
	/// <summary>
	/// The logger that delegates logmessages and events to the registered <see cref="ILogWriter"/>
	/// </summary>
	public class Logger : ILogger
	{
		/// <summary>
		/// Creates a new instance of the Logger
		/// </summary>
		public Logger()
		{
			Writers = new List<ILogWriter>();
		}

		/// <summary>
		/// Gets a list of all <see cref="ILogWriter"/> that are registered to this logger
		/// </summary>
		public List<ILogWriter> Writers { get; }

		/// <summary>
		/// Send a event to all registered <see cref="ILogWriter"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="event"></param>
		/// <param name="category"></param>
		public void Write<T>(T @event, Category category) where T : ILogEvent
		{
			foreach (var writer in Writers.Where(w => w.Category == category))
			{
				try
				{
					writer.Write(@event);
				}
				catch (Exception e)
				{
					// do nothing...
					Trace.WriteLine(e);
				}
			}
		}
	}
}
