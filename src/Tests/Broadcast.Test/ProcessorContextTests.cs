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
        public void DefaultProcessorContextTest()
        {
            var context = new ProcessorContext(new TaskStore(new Options()));

            Assert.IsNotNull(context.Store);
        }

        [Test]
        public void ProcessorContextWithCustomModeTest()
        {
            var context = new ProcessorContext(new TaskStore(new Options()));

            Assert.IsNotNull(context.Store);
        }

        [Test]
        public void ProcessorContextWithCustomAndDefaultModeTest()
        {
            var context = new ProcessorContext(new TaskStore(new Options()));

            Assert.IsNotNull(context.Store);
        }
		
        [Test]
        public void BroadcasterCustom_TaskStore_InBroadcaster()
        {
	        var broadcaster = new Broadcaster
	        {
		        Context = new ProcessorContext(new TaskStore(new Options()))
	        };

            Assert.AreSame(broadcaster.GetStore(), broadcaster.Context.Store);
        }

        [Test]
        public void BroadcasterCustom_Open_Multiple()
        {
	        var context = new ProcessorContext(new TaskStore(new Options()));

	        Assert.AreSame(context.Open(), context.Open());
        }

        [Test]
        public void BroadcasterCustom_Open_DifferentContext()
        {
	        Assert.AreNotSame(new ProcessorContext(new TaskStore(new Options())).Open(), new ProcessorContext(new TaskStore(new Options())).Open());
        }
	}
}
