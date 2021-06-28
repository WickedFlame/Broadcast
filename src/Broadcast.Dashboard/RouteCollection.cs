using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Broadcast.Dashboard
{
	/// <summary>
	/// Collection for storing and retrieving routes
	/// </summary>
	public class RouteCollection
	{
		private readonly List<DispatcherRoute> _dispatchers
			= new List<DispatcherRoute>();

		/// <summary>
		/// Add a new route and <see cref="IDashboardDispatcher"/> to the collection
		/// </summary>
		/// <param name="pathTemplate"></param>
		/// <param name="dispatcher"></param>
		public void Add(string pathTemplate, IDashboardDispatcher dispatcher)
		{
			if (string.IsNullOrEmpty(pathTemplate))
			{
				throw new ArgumentNullException(nameof(pathTemplate));
			}

			if (dispatcher == null)
			{
				throw new ArgumentNullException(nameof(dispatcher));
			}

			_dispatchers.Add(new DispatcherRoute
			{
				Path = pathTemplate, 
				Dispatcher = dispatcher
			});
		}

		/// <summary>
		/// Find the <see cref="IDashboardDispatcher"/> associated with the path
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public RouteMatch FindDispatcher(string path)
		{
			if (path.Length == 0)
			{
				path = "/";
			}

			foreach (var dispatcher in _dispatchers)
			{
				var pattern = dispatcher.Path;

				if (!pattern.StartsWith("^", StringComparison.OrdinalIgnoreCase))
				{
					pattern = "^" + pattern;
				}

				if (!pattern.EndsWith("$", StringComparison.OrdinalIgnoreCase))
				{
					pattern += "$";
				}

				var match = Regex.Match(path, pattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);
				if (match.Success)
				{
					return new RouteMatch
					{
						Dispatcher = dispatcher.Dispatcher,
						UriMatch = match
					};
				}
			}

			return null;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class DispatcherRoute
	{
		/// <summary>
		/// Gets the found <see cref="IDashboardDispatcher"/>
		/// </summary>
		public IDashboardDispatcher Dispatcher { get; set; }

		/// <summary>
		/// Gets the registerd route
		/// </summary>
		public string Path { get; set; }
	}

	/// <summary>
	/// Match for the dispatcher
	/// </summary>
	public class RouteMatch
	{
		/// <summary>
		/// Gets the found <see cref="IDashboardDispatcher"/>
		/// </summary>
		public IDashboardDispatcher Dispatcher { get; set; }

		/// <summary>
		/// Gets the found Uri
		/// </summary>
		public Match UriMatch { get; set; }
	}
}
