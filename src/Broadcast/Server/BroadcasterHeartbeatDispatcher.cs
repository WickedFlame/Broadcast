using System;
using System.Threading.Tasks;
using Broadcast.Configuration;
using Broadcast.EventSourcing;
using Broadcast.Storage;

namespace Broadcast.Server
{
	/// <summary>
	/// <see cref="IBackgroundDispatcher{T}"/> that propagates the heartbeat and the serverdata to the <see cref="IStorage"/>.
	/// The <see cref="IStorage"/> is responsible to propagate the server to all other servers that are registered to the same <see cref="IStorage"/>
	/// </summary>
	public class BroadcasterHeartbeatDispatcher : IBackgroundDispatcher<IBroadcasterConterxt>
	{
		private readonly ITaskStore _store;
		private readonly Options _options;

		/// <summary>
		/// Creates a new instance of the BroadcasterHeartbeatDispatcher
		/// </summary>
		/// <param name="store"></param>
		/// <param name="options"></param>
		public BroadcasterHeartbeatDispatcher(ITaskStore store, Options options)
		{
			_store = store ?? throw new ArgumentNullException(nameof(store));
			_options = options ?? throw new ArgumentNullException(nameof(options));
		}

		/// <summary>
		/// Execute the dispatcher to propagate the heartbeat to the <see cref="ITaskStore"/> and <see cref="IStorage"/>
		/// </summary>
		/// <param name="context"></param>
		public async void Execute(IBroadcasterConterxt context)
		{
			while (context.IsRunning)
			{
				var model = new ServerModel
				{
					Name = _options.ServerName, 
					Id = context.Id,
					Heartbeat = DateTime.Now
				};
				_store.Storage(s => s.Set(new StorageKey($"server:{_options.ServerName}:{context.Id}"), model));

				// don't run again for at least 
				await Task.Delay(_options.HeartbeatInterval);
			}
		}
	}
}
