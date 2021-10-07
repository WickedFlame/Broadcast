using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Setup;
using NUnit.Framework;

namespace Broadcast.Test.Setup
{
	public class ServerSetupTests
	{
		[Test]
		public void ServerSetup_ctor()
		{
			Assert.DoesNotThrow(() => new ServerSetup());
		}
	}
}
