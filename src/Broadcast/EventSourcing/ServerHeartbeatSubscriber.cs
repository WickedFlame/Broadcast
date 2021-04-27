using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Server;
using Broadcast.Storage;

namespace Broadcast.EventSourcing
{
	/// <summary>
	/// Subscription for the <see cref="IStorage"/> that propagates the server heartbeat to the <see cref="ITaskStore"/>
	/// </summary>
	public class ServerHeartbeatSubscriber : ISubscription
	{
		private readonly ITaskStore _store;

		/// <summary>
		/// Creates a new instance of the ServerHeartbeatSubscriber
		/// </summary>
		/// <param name="store"></param>
		public ServerHeartbeatSubscriber(ITaskStore store)
		{
			_store = store ?? throw new ArgumentNullException(nameof(store));
		}

		/// <summary>
		/// Gets the name of the event that is related to the subscription
		/// </summary>
		public string EventKey { get; } = "server";

		/// <summary>
		/// Raisee the event for the subscriber.
		/// This gets called when a server creates a heartbeat and propagates the heartbeat as registration to the <see cref="ITaskStore"/>
		/// </summary>
		public void RaiseEvent()
		{
			_store.Storage(s =>
			{
				var keys = s.GetKeys(new StorageKey("server"));
				foreach (var key in keys)
				{
					var server = s.Get<ServerModel>(new StorageKey(key));
					_store.PropagateServer(server);
				}
			});
		}
	}
}
