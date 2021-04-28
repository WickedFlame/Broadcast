using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Broadcast.EventSourcing
{
	/// <summary>
	/// A storage for managing <see cref="IDispatcher"/>
	/// <see cref="IDispatcher"/> are stored per Id of the <see cref="IBroadcaster"/>.
	/// Uses a round robin implementation to get the next set of <see cref="IDispatcher"/> for processing
	/// </summary>
	public class DispatcherStorage
	{
		private readonly object _lockObject = new object();

		private readonly Dictionary<string, IDispatcher[]> _dispatchers;
		private readonly List<string> _ids;
		private int _currentIndex;

		/// <summary>
		/// Creates a new instance of the DispatcherStorage
		/// </summary>
		public DispatcherStorage()
		{
			_dispatchers = new Dictionary<string, IDispatcher[]>();
			_ids = new List<string>();
			_currentIndex = -1;
		}

		/// <summary>
		/// Add a new set of <see cref="IDispatcher"/> for a given id
		/// </summary>
		/// <param name="id"></param>
		/// <param name="dispatchers"></param>
		public void Add(string id, IEnumerable<IDispatcher> dispatchers)
		{
			lock(_lockObject)
			{
				if (_dispatchers.ContainsKey(id))
				{
					Remove(id);
				}

				_dispatchers[id] = dispatchers.ToArray();
				_ids.Add(id);
			}
		}

		/// <summary>
		/// Remove a set of <see cref="IDispatcher"/> from the storage
		/// </summary>
		/// <param name="id"></param>
		public void Remove(string id)
		{
			lock(_lockObject)
			{
				if (!_dispatchers.ContainsKey(id))
				{
					return;
				}

				_dispatchers.Remove(id);
				_ids.Remove(id);
			}
		}

		/// <summary>
		/// Get the next set of <see cref="IDispatcher"/> using a round robin selection
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IDispatcher> GetNext()
		{
			if (!_dispatchers.Any())
			{
				return Enumerable.Empty<IDispatcher>();
			}

			lock (_lockObject)
			{
				_currentIndex += 1;
				if (_currentIndex >= _ids.Count)
				{
					_currentIndex = 0;
				}

				return _dispatchers[_ids[_currentIndex]];
			}
		}

		/// <summary>
		/// Gets the number of registered sets of <see cref="IDispatcher"/>
		/// </summary>
		/// <returns></returns>
		public int Count()
		{
			return _ids.Count();
		}
	}
}
