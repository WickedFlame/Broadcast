using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Diagnostics;
using Moq;
using NUnit.Framework;

namespace Broadcast.Test.Diagnostics
{
	public class LoggerTests
	{
		[Test]
		public void Logger_ctor()
		{
			Assert.DoesNotThrow(() => new Logger());
		}

		[Test]
		public void Logger_Writers_NotNull()
		{
			var logger = new Logger();
			Assert.IsNotNull(logger.Writers);
		}

		[Test]
		public void Logger_Write()
		{
			var writer1 = new Mock<ILogWriter>();
			writer1.Setup(exp => exp.Category).Returns(() => Category.Log);

			var logger = new Logger();
			logger.Writers.Add(writer1.Object);

			logger.Write(new LogMessage("test", LogLevel.Info), Category.Log);

			writer1.Verify(exp => exp.Write(It.IsAny<LogMessage>()), Times.Once);
		}

		[Test]
		public void Logger_Write_CheckCategory_Log()
		{
			var writer1 = new Mock<ILogWriter>();
			writer1.Setup(exp => exp.Category).Returns(() => Category.Log);
			var writer2 = new Mock<ILogWriter>();
			writer1.Setup(exp => exp.Category).Returns(() => Category.Statistic);

			var logger = new Logger();
			logger.Writers.Add(writer1.Object);
			logger.Writers.Add(writer2.Object);

			logger.Write(new LogMessage("test", LogLevel.Info), Category.Log);

			writer1.Verify(exp => exp.Write(It.IsAny<LogMessage>()), Times.Once);
			writer2.Verify(exp => exp.Write(It.IsAny<LogMessage>()), Times.Never);
		}

		[Test]
		public void Logger_Write_CheckCategory_Statistic()
		{
			var writer1 = new Mock<ILogWriter>();
			writer1.Setup(exp => exp.Category).Returns(() => Category.Log);
			var writer2 = new Mock<ILogWriter>();
			writer1.Setup(exp => exp.Category).Returns(() => Category.Statistic);

			var logger = new Logger();
			logger.Writers.Add(writer1.Object);
			logger.Writers.Add(writer2.Object);

			logger.Write(new LogMessage("test", LogLevel.Info), Category.Statistic);

			writer1.Verify(exp => exp.Write(It.IsAny<LogMessage>()), Times.Never);
			writer2.Verify(exp => exp.Write(It.IsAny<LogMessage>()), Times.Once);
		}

		[Test]
		public void Logger_Write_Exception()
		{
			var writer1 = new Mock<ILogWriter>();
			writer1.Setup(exp => exp.Category).Returns(() => Category.Log);
			writer1.Setup(exp => exp.Write(It.IsAny<LogMessage>())).Throws< Exception>();

			var logger = new Logger();
			logger.Writers.Add(writer1.Object);

			Assert.DoesNotThrow(() => logger.Write(new LogMessage("test", LogLevel.Info), Category.Log));
		}
	}
}
