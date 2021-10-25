using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#if OWIN
using Microsoft.Owin;
#endif

namespace Broadcast.Dashboard.Owin
{
#if OWIN
	//
	// http://www.codedigest.com/posts/8/understanding-and-creating-owin-middlewares---part-1
	//
	using AppFunc = Func<IDictionary<string, object>, Task>;

	/// <summary>
	/// Owin middleware for getting dashboard ressources
	/// </summary>
	public class OwinDashboardMiddleware
	{
		private readonly AppFunc _next;
		private readonly ITaskStore _storage;
		private readonly RouteCollection _routes;

		/// <summary>
		/// Creates a new OwinMiddleware
		/// </summary>
		/// <param name="next"></param>
		/// <param name="storage"></param>
		/// <param name="routes"></param>
		public OwinDashboardMiddleware(AppFunc next, ITaskStore storage, RouteCollection routes)
		{
			_next = next;

			_storage = storage;
			_routes = routes;
		}

		/// <summary>
		/// Invoke the middleware
		/// </summary>
		/// <param name="environment"></param>
		/// <returns></returns>
		public Task Invoke(IDictionary<string, object> environment)
		{
			var owinContext = new OwinContext(environment);

			var context = new DashboardContext(new OwinDashboardResponse(owinContext), _storage);

			//if (options.Authorization.Any(filter => !filter.Authorize(context)))
			//{
			//	return Unauthorized(owinContext);
			//}

			var findResult = _routes.FindDispatcher(owinContext.Request.Path.Value);

			if (findResult == null)
			{
				return _next.Invoke(environment);
			}

			context.UriMatch = findResult.UriMatch;

			return findResult.Dispatcher.Dispatch(context);
		}
	}
#endif
}
