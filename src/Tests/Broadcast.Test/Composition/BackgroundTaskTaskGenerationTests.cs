using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.EventSourcing;
using NUnit.Framework;

namespace Broadcast.Test.Composition
{
	public class BackgroundTaskTaskGenerationTests
	{
		[Test]
		public void BackgroundTask_TaskGeneration_Recurring_Id()
		{
			BroadcastServer.Setup(s => { });

			// execute a static method
			// serializeable
			Assert.IsNotEmpty(BackgroundTask.Recurring(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.5)));
		}

		[Test]
		public void BackgroundTask_TaskGeneration_Schedule_Id()
		{
			BroadcastServer.Setup(s => { });

			// execute a static method
			// serializeable
			Assert.IsNotEmpty(BackgroundTask.Schedule(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.5)));
		}

		[Test]
		public void BackgroundTask_TaskGeneration_Send_Id()
		{
			BroadcastServer.Setup(s => { });

			// execute a static method
			// serializeable
			Assert.IsNotEmpty(BackgroundTask.Send(() => Trace.WriteLine("test")));
		}
	}
}
