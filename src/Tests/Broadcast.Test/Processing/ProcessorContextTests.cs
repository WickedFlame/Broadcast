using Broadcast.EventSourcing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Broadcast.Configuration;
using Broadcast.Processing;
using Moq;

namespace Broadcast.Test.Processing
{
    [TestFixture]
    public class ProcessorContextTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ProcessorContext_ctor()
        {
	        Assert.DoesNotThrow(() => new ProcessorContext(new Mock<ITaskStore>().Object));
        }

        [Test]
        public void ProcessorContext_Options()
        {
	        var ctx = new ProcessorContext(new Mock<ITaskStore>().Object);
	        Assert.IsNotNull(ctx.Options);
        }

	}
}
