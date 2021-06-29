﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Broadcast.Configuration;
using Broadcast.Dashboard.Owin;
using Broadcast.EventSourcing;
using Broadcast.Processing;
using Broadcast.Scheduling;
using Broadcast.Storage;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Owin;

namespace Broadcast.Dashboard
{
	//
	// http://www.codedigest.com/posts/8/understanding-and-creating-owin-middlewares---part-1
	//

	using MiddlewareFunc = Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>;
	using BuilderFunc = Action<Func<IDictionary<string, object>, Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>>>;

	public static class AppBuilderExtensions
	{
		public static IAppBuilder UseBroadcastServer(this IAppBuilder builder, Action<ServerSetup> config)
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

			var processor = new TaskProcessor(store, options);
			var scheduler = new Scheduler();

			TaskStore.Setup(() => store);

			BroadcastServer.Setup(c =>
				c.AddOptions(options)
					.AddProcessor(processor)
					.AddScheduler(scheduler)
					.AddTaskStore(store)
			);

			TaskServerClient.Setup(() => new BroadcastingClient(store));
			BackgroundTaskClient.Setup(() => new BroadcastingClient(store));
			
			return builder;
		}

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

			DashboardOptions.Default.RouteBasePath = $"~{pathMatch.EnsureLeadingSlash()}";
			DashboardOptions.Default.RouteBasePath = pathMatch.RemoveLeadingSlash().EnsureTrailingSlash();

			var routes = Routes.RouteCollection;
			var storage = TaskStore.Default;

			SignatureConversions.AddConversions(app);
			app.Map(pathMatch, subApp => subApp.UseOwin().UseBroadcastDashboard(storage, routes));

			return app;
		}

		private static BuilderFunc UseOwin(this IAppBuilder builder)
		{
			return middleware => builder.Use(middleware(builder.Properties));
		}

		private static BuilderFunc UseBroadcastDashboard(this BuilderFunc builder, ITaskStore storage, RouteCollection routes)
		{
			builder(_ => UseBroadcastDashboard(storage, routes));

			return builder;
		}


		private static MiddlewareFunc UseBroadcastDashboard(ITaskStore storage, RouteCollection routes)
		{
			//
			// http://www.codedigest.com/posts/8/understanding-and-creating-owin-middlewares---part-1
			//

			if (storage == null)
			{
				throw new ArgumentNullException(nameof(storage));
			}

			if (routes == null)
			{
				throw new ArgumentNullException(nameof(routes));
			}

			return
				next =>
				ctx =>
				{
					var owinContext = new OwinContext(ctx);
					
					var context = new DashboardContext(new OwinDashboardResponse(owinContext), storage);

					//if (options.Authorization.Any(filter => !filter.Authorize(context)))
					//{
					//	return Unauthorized(owinContext);
					//}

					var findResult = routes.FindDispatcher(owinContext.Request.Path.Value);

					if (findResult == null)
					{
						return next(ctx);
					}

					context.UriMatch = findResult.UriMatch;

					return findResult.Dispatcher.Dispatch(context);
				};
		}
	}
}
