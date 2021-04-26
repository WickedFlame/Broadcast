using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Diagnostics;
using Moq;
using NUnit.Framework;

namespace Broadcast.Test.Diagnostics
{
	public class LoggerExtensionsTests
	{
		[Test]
		public void LoggerExtensions_Write_Exception()
		{
			var logger = new Mock<ILogger>();
			logger.Object.Write("test", new Exception("Exception"));

			logger.Verify(exp => exp.Write(It.Is<LogMessage>(l => l.Message.StartsWith($"test{Environment.NewLine}Exception") && l.Level == LogLevel.Error), Category.Log), Times.Once);
		}

		[Test]
		public void LoggerExtensions_Write_DefaultLogLevel()
		{
			var logger = new Mock<ILogger>();
			logger.Object.Write("test");

			logger.Verify(exp => exp.Write(It.Is<LogMessage>(l => l.Message == "test" && l.Level == LogLevel.Info), It.IsAny<Category>()), Times.Once);
		}

		[Test]
		public void LoggerExtensions_Write_Level()
		{
			var logger = new Mock<ILogger>();
			logger.Object.Write("test", LogLevel.Warning);

			logger.Verify(exp => exp.Write(It.Is<LogMessage>(l => l.Level == LogLevel.Warning), It.IsAny<Category>()), Times.Once);
		}

		[Test]
		public void LoggerExtensions_Write_Category()
		{
			var logger = new Mock<ILogger>();
			logger.Object.Write("test", category: Category.Log);

			logger.Verify(exp => exp.Write(It.IsAny<LogMessage>(), Category.Log), Times.Once);
		}
		
		[Test]
		public void LoggerExtensions_Write_DefaultLogCategory()
		{
			var logger = new Mock<ILogger>();
			logger.Object.Write("test");

			logger.Verify(exp => exp.Write(It.IsAny<LogMessage>(), Category.Log), Times.Once);
		}

		[Test]
		public void LoggerExtensions_Write_Category_Invalid()
		{
			//
			// Category.Statistic is not yet implemented
			// 
			var logger = new Mock<ILogger>();
			logger.Object.Write("test", category: Category.Statistic);

			logger.Verify(exp => exp.Write(It.IsAny<LogMessage>(), Category.Log), Times.Never);
		}
	}
}
