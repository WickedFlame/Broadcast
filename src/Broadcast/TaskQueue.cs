using System;
using System.Collections.Generic;
using System.Threading;
using Broadcast.EventSourcing;

namespace Broadcast
{
	/// <summary>
	/// Queue for ITasks
	/// </summary>
	public class TaskQueue : ITaskQueue
	{
		private readonly object _syncRoot = new object();
		private readonly Queue<ITask> _queue;
		private readonly CountdownEvent _counter;

		/// <summary>
		/// creates a new queue
		/// </summary>
		public TaskQueue()
		{
			_queue = new Queue<ITask>();
			_counter = new CountdownEvent(0);
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
				_counter.Reset(_queue.Count);
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
					_counter.Signal();

					return true;
				}

				value = default(ITask);
				_counter.Reset();

				return false;
			}
		}

		/// <summary>
		/// Wait for all tasks in the queue to be processed
		/// </summary>
		public void WaitAll()
		{
			while (Count > 0)
			{
				System.Diagnostics.Trace.WriteLine("Wait for TaskQueue");
				_counter.WaitHandle.WaitOne(50);
			}
		}
	}
}
