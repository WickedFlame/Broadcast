using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;

namespace Broadcast.Dashboard.Test
{
	public class RouteCollectionTests
	{
		[Test]
		public void RouteCollection_ctor()
		{
			Assert.DoesNotThrow(() => new RouteCollection());
		}

		[Test]
		public void RouteCollection_Add()
		{
			var dispatcher = new Mock<IDashboardDispatcher>();

			var routes = new RouteCollection();
			Assert.DoesNotThrow(() => routes.Add("test", dispatcher.Object));
		}

		[Test]
		public void RouteCollection_Add_NullPath()
		{
			var dispatcher = new Mock<IDashboardDispatcher>();

			var routes = new RouteCollection();
			Assert.Throws<ArgumentNullException>(() => routes.Add(null, dispatcher.Object));
		}

		[Test]
		public void RouteCollection_Add_NoPath()
		{
			var dispatcher = new Mock<IDashboardDispatcher>();

			var routes = new RouteCollection();
			Assert.Throws<ArgumentNullException>(() => routes.Add(string.Empty, dispatcher.Object));
		}

		[Test]
		public void RouteCollection_Add_NullDispatcher()
		{
			var routes = new RouteCollection();
			Assert.Throws<ArgumentNullException>(() => routes.Add("test", null));
		}

		[Test]
		public void RouteCollection_Find()
		{
			var dispatcher = new Mock<IDashboardDispatcher>();

			var routes = new RouteCollection();
			routes.Add("test", dispatcher.Object);

			Assert.NotNull(routes.FindDispatcher("test"));
		}

		[Test]
		public void RouteCollection_Find_CheckPath()
		{
			var dispatcher = new Mock<IDashboardDispatcher>();

			var routes = new RouteCollection();
			routes.Add("test", dispatcher.Object);

			Assert.AreEqual(routes.FindDispatcher("test").UriMatch.Value, "test");
		}

		[Test]
		public void RouteCollection_Find_CheckDispatcher()
		{
			var dispatcher = new Mock<IDashboardDispatcher>();

			var routes = new RouteCollection();
			routes.Add("test", dispatcher.Object);

			Assert.AreSame(routes.FindDispatcher("test").Dispatcher, dispatcher.Object);
		}

		[Test]
		public void RouteCollection_NotFound()
		{
			var dispatcher = new Mock<IDashboardDispatcher>();

			var routes = new RouteCollection();
			routes.Add("test", dispatcher.Object);

			Assert.Null(routes.FindDispatcher("invalid"));
		}

		[Test]
		public void RouteCollection_Find_EmptyRoute()
		{
			var dispatcher = new Mock<IDashboardDispatcher>();

			var routes = new RouteCollection();
			routes.Add("/", dispatcher.Object);

			Assert.AreSame(routes.FindDispatcher("").Dispatcher, dispatcher.Object);
		}

		[Test]
		public void RouteCollection_Find_IgnoreCase()
		{
			var dispatcher = new Mock<IDashboardDispatcher>();

			var routes = new RouteCollection();
			routes.Add("test", dispatcher.Object);

			Assert.AreSame(routes.FindDispatcher("TEST").Dispatcher, dispatcher.Object);
		}
	}
}
