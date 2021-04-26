
using System;

namespace Broadcast.Diagnostics
{
	/// <summary>
	/// Extension methods for the logger
	/// </summary>
	public static class LoggerExtensions
	{
		public static void Write(this ILogger logger, string message, Exception e)
		{
			logger.Write($"{message}{Environment.NewLine}{e.Message}{Environment.NewLine}{e.StackTrace}", LogLevel.Error);
		}

		/// <summary>
		/// Write a message to the log
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="message"></param>
		/// <param name="category"></param>
		/// <param name="level"></param>
		public static void Write(this ILogger logger, string message, LogLevel level = LogLevel.Info, Category category = Category.Log)
		{
			switch (category)
			{
				case Category.Log:
					logger.Write(new LogMessage(message, level), category);
					break;
			}
		}

		///// <summary>
		///// Writes a message that can be collected by the dashboard
		///// </summary>
		///// <param name="logger"></param>
		///// <param name="message"></param>
		///// <param name="metric"></param>
		//public static void WriteMetric<T>(this ILogger logger, T message, StatisticType metric)
		//{
		//	logger.Write(new StatisticEvent<T>(message, metric), Category.EventStatistic);
		//}
	}
}
