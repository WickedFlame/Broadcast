using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Broadcast.EventSourcing
{
	/// <summary>
	/// A storage for managing <see cref="IDispatcher"/>
	/// <see cref="IDispatcher"/> are stored per key or per <see cref="IBroadcaster"/>
	/// </summary>
	public class DispatcherStorage : IEnumerable<IDispatcher>
	{
		private readonly List<IDispatcher> _dispatchers;
		private readonly Dictionary<string, IDispatcher[]> _idref;

		/// <summary>
		/// Creates a new instance of the DispatcherStorage
		/// </summary>
		public DispatcherStorage()
		{
			_dispatchers = new List<IDispatcher>();
			_idref = new Dictionary<string, IDispatcher[]>();
		}

		/// <summary>
		/// Add a new set of <see cref="IDispatcher"/> for a given id
		/// </summary>
		/// <param name="id"></param>
		/// <param name="dispatchers"></param>
		public void Add(string id, IEnumerable<IDispatcher> dispatchers)
		{
			if (_idref.ContainsKey(id))
			{
				Remove(id);
			}

			_dispatchers.AddRange(dispatchers);
			_idref[id] = dispatchers.ToArray();
		}

		/// <summary>
		/// Remove a set of <see cref="IDispatcher"/> from the storage
		/// </summary>
		/// <param name="id"></param>
		public void Remove(string id)
		{
			if (!_idref.ContainsKey(id))
			{
				return;
			}

			foreach (var dispatcher in _idref[id])
			{
				_dispatchers.Remove(dispatcher);
			}

			_idref.Remove(id);
		}

		/// <summary>
		/// Gets the enumerator of the storage
		/// </summary>
		/// <returns></returns>
		public IEnumerator<IDispatcher> GetEnumerator()
		{
			return _dispatchers.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
