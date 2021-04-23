using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Server;
using NUnit.Framework;

namespace Broadcast.Test.Server
{
	public class BackgroundServerProcessTests
	{
		[Test]
		public void BackgroundServerProcess_ctor()
		{
			Assert.DoesNotThrow(() => new BackgroundServerProcess<TestContext>(new TestContext()));
		}

		[Test]
		public void BackgroundServerProcess_ctor_Null_Context()
		{
			Assert.Throws<ArgumentNullException>(() => new BackgroundServerProcess<TestContext>(null));
		}

		[Test]
		public void BackgroundServerProcess_StartNew()
		{
			var ctx = new TestContext();
			var dispatcher = new TestDispatcher(c => c.IsCalled = true);
			var server = new BackgroundServerProcess<TestContext>(ctx);

			server.StartNew(dispatcher);

			server.WaitAll();

			Assert.IsTrue(ctx.IsCalled);
		}

		private class TestContext : IServerContext
		{
			public bool IsCalled{ get; set; }
		}

		private class TestDispatcher : IBackgroundDispatcher<TestContext>
		{
			private Action<TestContext> _action;

			public TestDispatcher(Action<TestContext> action)
			{
				_action = action;
			}

			public void Execute(TestContext context)
			{
				_action(context);
			}
		}
	}
}
