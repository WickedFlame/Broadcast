using System.Collections;
using System.Collections.Generic;

namespace Broadcast.EventSourcing
{
	/// <summary>
	/// The inmemorystorage for storing tasks in the local memory
	/// </summary>
	public class InmemoryStorage : ITaskStore, IEnumerable<ITask>
	{
		private readonly object _lockHandle = new object();

		readonly List<ITask> _store;

		/// <summary>
		/// Creates a new InmemoryStorage
		/// </summary>
		public InmemoryStorage()
		{
			_store = new List<ITask>();
		}

		/// <summary>
		/// Adds a new Task to the queue to be processed
		/// </summary>
		/// <param name="task"></param>
		public void Add(ITask task)
		{
			lock (_lockHandle)
			{
				_store.Add(task);
			}
		}

		/// <summary>
		/// Gets the enumerator
		/// </summary>
		/// <returns></returns>
		public IEnumerator<ITask> GetEnumerator()
		{
			return _store.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _store.GetEnumerator();
		}
	}
}
