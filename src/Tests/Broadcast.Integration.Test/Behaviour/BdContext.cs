using System;
using System.Collections.Generic;

namespace Broadcast.Integration.Test.Behaviour
{
	public class BdContext
	{
		public Dictionary<string, object> Context = new Dictionary<string, object>();

		public ITaskStore Store { get; set; }

		public Broadcaster Server { get; set; }

		public void Wait()
		{
			System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(5)).Wait();
		}
	}
}
