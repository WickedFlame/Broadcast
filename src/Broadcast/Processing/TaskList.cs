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
			_taskList.Add(task);
			task.ContinueWith(t => Remove(t));

			if (task.Status != TaskStatus.Running)
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
			Task.WaitAll(_taskList.ToArray());
			Trace.WriteLine($"Task count: {_taskList.Count}");
			Trace.WriteLine($"Active Tasks: {_taskList.Count(t => t.Status == TaskStatus.Running)}");
		}
	}
}
