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
        public void ProcessorOptions_ctor()
        {
            Assert.DoesNotThrow(() => new ProcessorOptions());
        }

		[Test]
		public void ProcessorOptions_Property_ServerName_Default()
		{
			Assert.AreEqual(Environment.MachineName, new ProcessorOptions().ServerName);
		}

		[Test]
		public void ProcessorOptions_Property_HeartbeatInterval_Default()
		{
			Assert.AreEqual(60000, new ProcessorOptions().HeartbeatInterval);
		}
	}
}
