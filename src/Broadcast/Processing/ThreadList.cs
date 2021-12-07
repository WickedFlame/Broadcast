using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Broadcast.Processing
{
    /// <summary>
	/// A list of running threads
	/// </summary>
	public class ThreadList : IThreadList
    {
        private readonly List<Task> _taskList = new List<Task>();
        private readonly string _name;
        private EventHandler<ThreadHandlerEventArgs> _threadCountHandler;
        private readonly ThreadCounter _threadCounter;

		public ThreadList()
        {
            _name = Guid.NewGuid().ToString();
            _threadCounter = new ThreadCounter(this);
        }

		/// <summary>
		/// The Eventhandler used to notify when a thread is created or removed
		/// </summary>
        public event EventHandler<ThreadHandlerEventArgs> ThreadCountHandler
        {
            add => _threadCountHandler += value;
            remove => _threadCountHandler -= value;
        }

		/// <summary>
		/// Add a task to the threadlist
		/// </summary>
		/// <param name="task"></param>
		public void Add(Task task)
		{
			lock(_taskList)
			{
				_taskList.Add(task);
                _threadCountHandler?.Invoke(this, new ThreadHandlerEventArgs { Name = _name, Count = _taskList.Count });

				task.ContinueWith(t =>
				{
					// make sure the thread is completed
					t.ConfigureAwait(false).GetAwaiter().GetResult();
					Remove(t);
				});
			}
		}

		/// <summary>
		/// Remove a task from the threadlist
		/// </summary>
		/// <param name="task"></param>
		public void Remove(Task task)
		{
			lock(_taskList)
			{
				if (_taskList.Contains(task))
				{
					Trace.WriteLine($"Remove Thread with state: {task.Status}");
                    _taskList.Remove(task);
                }

                _threadCountHandler?.Invoke(this, new ThreadHandlerEventArgs { Name = _name, Count = _taskList.Count });
			}
		}

		/// <summary>
		/// Wait for all threads to end
		/// </summary>
		public void WaitAll()
		{
			while(Count() > 0)
			{
				Trace.WriteLine($"Task count before waitall: {Count()}");

				Task.WaitAll(GetTaskArray(), -1, CancellationToken.None);

				Trace.WriteLine($"Task count after waitall: {Count()}");
			}
		}

		private Task[] GetTaskArray()
		{
			lock (_taskList)
			{
				return _taskList.ToArray();
			}
		}

		/// <summary>
		/// Gets the count of threads in the list
		/// </summary>
		/// <returns></returns>
        public int Count()
		{
			lock (_taskList)
			{
				return _taskList.Count;
			}
		}
	}
}
