﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Broadcast.Configuration;
using Broadcast.EventSourcing;
using Broadcast.Setup;
using Broadcast.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Broadcast
{
	/// <summary>
	/// Extensions for <see cref="IServiceCollection"/>
	/// </summary>
	public static class BroadcastServiceCollectionExtensions
	{
		/// <summary>
		/// Configure and initialialize <see cref="Broadcaster"/> with a default <see cref="InmemoryStorage"/> for managing tasks.
		/// All items are added to the <see cref="IServiceCollection"/>.
		/// The configured <see cref="ITaskStore"/> ist set as the default <see cref="ITaskStore"/>. 
		/// The server has to be added with app.UseBroadcastServer()
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceCollection AddBroadcast(this IServiceCollection services)
		{
			return services.AddBroadcast(c => c.UseTaskStore(new TaskStore()));
		}

		/// <summary>
		/// Configure and initialialize <see cref="Broadcaster"/> with the passed setup.
		/// All items are added to the <see cref="IServiceCollection"/>
		/// The configured <see cref="ITaskStore"/> ist set as the default <see cref="ITaskStore"/>. 
		/// The server has to be added with app.UseBroadcastServer()
		/// </summary>
		/// <param name="services"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public static IServiceCollection AddBroadcast(this IServiceCollection services, Action<ServerSetup> config)
		{
			var serverSetup = new ServerSetup();

			config(serverSetup);

			var options = serverSetup.Resolve<Options>() ?? new Options();
			services.TryAddSingletonChecked(_ => options);

			var storage = serverSetup.Resolve<IStorage>() ?? new InmemoryStorage();
			
			var store = serverSetup.Resolve<ITaskStore>() ?? new TaskStore(options, storage);
			services.TryAddSingletonChecked(_ => store);

			TaskStore.Setup(() => store);

			return services;
		}

		private static void TryAddSingletonChecked<T>(this IServiceCollection serviceCollection, Func<IServiceProvider, T> implementationFactory) where T : class
		{
			serviceCollection.TryAddSingleton<T>(serviceProvider =>
			{
				if (serviceProvider == null)
				{
					throw new ArgumentNullException(nameof(serviceProvider));
				}

				// ensure the configuration was performed
				//serviceProvider.GetRequiredService<IGlobalConfiguration>();

				return implementationFactory(serviceProvider);
			});
		}
	}
}
