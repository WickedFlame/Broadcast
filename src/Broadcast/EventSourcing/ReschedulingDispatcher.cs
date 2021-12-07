﻿using System;
using Broadcast.Diagnostics;
using Broadcast.Storage;

namespace Broadcast.EventSourcing
{
	/// <summary>
	/// 
	/// </summary>
	public class ReschedulingDispatcher : IStorageObserver
	{
		/// <summary>
		/// Creates a new instance of a ReschedulingDispatcher
		/// </summary>
		public ReschedulingDispatcher()
		{
			var logger = LoggerFactory.Create();
			logger.Write($"Starting new ReschedulingDispatcher");
		}

		/// <summary>
		/// Execute the observer
		/// </summary>
		/// <param name="context"></param>
		public void Execute(ObserverContext context)
		{
            if (context.DispatcherLock.IsLocked())
            {
				return;
            }

			context.DispatcherLock.Lock();

			try
			{
				var dispatch = false;

				var dispatched = context.Store.Storage(s => s.GetFetchedTasks());
				foreach (var disp in dispatched)
				{
					var task = context.Store.Storage(s => s.Get<BroadcastTask>(new StorageKey($"task:{disp}")));
					if (task.State != TaskState.New)
					{
						// task is already processing
						continue;
					}

					// delay some time to ensure the task is not yet dispatched
					if (task.Time != null && task.CreatedAt + task.Time > DateTime.Now.Subtract(TimeSpan.FromMilliseconds(500)))
					{
						// task is not scheduled jet
						continue;
					}

					// move task to enqued list
					context.Store.Storage(s =>
                    {
                        if (!s.RemoveFromList(new StorageKey("tasks:dequeued"), disp))
                        {
                            return;
                        }

                        // delay rescheduling to prevent 2 servers that start the same time
                        // to process the task as soon as it is enqueued again
                        System.Threading.Tasks.Task.Delay(50).Wait();
                        s.AddToList(new StorageKey("tasks:enqueued"), disp);
                    });

					dispatch = true;

					//TODO: check if server is running
					//else remove the assigned serverid from the task
				}

				if (dispatch)
				{
					// notify the store to dispatch all tasks again
					context.Store.DispatchTasks();
				}

            }
            finally
            {
				context.DispatcherLock.Unlock();
            }
		}
	}
}
