﻿using System;

namespace Broadcast.Server
{
	/// <summary>
	/// Heartbeat that is published by the <see cref="BroadcasterHeartbeatDispatcher"/>
	/// </summary>
	public class ServerModel
	{
		/// <summary>
		/// Gets or sets the ServerName
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the Heartbeat timestamp
		/// </summary>
		public DateTime Heartbeat { get; set; }
	}
}
