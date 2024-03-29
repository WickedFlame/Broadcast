﻿using System;
using Broadcast.Configuration;
using Broadcast.Dashboard.Owin;
using Broadcast.EventSourcing;
using Broadcast.Processing;
using Broadcast.Scheduling;
using Broadcast.Setup;
using Broadcast.Storage;
#if OWIN
using Microsoft.Owin.Infrastructure;
using Owin;
#endif

namespace Broadcast.Dashboard
{
#if OWIN
	/// <summary>
	/// 
	/// </summary>
	public static class AppBuilderExtensions
	{
		/// <summary>
		/// Add a broadcast server to the application
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public static IAppBuilder UseBroadcastServer(this IAppBuilder builder, Action<IServerSetup> config)
		{
			if (builder == null)
			{
				throw new ArgumentNullException(nameof(builder));
			}

			var serverSetup = new ServerSetup();

			config(serverSetup);

			var options = serverSetup.Resolve<Options>() ?? new Options();
			var storage = serverSetup.Resolve<IStorage>() ?? new InmemoryStorage();
			var store = serverSetup.Resolve<ITaskStore>() ?? new TaskStore(options, storage);

            var processorOptions = serverSetup.Resolve<ProcessorOptions>() ?? new ProcessorOptions();
			var processor = new TaskProcessor(store, processorOptions);
			var scheduler = new Scheduler();

			TaskStore.Setup(() => store);

			BroadcastServer.Setup(c =>
				c.AddOptions(processorOptions)
					.AddProcessor(processor)
					.AddScheduler(scheduler)
					.AddTaskStore(store)
			);

			BackgroundTask.Setup(() => new BroadcastingClient(store));
			
			return builder;
		}

		/// <summary>
		/// Add the dashboard for broadcast
		/// </summary>
		/// <param name="app"></param>
		/// <param name="pathMatch"></param>
		/// <returns></returns>
		public static IAppBuilder UseBroadcastDashboard(this IAppBuilder app, string pathMatch = "/broadcast")
		{
			if (app == null)
			{
				throw new ArgumentNullException(nameof(app));
			}

			if (pathMatch == null)
			{
				throw new ArgumentNullException(nameof(pathMatch));
			}

			DashboardOptions.Default.RouteBasePath = pathMatch.EnsureLeadingSlash();
			DashboardOptions.Default.TemplateParameters["%(RouteBasePath)"] = pathMatch.EnsureLeadingSlash();


			var routes = Routes.RouteCollection;
			var storage = TaskStore.Default;

			SignatureConversions.AddConversions(app);
			app.Map(pathMatch, subApp => subApp.Use<OwinDashboardMiddleware>(storage, routes));

			return app;
		}
	}
#endif
}
