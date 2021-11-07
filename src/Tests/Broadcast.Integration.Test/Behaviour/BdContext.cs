using System;

namespace Broadcast.Integration.Test.Behaviour
{
	public class BdContext
	{
		public ITaskStore Store { get; set; }

		public Broadcaster Server { get; set; }

		public void Wait()
		{
			System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(5)).Wait();
		}
	}
}
