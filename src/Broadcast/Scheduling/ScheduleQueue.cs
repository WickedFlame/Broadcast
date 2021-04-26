using System.Collections.Generic;
using System.Linq;

namespace Broadcast.Scheduling
{
	/// <summary>
	/// Threadsafe Queue for managing <see cref="SchedulerTask"/>
	/// </summary>
	public class ScheduleQueue : IScheduleQueue
	{
		private readonly object _lockHandle = new object();
		private readonly List<SchedulerTask> _queue;

		/// <summary>
		/// Creates a new instance of the ScheduleQueue
		/// </summary>
		public ScheduleQueue()
		{
			_queue = new List<SchedulerTask>();
		}

		/// <summary>
		/// Adds a new task to the queue
		/// </summary>
		/// <param name="task"></param>
		public void Enqueue(SchedulerTask task)
		{
			lock (_lockHandle)
			{
				_queue.Add(task);
			}
		}

		/// <summary>
		/// Removes the task from the schedule queue
		/// </summary>
		/// <param name="task">The task to remove</param>
		public void Dequeue(SchedulerTask task)
		{
			lock (_lockHandle)
			{
				_queue.Remove(task);
			}
		}

		/// <summary>
		/// Creates a copy of the queue
		/// </summary>
		/// <returns></returns>
		public IEnumerable<SchedulerTask> ToList()
		{
			lock (_lockHandle)
			{
				if (!_queue.Any())
				{
					return Enumerable.Empty<SchedulerTask>();
				}

				return _queue.ToList();
			}
		}
	}
}
