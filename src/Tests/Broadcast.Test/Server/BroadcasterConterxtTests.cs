using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Server;
using NUnit.Framework;

namespace Broadcast.Test.Server
{
	public class BroadcasterConterxtTests
	{
		[Test]
		public void BroadcasterConterxt_ctor()
		{
			Assert.DoesNotThrow(() => new BroadcasterConterxt());
		}
	}
}
