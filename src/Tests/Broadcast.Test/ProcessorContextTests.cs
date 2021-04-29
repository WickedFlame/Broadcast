using Broadcast.EventSourcing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Broadcast.Configuration;

namespace Broadcast.Test
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
	        Assert.DoesNotThrow(() => new ProcessorContext());
        }

        [Test]
        public void ProcessorContext_Options()
        {
	        var ctx = new ProcessorContext();
	        Assert.IsNotNull(ctx.Options);
        }

	}
}
