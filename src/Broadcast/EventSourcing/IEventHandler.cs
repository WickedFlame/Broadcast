
namespace Broadcast
{
    public interface IEventHandler<in T>
    {
        void Handle(T @event);
    }
}
