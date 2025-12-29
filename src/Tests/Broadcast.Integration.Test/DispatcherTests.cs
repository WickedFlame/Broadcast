using System;
using System.Collections.Generic;
using System.Text;

namespace Broadcast.Integration.Test
{
    public class DispatcherTests
    {
        [Test]
        public void Dispatcher_No_Registration_In_EventBus()
        {
            var eventBus = new EventBus();
            var dispatcher = new Dispatcher(eventBus);

            // Act
            var act = () => dispatcher.Publish(new FirstWorkflowEvent());

            // No exception should be thrown
            act.Should().NotThrow();
        }
    }
}
