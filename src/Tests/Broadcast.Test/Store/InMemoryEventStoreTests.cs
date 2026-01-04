using Polaroider;

namespace Broadcast.Test.Store
{
    public class InMemoryEventStoreTests
    {
        private InMemoryEventStore _eventStore;

        [SetUp]
        public void Setup()
        {
            _eventStore = new InMemoryEventStore();
        }

        [Test]
        public void InMemoryEventStore_AddAsync()
        {
            // Act
            var result = _eventStore.AddAsync("event-1", "stream-1", 1, "TestType", DateTime.UtcNow, new { Name = "Test Event" }).Result;

            // Assert
            result.MatchSnapshot();
        }

        [Test]
        public void InMemoryEventStore_AddAsync_EventStoreItem()
        {
            // Act
            var _ = _eventStore.AddAsync("event-1", "stream-1", 1, "TestType", DateTime.UtcNow, new { Name = "Test Event" }).Result;

            // Assert
            _eventStore.Events.Single().MatchSnapshot(SnapshotOptions.Default.MockDateTimes());
        }

        [Test]
        public async Task InMemoryEventStore_ReadStreamAsync()
        {
            await _eventStore.AddAsync("event-1", "stream-1", 1, "TestType", DateTime.UtcNow, new { Name = "Test Event" });
            await _eventStore.AddAsync("event-2", "stream-2", 1, "TestType", DateTime.UtcNow, new { Name = "Test Event" });
            await _eventStore.AddAsync("event-3", "stream-1", 1, "TestType", DateTime.UtcNow, new { Name = "Test Event" });

            var envelopes = await _eventStore.ReadStreamAsync("stream-1");
            
            envelopes.Should().HaveCount(2);
            envelopes.All(e => e.StreamId == "stream-1").Should().BeTrue();
        }

        [Test]
        public async Task InMemoryEventStore_ReadAllAsync()
        {
            await _eventStore.AddAsync("event-1", "stream-1", 1, "TestType", DateTime.UtcNow, new { Name = "Test Event" });
            await _eventStore.AddAsync("event-2", "stream-2", 1, "TestType", DateTime.UtcNow, new { Name = "Test Event" });
            await _eventStore.AddAsync("event-3", "stream-1", 1, "TestType", DateTime.UtcNow, new { Name = "Test Event" });

            var envelopes = await _eventStore.ReadAllAsync();

            envelopes.Should().HaveCount(3);
        }
    }
}
