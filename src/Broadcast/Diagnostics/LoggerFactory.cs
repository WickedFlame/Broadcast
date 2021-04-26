using System;
using System.Collections.Generic;

namespace Broadcast.Diagnostics
{
	/// <summary>
	/// Factory to create fully setup instances of <see cref="ILogger"/>
	/// </summary>
	public class LoggerFactory
	{
		private static List<ILogWriter> _defaultWriters = new List<ILogWriter>
		{
			new TraceLogWriter()
		};

		private static Func<ILogger> _customLoggerFactory;

		/// <summary>
		/// Allows the creation of a custom <see cref="ILogger"/>.
		/// Assign null to reset to the default behaviour.
		/// </summary>
		/// <param name="setup"></param>
		public static void Setup(Func<ILogger> setup)
		{
			_customLoggerFactory = setup;
		}

		/// <summary>
		/// Create a new instance of a <see cref="ILogger"/> that contains all default <see cref="ILogWriter"/>.
		/// Current default writers contains <see cref="TraceLogWriter"/>
		/// </summary>
		/// <returns></returns>
		public static ILogger Create()
		{
			if (_customLoggerFactory != null)
			{
				return _customLoggerFactory();
			}

			var logger = new Logger();

			foreach (var writer in _defaultWriters)
			{
				logger.Writers.Add(writer);
			}

			return logger;
		}

		/// <summary>
		/// Add a <see cref="ILogWriter"/> to the default list of <see cref="ILogWriter"/>
		/// </summary>
		/// <param name="writer"></param>
		public static void AddLogWriter(ILogWriter writer)
		{
			_defaultWriters.Add(writer);
		}
	}
}
