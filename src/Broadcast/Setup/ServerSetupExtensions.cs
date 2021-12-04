using Broadcast.Configuration;

namespace Broadcast
{
	/// <summary>
	/// Extensions for the <see cref="IServerSetup"/>
	/// </summary>
	public static class ServerSetupExtensions
	{
		/// <summary>
		/// Setup the ProcessingServer with the help of the PipelineBuilder
		/// </summary>
		/// <param name="setup"></param>
		/// <param name="store"></param>
		/// <returns></returns>
		public static IServerSetup UseTaskStore(this IServerSetup setup, ITaskStore store)
		{
			setup.Register(store);

			return setup;
		}

		/// <summary>
		/// Set the options that are used for the Storage
		/// </summary>
		/// <param name="setup"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static IServerSetup UseOptions(this IServerSetup setup, Options options)
		{
			setup.Register(options);
			return setup;
		}

        /// <summary>
        /// Set the options that are used for the Server
        /// </summary>
        /// <param name="setup"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServerSetup UseOptions(this IServerSetup setup, ProcessorOptions options)
        {
            setup.Register(options);
            return setup;
        }

		/// <summary>
		/// Register a item in the <see cref="IServerSetup"/> Context
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="setup"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		public static IServerSetup Register<T>(this IServerSetup setup, T item)
		{
			setup.Context[typeof(T).Name] = item;

			return setup;
		}

		/// <summary>
		/// Resolve a item from the <see cref="IServerSetup"/> Context
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="setup"></param>
		/// <returns></returns>
		public static T Resolve<T>(this IServerSetup setup)
		{
			var key = typeof(T).Name;
			if (setup.Context.ContainsKey(key))
			{
				return (T)setup.Context[key];
			}

			return default(T);
		}
	}
}
