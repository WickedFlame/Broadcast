using System;
using System.Threading;

namespace Broadcast.Configuration
{
	/// <summary>
	/// Factory to create default instances
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ItemFactory<T>
	{
		private readonly Func<T> _defaultFactory;

		private Func<T> _clientFactory;
		private readonly object _syncLock = new object();

		/// <summary>
		/// Creates a new instance of the ItemFactory
		/// </summary>
		/// <param name="factory"></param>
		public ItemFactory(Func<T> factory)
		{
			var cachedClient = new Lazy<T>(factory, LazyThreadSafetyMode.PublicationOnly);
			_defaultFactory = () => cachedClient.Value;
		}

		/// <summary>
		/// Gets or sets the factory for the item.
		/// The new factory is stored as a singleton at the first resolving.
		/// Resete the factory to the default instance by assigning null
		/// </summary>
		public Func<T> Factory
		{
			get
			{
				lock (_syncLock)
				{
					return _clientFactory ?? _defaultFactory;
				}
			}
			set
			{
				lock (_syncLock)
				{
					if (value == null)
					{
						// reset the factory to the default
						_clientFactory = null;
						return;
					}

					var factory = new Lazy<T>(value, LazyThreadSafetyMode.PublicationOnly);
					_clientFactory = () => factory.Value;
				}
			}
		}
	}
}
