using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.EventSourcing;

namespace Broadcast
{
	public class TaskQueue
	{
		private readonly object _syncRoot = new object();
		private readonly Queue<ITask> _queue;

		/// <summary>
		/// creates a new queue
		/// </summary>
		public TaskQueue()
		{
			_queue = new Queue<ITask>();
		}

		/// <summary>
		/// gets the amount of items in the queue
		/// </summary>
		public int Count
		{
			get
			{
				lock (_syncRoot)
				{
					return _queue.Count;
				}
			}
		}

		/// <summary>
		/// enqueue a new event
		/// </summary>
		/// <param name="event"></param>
		public void Enqueue(ITask @event)
		{
			lock (_syncRoot)
			{
				_queue.Enqueue(@event);
			}
		}

		/// <summary>
		/// try to dequeue a event.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool TryDequeue(out ITask value)
		{
			lock (_syncRoot)
			{
				if (_queue.Count > 0)
				{
					value = _queue.Dequeue();

					return true;
				}

				value = default(ITask);

				return false;
			}
		}
	}
}
