using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Broadcast.Dashboard
{
	public class DashboardMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly RouteCollection _routes;
		private readonly ITaskStore _storage;

		public DashboardMiddleware(RequestDelegate next, RouteCollection routes, ITaskStore storage)
		{
			_next = next ?? throw new ArgumentNullException(nameof(next));
			_routes = routes ?? throw new ArgumentNullException(nameof(routes));
			_storage = storage ?? throw new ArgumentNullException(nameof(storage));
		}

		public async Task Invoke(HttpContext httpContext)
		{
			var context = new AspNetCoreDashboardContext(httpContext, _storage);
			var findResult = _routes.FindDispatcher(httpContext.Request.Path.Value);

			if (findResult == null)
			{
				await _next.Invoke(httpContext);
				return;
			}

			//foreach (var filter in _options.Authorization)
			//{
			//    if (!filter.Authorize(context))
			//    {
			//        var isAuthenticated = httpContext.User?.Identity?.IsAuthenticated;

			//        httpContext.Response.StatusCode = isAuthenticated == true
			//            ? (int)HttpStatusCode.Forbidden
			//            : (int)HttpStatusCode.Unauthorized;

			//        return;
			//    }
			//}

			context.UriMatch = findResult.UriMatch;

			await findResult.Dispatcher.Dispatch(context);
		}
	}
}
