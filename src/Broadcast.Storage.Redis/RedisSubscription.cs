using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Broadcast.Storage.Redis
{
	public class RedisSubscription : IDisposable
	{
		private readonly ISubscriber _subscriber;

		private readonly List<ISubscription> _subscriptions;

		public RedisSubscription(ISubscriber subscriber)
		{
			_subscriptions = new List<ISubscription>();

			_subscriber = subscriber ?? throw new ArgumentNullException(nameof(subscriber));
			_subscriber.Subscribe(Channel, (channel, value) =>
			{
				var stringKey = value.ToString().ToLower();
				foreach (var dispatcher in _subscriptions.Where(d => stringKey.Contains(d.EventKey.ToLower())))
				{
					dispatcher.RaiseEvent();
				}
			});
		}

		internal static string Channel => "BroadcastTaskFetchChannel";

		public IEnumerable<ISubscription> Subscriptions => _subscriptions;

		public void RegisterSubscription(ISubscription subscription)
		{
			_subscriptions.Add(subscription);
		}

		public void Dispose()
		{
			_subscriber.Unsubscribe(Channel);
		}
	}
}
