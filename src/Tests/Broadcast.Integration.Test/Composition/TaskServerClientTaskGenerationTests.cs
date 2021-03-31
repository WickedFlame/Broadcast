using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.EventSourcing;
using NUnit.Framework;

namespace Broadcast.Integration.Test.Composition
{
	[SingleThreaded]
	//[Explicit]
	[Category("Integration")]
	public class TaskServerClientTaskGenerationTests
	{
		[Test]
		public void TaskServerClient_TaskGeneration_Send()
		{
			Broadcaster.Setup(s => { });

			// execute a static method
			// serializeable
			TaskServerClient.Send(() => Trace.WriteLine("test"));

			Assert.IsAssignableFrom<ExpressionTask>(Broadcaster.Server.GetStore().Single());
		}

		[Test]
		public void TaskServerClient_TaskGeneration_Schedule()
		{
			Broadcaster.Setup(s => { });

			// execute a static method
			// serializeable
			TaskServerClient.Schedule(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.5));

			//Thread.Sleep(TimeSpan.FromSeconds(1));
			Task.Delay(1000).Wait();

			Assert.IsAssignableFrom<ExpressionTask>(Broadcaster.Server.GetStore().Single());
		}

		[Test]
		public void TaskServerClient_TaskGeneration_Recurring()
		{
			Broadcaster.Setup(s => { });

			// execute a static method
			// serializeable
			TaskServerClient.Recurring(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.5));

			//Thread.Sleep(TimeSpan.FromSeconds(1));
			Task.Delay(1000).Wait();

			Assert.IsAssignableFrom<ExpressionTask>(Broadcaster.Server.GetStore().First());
		}
	}
}
