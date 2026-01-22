namespace Broadcast
{
    public class ProjectionManager
    {
        private readonly IEventStore _eventStore;
        private readonly IEventBus _eventBus;

        public ProjectionManager(IEventStore eventStore, IEventBus eventBus)
        {
            _eventStore = eventStore;
            _eventBus = eventBus;
        }

        public IProgress<ProjectionProgress> Progress { get; set; }

        public async Task ReplayAllEventsAsync()
        {
            var events = await _eventStore.ReadAllAsync();
            var total = events.Count();

            var i = 1;
            foreach (var evnt in events.OrderBy(e => e.Time))
            {
                _eventBus.Send(evnt.Data);

                // Report every 10 items OR on the final item to ensure completion is captured
                if (i % 10 == 0 || i == total)
                {
                    Progress?.Report(new ProjectionProgress(i, total));
                }

                i++;
            }
        }
    }

    public class ProjectionProgress
    {
        public ProjectionProgress(int processed, int total)
        {
            Processed = processed;
            Total = total;
        }

        public int Processed { get; set; }

        public int Total { get; set; }
    }
}
