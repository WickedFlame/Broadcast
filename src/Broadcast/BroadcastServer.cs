using System;
using Broadcast.Configuration;
using Broadcast.EventSourcing;
using Broadcast.Processing;
using Broadcast.Scheduling;

namespace Broadcast
{
	/// <summary>
	/// Default server
	/// </summary>
	public class BroadcastServer
	{
		static BroadcastServer()
		{
			// setup the default server
			// this is created even if it is not used
			//Setup(s => { });
		}

		/// <summary>
		/// Gets the BroadcasterServer
		/// </summary>
		public static IBroadcaster Server { get; private set; }

		/// <summary>
		/// Setup a BroadcasterServer.
		/// This uses the default store from TaskStore.Default and the default options from Options.Default
		/// </summary>
		/// <param name="config"></param>
		public static void Setup(Action<ServerConfiguration> config)
		{
			// dispose existing servers to ensure the scheduler threads and all dispatchers are ended
			Server?.Dispose();

			var serverSetup = new ServerConfiguration();
			config(serverSetup);

			var store = serverSetup.Get<ITaskStore>() ?? TaskStore.Default;
			var options = serverSetup.Get<Options>() ?? new Options();
			var processor = serverSetup.Get<ITaskProcessor>() ?? new TaskProcessor(store, options);
			var scheduler = serverSetup.Get<IScheduler>() ?? new Scheduler();


			var server = new Broadcaster(store, processor, scheduler, options);
			Server = server;
		}
	}
}
