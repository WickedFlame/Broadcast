using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;
using Broadcast.Composition;
using NUnit.Framework;

namespace Broadcast.Test
{
	public class ApiTests
	{
		//[Test]
		//public void Api_Send()
		//{
		//	var broadcaster = new Broadcaster();

		//	// execute a static method
		//	// serializeable
		//	broadcaster.Send(() => Trace.WriteLine("test"));
		//	TaskServerClient.Send(() => Trace.WriteLine("test"));
		//	BackgroundTaskClient.Send(() => Trace.WriteLine("test"));

		//	// execute a local method
		//	// serializeable
		//	broadcaster.Send(() => TestMethod(1));
		//	TaskServerClient.Send(() => TestMethod(1));
		//	BackgroundTaskClient.Send(() => TestMethod(1));

		//	// execute a generic method
		//	// serializeable
		//	broadcaster.Send(() => GenericMethod(1));
		//	TaskServerClient.Send(() => GenericMethod(1));
		//	BackgroundTaskClient.Send(() => GenericMethod(1));

		//	// send a event to a handler
		//	// Nonserializeable Func<TestClass>
		//	broadcaster.Send<TestClass>(() => new TestClass(1));
		//	BackgroundTaskClient.Send<TestClass>(() => new TestClass(1));
		//	BackgroundTaskClient.Send<TestClass>(() => Returnable(1));

		//	// send a event to a handler
		//	// serializeable Func<TestClass>
		//	TaskServerClient.Send<TestClass>(() => new TestClass(1));
		//	TaskServerClient.Send<TestClass>(() => Returnable(1));

		//	// send a local action
		//	// Nonserializeable
		//	//broadcaster.Send(() =>
		//	//{
		//	//	Trace.WriteLine("test");
		//	//});
		//	BackgroundTaskClient.Send(() =>
		//	{
		//		Trace.WriteLine("test");
		//	});
		//}

		//[Test]
		//public void Api_Schedule()
		//{
		//	var broadcaster = new Broadcaster();
			
		//	// execute a static method
		//	// serializeable
		//	broadcaster.Schedule(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(1));
		//	TaskServerClient.Schedule(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(1));
		//	BackgroundTaskClient.Schedule(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(1));

		//	// execute a local method
		//	// serializeable
		//	broadcaster.Schedule(() => TestMethod(1), TimeSpan.FromSeconds(1));
		//	TaskServerClient.Schedule(() => TestMethod(1), TimeSpan.FromSeconds(1));
		//	BackgroundTaskClient.Schedule(() => TestMethod(1), TimeSpan.FromSeconds(1));

		//	// execute a generic method
		//	// serializeable
		//	broadcaster.Schedule(() => GenericMethod(1), TimeSpan.FromSeconds(1));
		//	TaskServerClient.Schedule(() => GenericMethod(1), TimeSpan.FromSeconds(1));
		//	BackgroundTaskClient.Schedule(() => GenericMethod(1), TimeSpan.FromSeconds(1));

		//	// send a event to a handler
		//	// Nonserializeable Func<TestClass>
		//	broadcaster.Schedule<TestClass>(() => new TestClass(1), TimeSpan.FromSeconds(1));
		//	broadcaster.Schedule<TestClass>(() => Returnable(1), TimeSpan.FromSeconds(1));

		//	// send a event to a handler
		//	// Nonserializeable Func<TestClass>
		//	BackgroundTaskClient.Schedule<TestClass>(() => new TestClass(1), TimeSpan.FromSeconds(1));
		//	BackgroundTaskClient.Schedule<TestClass>(() => Returnable(1), TimeSpan.FromSeconds(1));

		//	// send a event to a handler
		//	// serializeable Func<TestClass>
		//	TaskServerClient.Schedule<TestClass>(() => new TestClass(1), TimeSpan.FromSeconds(1));
		//	TaskServerClient.Schedule<TestClass>(() => Returnable(1), TimeSpan.FromSeconds(1));

		//	// send a local action
		//	// Nonserializeable
		//	//broadcaster.Schedule(() =>
		//	//{
		//	//	Trace.WriteLine("test");
		//	//}, TimeSpan.FromSeconds(1));
		//	BackgroundTaskClient.Schedule(() =>
		//	{
		//		Trace.WriteLine("test");
		//	}, TimeSpan.FromSeconds(1));
		//}

		//[Test]
		//public void Api_Recurring()
		//{
		//	var broadcaster = new Broadcaster();

		//	// execute a static method
		//	// serializeable
		//	broadcaster.Recurring(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(1));
		//	TaskServerClient.Recurring(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(1));
		//	BackgroundTaskClient.Recurring(() => Trace.WriteLine("test"), TimeSpan.FromSeconds(1));

		//	// execute a local method
		//	// serializeable
		//	broadcaster.Recurring(() => TestMethod(1), TimeSpan.FromSeconds(1));
		//	TaskServerClient.Recurring(() => TestMethod(1), TimeSpan.FromSeconds(1));
		//	BackgroundTaskClient.Recurring(() => TestMethod(1), TimeSpan.FromSeconds(1));

		//	// execute a generic method
		//	// serializeable
		//	broadcaster.Recurring(() => GenericMethod(1), TimeSpan.FromSeconds(1));
		//	TaskServerClient.Recurring(() => GenericMethod(1), TimeSpan.FromSeconds(1));
		//	BackgroundTaskClient.Recurring(() => GenericMethod(1), TimeSpan.FromSeconds(1));

		//	// send a event to a handler
		//	// Nonserializeable Func<TestClass>
		//	broadcaster.Recurring<TestClass>(() => new TestClass(1), TimeSpan.FromSeconds(1));
		//	BackgroundTaskClient.Recurring<TestClass>(() => new TestClass(1), TimeSpan.FromSeconds(1));
		//	BackgroundTaskClient.Recurring<TestClass>(() => Returnable(1), TimeSpan.FromSeconds(1));

		//	// send a event to a handler
		//	// serializeable Func<TestClass>
		//	TaskServerClient.Recurring<TestClass>(() => new TestClass(1), TimeSpan.FromSeconds(1));
		//	TaskServerClient.Recurring<TestClass>(() => Returnable(1), TimeSpan.FromSeconds(1));

		//	// send a local action
		//	// Nonserializeable

		//	//broadcaster.Recurring(() =>
		//	//{
		//	//	Trace.WriteLine("test");
		//	//}, TimeSpan.FromSeconds(1));
		//	BackgroundTaskClient.Recurring(() =>
		//	{
		//		Trace.WriteLine("test");
		//	}, TimeSpan.FromSeconds(1));
		//}

		public void TestMethod(int i) { }

		public void GenericMethod<T>(T value){}

		public TestClass Returnable(int i) => new TestClass(i);

		public class TestClass : INotification
		{
			public TestClass(int i){}
		}

		
	}

	public class TaskServerClient
	{
		public static void Recurring(Expression<Action> task, TimeSpan fromSeconds)
		{
			throw new NotImplementedException();
		}

		public static void Schedule(Expression<Action> task, TimeSpan fromSeconds)
		{
			throw new NotImplementedException();
		}

		public static void Send(Expression<Action> expression)
		{
			var task = TaskFactory.CreateTask(expression);
			Broadcaster.Server.Process(task);
		}




		public static void Recurring<T>(Expression<Func<T>> task, TimeSpan fromSeconds)
		{
			throw new NotImplementedException();
		}

		public static void Schedule<T>(Expression<Func<T>> task, TimeSpan fromSeconds)
		{
			throw new NotImplementedException();
		}

		public static void Send<T>(Expression<Func<T>> expression)
		{
			var task = TaskFactory.CreateTask(expression);
			Broadcaster.Server.Process(task);
		}
	}

	public class BackgroundTaskClient
	{
		// local jobs need a local server running

		public static void Recurring(Action action, TimeSpan timeSpan)
		{
			throw new NotImplementedException();
		}

		public static void Schedule(Action task, TimeSpan fromSeconds)
		{
			throw new NotImplementedException();
		}

		public static void Send(Action expression)
		{
			var task = TaskFactory.CreateTask(expression);
			Broadcaster.Server.Process(task);
		}



		public static void Recurring<T>(Func<T> task, TimeSpan fromSeconds)
		{
			throw new NotImplementedException();
		}

		public static void Schedule<T>(Func<T> task, TimeSpan fromSeconds)
		{
			throw new NotImplementedException();
		}

		public static void Send<T>(Func<T> expression)
		{
			var task = TaskFactory.CreateTask(expression);
			Broadcaster.Server.Process(task);
		}
	}
}
