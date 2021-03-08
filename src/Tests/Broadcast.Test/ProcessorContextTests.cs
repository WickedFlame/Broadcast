using Broadcast.EventSourcing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Broadcast.Test
{
    [TestFixture]
    public class ProcessorContextTests
    {
        [SetUp]
        public void Setup()
        {
            ProcessorContextFactory.ContextFactory = () => new ProcessorContext();
            //ProcessorContextFactory.ModeFactory = null;
        }

        [Test]
        public void DefaultProcessorContextTest()
        {
            var context = new ProcessorContext();

            Assert.IsTrue(context.Mode == ProcessorMode.Background);
            Assert.IsNotNull(context.Store);
        }

        [Test]
        public void ProcessorContextWithCustomModeTest()
        {
            //ProcessorContextFactory.ModeFactory = () => ProcessorMode.Serial;

            var context = new ProcessorContext();

            Assert.IsTrue(context.Mode == ProcessorMode.Background);
            Assert.IsNotNull(context.Store);
        }

        [Test]
        public void ProcessorContextWithCustomAndDefaultModeTest()
        {
            //ProcessorContextFactory.ModeFactory = () => ProcessorMode.Serial;

            var context = new ProcessorContext(ProcessorMode.Background);

            Assert.IsTrue(context.Mode == ProcessorMode.Background);
            Assert.IsNotNull(context.Store);
        }

        [Test]
        public void ProcessorContextFactoryTest()
        {
            var context = new ProcessorContext();
            ProcessorContextFactory.ContextFactory = () => context;

            Assert.AreSame(context, ProcessorContextFactory.ContextFactory());
        }

        [Test]
        public void BroadcasterWithCustomProcessorContextFactoryTest()
        {
            var context = new ProcessorContext();
            ProcessorContextFactory.ContextFactory = () => context;

            var broadcaster = new Broadcaster();

            Assert.AreSame(context, broadcaster.Context);
        }
    }
}
