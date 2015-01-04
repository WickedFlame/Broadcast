using Broadcast.EventSourcing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Broadcast.Test
{
    [TestClass]
    public class ProcessorContextTests
    {
        [TestInitialize]
        public void Setup()
        {
            ProcessorContextFactory.ContextFactory = () => new ProcessorContext();
            ProcessorContextFactory.ModeFactory = null;
        }

        [TestMethod]
        public void DefaultProcessorContextTest()
        {
            var context = new ProcessorContext();

            Assert.IsTrue(context.Mode == ProcessorMode.Default);
            Assert.IsNotNull(context.TaskStore);
        }

        [TestMethod]
        public void ProcessorContextWithCustomModeTest()
        {
            ProcessorContextFactory.ModeFactory = () => ProcessorMode.Async;

            var context = new ProcessorContext();

            Assert.IsTrue(context.Mode == ProcessorMode.Async);
            Assert.IsNotNull(context.TaskStore);
        }

        [TestMethod]
        public void ProcessorContextFactoryTest()
        {
            var context = new ProcessorContext();
            ProcessorContextFactory.ContextFactory = () => context;

            Assert.AreSame(context, ProcessorContextFactory.ContextFactory());
        }

        [TestMethod]
        public void BroadcasterWithCustomProcessorContextFactoryTest()
        {
            var context = new ProcessorContext();
            ProcessorContextFactory.ContextFactory = () => context;

            var broadcaster = new Broadcaster();

            Assert.AreSame(context, broadcaster.Context);
        }
    }
}
