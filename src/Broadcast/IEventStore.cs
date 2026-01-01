
namespace Broadcast
{
    public interface IEventStore : IDisposable
    {
        [Obsolete("Use Add with full metadata instead", false)]
        string Add<T>(string testId, DateTime time, T model) where T : class;

        string Add<T>(string eventId, string streamId, int streamVersion, string type, DateTime time, T data) where T : class;
    }
}
