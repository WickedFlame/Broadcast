﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Broadcast.EventSourcing;
using NUnit.Framework;

namespace Broadcast.Test.Composition
{
	public class TaskServerClientTaskGenerationTests
	{
		[Test]
		public void TaskServerClient_TaskGeneration_Recurring_Id()
		{
			Broadcaster.Setup(s => { });

			// execute a static method
			// serializeable
			Assert.IsNotEmpty(TaskServerClient.Recurring(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.5)));
		}

		[Test]
		public void TaskServerClient_TaskGeneration_Schedule_Id()
		{
			Broadcaster.Setup(s => { });

			// execute a static method
			// serializeable
			Assert.IsNotEmpty(TaskServerClient.Schedule(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(0.5)));
		}

		[Test]
		public void TaskServerClient_TaskGeneration_Send_Id()
		{
			Broadcaster.Setup(s => { });

			// execute a static method
			// serializeable
			Assert.IsNotEmpty(TaskServerClient.Send(() => Trace.WriteLine("test")));
		}
	}
}
