using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Composition;
using Broadcast.EventSourcing;
using NUnit.Framework;

namespace Broadcast.Test.Composition
{
	public class TaskFactoryTests
	{
		[Test]
		public void TaskFactory_CreateTask_Args_VerifyGenericMethod()
		{
			var task = TaskFactory.CreateTask(() => GenericMethod<int>(5)) as ExpressionTask;
			Assert.AreEqual(task.Args[0], 5);
		}


		public void GenericMethod<T>(T input)
		{
			Console.WriteLine(input);
		}

		public class GenericClass<T>
		{

		}
	}
}
