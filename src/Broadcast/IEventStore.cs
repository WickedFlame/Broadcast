
namespace Broadcast
{
    public interface IEventStore : IDisposable
    {
        string Add<T>(string testId, DateTime time, T model) where T : IEvent;
    }
}
