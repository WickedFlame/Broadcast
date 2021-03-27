using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Broadcast.Processing
{
	public class TaskList
	{

		private readonly List<Task> _taskList = new List<Task>();

		public void Add(Task task)
		{
			if (task == null)
			{
				throw new Exception();
			}

			_taskList.Add(task);
			task.ContinueWith(t => Remove(t));

			if (task.IsCompleted || task.IsFaulted || task.IsCanceled)
			{
				Remove(task);
			}
		}

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

		public void WaitAll()
		{
			Trace.WriteLine($"Task count before waitall: {_taskList.Count}");
			Task.WaitAll(_taskList.ToArray());
			Trace.WriteLine($"Task count after waitall: {_taskList.Count}");
			Trace.WriteLine($"Active Tasks: {_taskList.Count(t => t.Status == TaskStatus.Running)}");
		}
	}
}
