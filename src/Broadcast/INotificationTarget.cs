
namespace Broadcast
{
    /// <summary>
    /// INotificationTarget can be registered in the <see cref="Mediator"/>. These get called when the Mediator handles a <see cref="INotification"/> object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface INotificationTarget<T> where T : INotification
    {
        void Handle(T notification);
    }
}
