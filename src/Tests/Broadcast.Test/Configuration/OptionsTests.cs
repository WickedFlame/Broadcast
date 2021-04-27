using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Configuration;
using NUnit.Framework;

namespace Broadcast.Test.Configuration
{
	public class OptionsTests
	{
		[Test]
		public void Options_ctor()
		{
			Assert.DoesNotThrow(() => new Options());
		}

		[Test]
		public void Options_Default()
		{
			Assert.IsNotNull(Options.Default);
		}

		[Test]
		public void Options_Property_ServerName_Default()
		{
			Assert.AreEqual(Environment.MachineName, new Options().ServerName);
		}

		[Test]
		public void Options_Property_HeartbeatInterval_Default()
		{
			Assert.AreEqual(60000, new Options().HeartbeatInterval);
		}

		[Test]
		public void Options_Setup()
		{
			Options.Setup(o =>
			{
				o.ServerName = "test";
				o.HeartbeatInterval = 1;
			});

			Assert.AreEqual("test", Options.Default.ServerName);
			Assert.AreEqual(1, Options.Default.HeartbeatInterval);
		}

		[Test]
		public void Options_Setup_Reset()
		{
			Options.Setup(o =>
			{
				o.ServerName = "test";
				o.HeartbeatInterval = 1;
			});

			Options.Setup(o => { });

			Assert.AreEqual(Environment.MachineName, Options.Default.ServerName);
			Assert.AreEqual(60000, Options.Default.HeartbeatInterval);
		}
	}
}
