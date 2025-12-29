using System;
using System.Collections.Generic;
using System.Text;

namespace Broadcast.Integration.Test
{
    public class CountEventHandler
        : IEventHandler<CountEvent>
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
        : IEventHandler<CountEvent>
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
