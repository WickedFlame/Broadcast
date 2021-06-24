using System;
using System.Collections.Generic;
using System.Text;

namespace Broadcast.Monitoring
{
	public class ServerDescription
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public DateTime Heartbeat { get; set; }
	}
}
