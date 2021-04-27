﻿using System;
using System.Collections.Generic;
using Broadcast.Storage;

namespace Broadcast.EventSourcing
{
    /// <summary>
    /// Represents a store conatining all Tasks
    /// </summary>
    public interface ITaskStore : IEnumerable<ITask>
    {
        /// <summary>
        /// Adds a new Task to the queue to be processed
        /// </summary>
        /// <param name="task"></param>
        void Add(ITask task);

		/// <summary>
		/// Register a set of <see cref="IDispatcher"/> to the TaskStore.
		/// Dispatchers are executed when a new Task is added to the TaskStore to notify clients of the changes.
		/// All previously registered <see cref="IDispatcher"/> will be removed.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="dispatchers"></param>
		void RegisterDispatchers(string id, IEnumerable<IDispatcher> dispatchers);

		/// <summary>
		/// Unregister all <see cref="IDispatcher"/> that are registered for the given key
		/// </summary>
		/// <param name="id"></param>
		void UnregisterDispatchers(string id);

		/// <summary>
		/// Clear all Tasks from the TaskStore
		/// </summary>
		void Clear();

		/// <summary>
		/// Gets the <see cref="IStorage"/> registered to the <see cref="ITaskStore"/>
		/// </summary>
		/// <returns></returns>
		void Storage(Action<IStorage> action);

    }
}
