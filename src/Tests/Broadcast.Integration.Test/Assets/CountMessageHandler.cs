using System;
using System.Collections.Generic;
using System.Text;

namespace Broadcast.Integration.Test
{
    public class CountMessageHandler
        : IMessageHandler<CountEvent>
    {
        public int Count { get; private set; }

        public void Handle(CountEvent @event)
        {
            Count++;
        }

        public void Dispose()
        {
        }        
    }

    public class CountMessageHandlerTwo
        : IMessageHandler<CountEvent>
    {
        public int Count { get; private set; }

        public void Handle(CountEvent @event)
        {
            Count++;
        }

        public void Dispose()
        {
        }
    }
}
