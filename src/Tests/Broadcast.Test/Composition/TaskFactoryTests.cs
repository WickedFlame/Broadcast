using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
			Expression<Action> expr = () => GenericMethod<int>(5);
			var task = TaskFactory.CreateTask(expr) as ExpressionTask;
			Assert.AreEqual(task.Args[0], 5);
		}

		[Test]
		public void TaskFactory_CreateTask_ExpressionAction()
		{
			Expression<Action> expr = () => GenericMethod<int>(5);
			Assert.IsAssignableFrom<ExpressionTask>(TaskFactory.CreateTask(expr));
		}

		[Test]
		public void TaskFactory_CreateTask_Action()
		{
			Action expr = () => GenericMethod<int>(5);
			Assert.IsAssignableFrom<ActionTask>(TaskFactory.CreateTask(expr));
		}

		[Test]
		public void TaskFactory_CreateTask_Func()
		{
			Assert.IsAssignableFrom<DelegateTask<Model>>(TaskFactory.CreateTask(() => new Model()));
		}

		[Test]
		public void TaskFactory_CreateTask_ExpressionFunc()
		{
			Assert.IsAssignableFrom<ExpressionTask<Model>>(TaskFactory.CreateNotifiableTask<Model>(() => new Model()));
		}


		public void GenericMethod<T>(T input)
		{
			Console.WriteLine(input);
		}

		public class Model : INotification
		{

		}
	}
}
