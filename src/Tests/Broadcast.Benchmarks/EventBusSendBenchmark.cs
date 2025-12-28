using Broadcast.Benchmarks.Assets;
using MeasureMap;

namespace Broadcast.Benchmarks
{
    public class EventBusSendBenchmarkTest
    {
        //
        // Each higher benchmark should be 10x more

        [Test]
        public void BenchmarkTest()
        {
            var runner = new BenchmarkRunner();
            var result = runner.RunSession<EventBusSendBenchmark>();
            result.Trace();
        }
    }

    [Iterations(10)]
    public class EventBusSendBenchmark
    {
        private readonly EventBus _eventBus;

        public EventBusSendBenchmark()
        {
            _eventBus = new EventBus();

            _eventBus.Subscribe<BenchmarkEvent>(new BenchmarkEventHandler());
        }

        [Benchmark]
        public void ProcessTen()
        {
            for (int i = 0; i < 10; i++)
            {
                _eventBus.Send(new BenchmarkEvent());
            }
        }

        [Benchmark]
        public void ProcessHundred()
        {
            for (int i = 0; i < 100; i++)
            {
                _eventBus.Send(new BenchmarkEvent());
            }
        }

        [Benchmark]
        public void ProcessThousend()
        {
            for (int i = 0; i < 1000; i++)
            {
                _eventBus.Send(new BenchmarkEvent());
            }
        }
    }
}
