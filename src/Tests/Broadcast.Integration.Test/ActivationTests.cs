using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Broadcast.Integration.Test
{
    public class ActivationTests
    {
        [Test]
        public void Activate_Parameter_Simple()
        {
            var broadcast = new Broadcaster();

            var task = new ActivationTask();
            broadcast.Send(() => task.Execute("test"));

            broadcast.WaitAll();
            broadcast.Store.Should().OnlyContain(t => t.State == EventSourcing.TaskState.Processed);
        }

        [Test]
        public void Activate_Parameter_Null()
        {
            var broadcast = new Broadcaster();

            var task = new ActivationTask();
            broadcast.Send(() => task.Execute(null));

            broadcast.WaitAll();
            broadcast.Store.Should().OnlyContain(t => t.State == EventSourcing.TaskState.Processed);
        }

        public class ActivationTask
        {
            public void Execute(string parameter)
            {
                // do nothing
            }
        }
    }
}
