using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Broadcast.Diagnostics;
using Moq;
using NUnit.Framework;

namespace Broadcast.Test.Diagnostics
{
	public class LoggerFactoryTests
	{
		[Test]
		public void LoggerFactory_Create()
		{
			Assert.IsNotNull(LoggerFactory.Create());
		}

		[Test]
		public void LoggerFactory_Create_VerifyInstance()
		{
			var logger = LoggerFactory.Create();
			Assert.IsAssignableFrom<Logger>(logger);
		}

		[Test]
		public void LoggerFactory_Create_NotSame()
		{
			Assert.AreNotSame(LoggerFactory.Create(), LoggerFactory.Create());
		}

		[Test]
		public void LoggerFactory_Create_DefaultWriters_Count()
		{
			var logger = LoggerFactory.Create() as Logger;
			Assert.AreEqual(1, logger.Writers.Count);
		}

		[Test]
		public void LoggerFactory_Create_DefaultWriters_TraceLogWriter()
		{
			var logger  = LoggerFactory.Create() as Logger;
			Assert.IsTrue(logger.Writers.Any(w => w.GetType() == typeof(TraceLogWriter)));
		}

		[Test]
		public void LoggerFactory_Create_CustomLogger()
		{
			var def = new Logger();
			LoggerFactory.Setup(() => def);

			Assert.AreSame(def, LoggerFactory.Create());
		}

		[Test]
		public void LoggerFactory_Create_CustomLogger_Reset()
		{
			LoggerFactory.Setup(() => new Logger());
			LoggerFactory.Setup(null);

			Assert.AreNotSame(LoggerFactory.Create(), LoggerFactory.Create());
		}
	}
}
