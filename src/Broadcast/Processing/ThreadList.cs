using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Broadcast.Processing
{
	/// <summary>
	/// A list of running threads
	/// </summary>
	public class ThreadList
	{

		private readonly List<Task> _taskList = new List<Task>();

		/// <summary>
		/// Add a task to the threadlist
		/// </summary>
		/// <param name="task"></param>
		public void Add(Task task)
		{
			if (task == null)
			{
				throw new Exception();
			}

			lock(_taskList)
			{
				_taskList.Add(task);
				task.ContinueWith(t => Remove(t));
			}

			if (task.IsCompleted || task.IsFaulted || task.IsCanceled)
			{
				Remove(task);
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
					_taskList.Remove(task);
				}
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

				Task.WaitAll(GetTaskArray());

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

		private int Count()
		{
			lock (_taskList)
			{
				return _taskList.Count;
			}
		}
	}
}
