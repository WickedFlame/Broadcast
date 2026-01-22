namespace Broadcast.Test
{
    public class ProjectionManagerTests
    {
        private Mock<IEventStore> _eventStore;
        private Mock<IEventBus> _eventBus;
        private ProjectionManager _projection;

        [SetUp]
        public void SetUp()
        {
            _eventStore = new Mock<IEventStore>();
            _eventBus = new Mock<IEventBus>();

            _projection = new ProjectionManager(_eventStore.Object, _eventBus.Object);
        }

        [Test]
        public async Task ProjectionManager_ReplayAllEventsAsync()
        {
            _eventStore.Setup(x => x.ReadAllAsync()).ReturnsAsync(
            [
                new EventEnvelope { Time = DateTime.UtcNow.AddMinutes(-5), Data = new { Id = "Event1" } },
                new EventEnvelope { Time = DateTime.UtcNow.AddMinutes(-3), Data = new { Id = "Event2" } },
                new EventEnvelope { Time = DateTime.UtcNow.AddMinutes(-1), Data = new { Id = "Event3" } },
            ]);

            await _projection.ReplayAllEventsAsync();

            _eventBus.Verify(x => x.Send(It.IsAny<object>()), Times.Exactly(3));
        }

        [Test]
        public async Task ProjectionManager_ReplayAllEventsAsync_Progress()
        {
            var lst = new List<EventEnvelope>();
            for (var i = 0; i < 100; i++)
            {
                lst.Add(new EventEnvelope { Time = DateTime.UtcNow.AddMinutes(-i), Data = new { Id = $"Event{i}" } });
            }

            _eventStore.Setup(x => x.ReadAllAsync()).ReturnsAsync(() => lst);

            var cnt = 0;

            _projection.Progress = new Progress<ProjectionProgress>(p =>
            {
                cnt++;
                Console.WriteLine($"Replaying events: {p.Processed}/{p.Total}");
            });

            await _projection.ReplayAllEventsAsync();

            Task.Delay(500).Wait();

            cnt.Should().BeGreaterThan(0);
        }
    }
}
