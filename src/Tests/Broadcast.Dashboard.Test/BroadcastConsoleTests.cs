using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Polaroider;

namespace Broadcast.Dashboard.Test
{
	public class BroadcastConsoleTests
	{
		[Test]
		public void BroadcastConsole_()
		{
			BroadcastConsole.AppendConsoleIncludes().ToString().MatchSnapshot();
		}
	}
}
