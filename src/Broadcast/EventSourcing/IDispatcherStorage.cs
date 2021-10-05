using System.Collections.Generic;

namespace Broadcast.EventSourcing
{
	/// <summary>
	/// A storage for managing <see cref="IDispatcher"/>
	/// <see cref="IDispatcher"/> are stored per Id of the <see cref="IBroadcaster"/>.
	/// Uses a round robin implementation to get the next set of <see cref="IDispatcher"/> for processing
	/// </summary>
	public interface IDispatcherStorage
	{
		/// <summary>
		/// Add a new set of <see cref="IDispatcher"/> for a given id
		/// </summary>
		/// <param name="id"></param>
		/// <param name="dispatchers"></param>
		void Add(string id, IEnumerable<IDispatcher> dispatchers);

		/// <summary>
		/// Remove a set of <see cref="IDispatcher"/> from the storage
		/// </summary>
		/// <param name="id"></param>
		void Remove(string id);

		/// <summary>
		/// Get the next set of <see cref="IDispatcher"/> using a round robin selection
		/// </summary>
		/// <returns></returns>
		IEnumerable<IDispatcher> GetNext();

		/// <summary>
		/// Get the set of <see cref="IDispatcher"/> that are registered for the server/queue
		/// </summary>
		/// <param name="id">The id of the server that the dispatchers are registered for</param>
		/// <returns></returns>
		IEnumerable<IDispatcher> GetNext(string id);

		/// <summary>
		/// Gets the number of registered sets of <see cref="IDispatcher"/>
		/// </summary>
		/// <returns></returns>
		int Count();
	}
}
