using Broadcast.Benchmarks.Assets;
using MeasureMap;

namespace Broadcast.Benchmarks
{
    public class DispatcherSendBenchmarkTest
    {
        //
        // Each higher benchmark should be 10x more

        [Test]
        public void BenchmarkTest()
        {
            var runner = new BenchmarkRunner();
            var result = runner.RunSession<DispatcherSendBenchmark>();
            result.Trace();
        }
    }

    [Iterations(10)]
    public class DispatcherSendBenchmark
    {
        private readonly IDispatcher<IEvent> _dispatcher;

        public DispatcherSendBenchmark()
        {
            var eventBus = new EventBus();
            eventBus.Subscribe<BenchmarkEvent>(new BenchmarkEventHandler());
            _dispatcher = new Dispatcher<IEvent>(eventBus);
        }

        [Benchmark]
        public void ProcessTen()
        {
            for (int i = 0; i < 10; i++)
            {
                _dispatcher.Publish(new BenchmarkEvent());
            }
        }

        [Benchmark]
        public void ProcessHundred()
        {
            for (int i = 0; i < 100; i++)
            {
                _dispatcher.Publish(new BenchmarkEvent());
            }
        }

        [Benchmark]
        public void ProcessThousend()
        {
            for (int i = 0; i < 1000; i++)
            {
                _dispatcher.Publish(new BenchmarkEvent());
            }
        }
    }
}
