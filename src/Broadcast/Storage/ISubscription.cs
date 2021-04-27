
namespace Broadcast.Storage
{
	/// <summary>
	/// Subscriptions are registered to containers like the <see cref="IStorage"/>.
	/// They get called on certain events like when a server heartbeat is sent.
	/// </summary>
	public interface ISubscription
	{
		/// <summary>
		/// Gets the name of the event that is related to the subscription
		/// </summary>
		string EventKey { get; }

		/// <summary>
		/// Raisee the event for the subscriber
		/// </summary>
		void RaiseEvent();
	}
}
