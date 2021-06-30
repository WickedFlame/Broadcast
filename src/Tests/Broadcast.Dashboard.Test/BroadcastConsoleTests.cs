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

		[Test]
		public void BroadcastConsole_Position_NoOptions()
		{
			BroadcastConsole.AppendConsoleIncludes().ToString().MatchSnapshot();
		}

		[Test]
		public void BroadcastConsole_Position_Default()
		{
			BroadcastConsole.AppendConsoleIncludes(new ConsoleOptions { }).ToString().MatchSnapshot();
		}

		[Test]
		public void BroadcastConsole_Position_TopRight()
		{
			BroadcastConsole.AppendConsoleIncludes(new ConsoleOptions{Position = ConsolePosition.TopRight}).ToString().MatchSnapshot();
		}

		[Test]
		public void BroadcastConsole_Position_BottomRight()
		{
			BroadcastConsole.AppendConsoleIncludes(new ConsoleOptions { Position = ConsolePosition.BottomRight }).ToString().MatchSnapshot();
		}

		[Test]
		public void BroadcastConsole_Position_BottomLeft()
		{
			BroadcastConsole.AppendConsoleIncludes(new ConsoleOptions { Position = ConsolePosition.BottomLeft }).ToString().MatchSnapshot();
		}

		[Test]
		public void BroadcastConsole_Position_TopLeft()
		{
			BroadcastConsole.AppendConsoleIncludes(new ConsoleOptions { Position = ConsolePosition.TopLeft }).ToString().MatchSnapshot();
		}
	}
}
