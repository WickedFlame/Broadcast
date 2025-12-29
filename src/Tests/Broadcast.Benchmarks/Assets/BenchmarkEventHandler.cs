using System;
using System.Collections.Generic;
using System.Text;

namespace Broadcast.Benchmarks.Assets
{
    public class BenchmarkEventHandler : IEventHandler<BenchmarkEvent>
    {
        public void Handle(BenchmarkEvent @event)
        {
            // do nothing
        }

        public void Dispose()
        {
        }
    }
}
