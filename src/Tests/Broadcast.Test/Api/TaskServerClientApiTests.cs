using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using NUnit.Framework;

namespace Broadcast.Test.Api
{
	public class TaskServerClientApiTests
	{
		[Test]
		public void TaskServerClient_Api_Send_StaticTrace()
		{
			// execute a static method
			// serializeable
			TaskServerClient.Send(() => Trace.WriteLine("test"));
		}

		[Test]
		public void TaskServerClient_Api_Send_Method()
		{
			// execute a local method
			// serializeable
			TaskServerClient.Send(() => TestMethod(1));
		}

		[Test]
		public void TaskServerClient_Api_Send_GenericMethod()
		{
			// execute a generic method
			// serializeable
			TaskServerClient.Send(() => GenericMethod(1));
		}

		[Test]
		public void TaskServerClient_Api_Send_Notification_Class()
		{
			// send a event to a handler
			// serializeable Func<TestClass>
			TaskServerClient.Send<TestClass>(() => new TestClass(1));
		}

		[Test]
		public void TaskServerClient_Api_Send_Notification_Method()
		{
			// send a event to a handler
			// serializeable Func<TestClass>
			TaskServerClient.Send<TestClass>(() => Returnable(1));
		}











		[Test]
		public void TaskServerClient_Api_Schedule_StaticTrace()
		{
			// execute a static method
			// serializeable
			TaskServerClient.Schedule(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(1));
		}

		[Test]
		public void TaskServerClient_Api_Schedule_Method()
		{
			// execute a local method
			// serializeable
			TaskServerClient.Schedule(() => TestMethod(1), TimeSpan.FromSeconds(1));
		}

		[Test]
		public void TaskServerClient_Api_Schedule_GenericMethod()
		{
			// execute a generic method
			// serializeable
			TaskServerClient.Schedule(() => GenericMethod(1), TimeSpan.FromSeconds(1));
		}

		[Test]
		public void TaskServerClient_Api_Schedule_Notification_Class()
		{
			// send a event to a handler
			// Nonserializeable Func<TestClass>
			TaskServerClient.Schedule<TestClass>(() => new TestClass(1), TimeSpan.FromSeconds(1));
		}

		[Test]
		public void TaskServerClient_Api_Schedule_Notification_Method()
		{
			// send a event to a handler
			// Nonserializeable Func<TestClass>
			TaskServerClient.Schedule<TestClass>(() => Returnable(1), TimeSpan.FromSeconds(1));
		}




		[Test]
		public void TaskServerClient_Api_Recurring_StaticTrace()
		{
			// execute a static method
			// serializeable
			TaskServerClient.Recurring(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(1));
		}

		[Test]
		public void TaskServerClient_Api_Recurring_Method()
		{
			// execute a local method
			// serializeable
			TaskServerClient.Recurring(() => TestMethod(1), TimeSpan.FromSeconds(1));
		}

		[Test]
		public void TaskServerClient_Api_Recurring_GenericMethod()
		{
			// execute a generic method
			// serializeable
			TaskServerClient.Recurring(() => GenericMethod(1), TimeSpan.FromSeconds(1));
		}

		[Test]
		public void TaskServerClient_Api_Recurring_Notification_Class()
		{
			// send a event to a handler
			// Nonserializeable Func<TestClass>
			TaskServerClient.Recurring<TestClass>(() => new TestClass(1), TimeSpan.FromSeconds(1));
		}

		[Test]
		public void TaskServerClient_Api_Recurring_Notification_Method()
		{
			// send a event to a handler
			// Nonserializeable Func<TestClass>
			TaskServerClient.Recurring<TestClass>(() => Returnable(1), TimeSpan.FromSeconds(1));
		}










		public void TestMethod(int i) { }

		public void GenericMethod<T>(T value) { }

		public TestClass Returnable(int i) => new TestClass(i);

		public class TestClass : INotification
		{
			public TestClass(int i) { }
		}
	}
}
