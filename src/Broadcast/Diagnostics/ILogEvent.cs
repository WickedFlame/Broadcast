using System;

namespace Broadcast.Diagnostics
{
	/// <summary>
	/// 
	/// </summary>
	public interface ILogEvent
	{
		/// <summary>
		/// Gets or sets the Message
		/// </summary>
		string Message { get; }

		/// <summary>
		/// Gets or sets the Timestamp
		/// </summary>
		DateTime Timestamp { get; }
	}
}
