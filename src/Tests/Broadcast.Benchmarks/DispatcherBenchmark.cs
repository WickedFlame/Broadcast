using Broadcast.Benchmarks.Assets;
using MeasureMap;

namespace Broadcast.Benchmarks
{
    public class DispatcherBenchmarkTest
    {
        [Test]
        public void BenchmarkTest()
        {
            var runner = new BenchmarkRunner();
            var result = runner.RunSession<DispatcherBenchmark>();
            result.Trace();
        }
    }

    [Iterations(10)]
    public class DispatcherBenchmark
    {
        private readonly IDispatcher<IEvent> _dispatcher;

        public DispatcherBenchmark()
        {
            var eventBus = new EventBus();
            eventBus.Subscribe<BenchmarkEvent>(new BenchmarkEventHandler());
            _dispatcher = new Dispatcher<IEvent>(eventBus);
        }

        [Benchmark]
        public void Send()
        {
            for (int i = 0; i < 100; i++)
            {
                _dispatcher.Send(new BenchmarkEvent());
            }
        }

        [Benchmark]
        public void Enqueue()
        {
            for (int i = 0; i < 100; i++)
            {
                _dispatcher.Enqueue(new BenchmarkEvent());
            }
        }
    }
}
