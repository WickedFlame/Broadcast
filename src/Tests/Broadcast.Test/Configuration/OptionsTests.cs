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
		public void Options_Property_ServerName_Default()
		{
			Assert.AreEqual(Environment.MachineName, new Options().ServerName);
		}

		[Test]
		public void Options_Property_HeartbeatInterval_Default()
		{
			Assert.AreEqual(60000, new Options().HeartbeatInterval);
		}
	}
}
