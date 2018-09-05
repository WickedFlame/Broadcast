using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Broadcast
{
    public interface IScheduledBroadcast : IDisposable
    {
        /// <summary>
        /// Schedules a new task. The task will be executed at the time passed
        /// </summary>
        /// <param name="task">The task to execute</param>
        /// <param name="time">The time to execute the task at</param>
        void Schedule(Action task, TimeSpan time);

        /// <summary>
        /// Creates and schedules a new task that will recurr at the given interval
        /// </summary>
        /// <param name="task">The task to execute</param>
        /// <param name="time">The interval time to execute the task at</param>
        void Recurring(Action task, TimeSpan time);

        /// <summary>
        /// Schedules a INotification that is sent to the processor. The INotification will be passed to all registered Handlers of the same type
        /// </summary>
        /// <typeparam name="T">The notification type</typeparam>
        /// <param name="notification">The delegate returning the notification that will be processed and passed to the handlers</param>
        /// <param name="time">The interval time to execute the task at</param>
        void Schedule<T>(Func<T> notification, TimeSpan time) where T : INotification;

        /// <summary>
        /// Schedules a recurring INotification task that is sent to the processor. The INotification will be passed to all registered Handlers of the same type
        /// </summary>
        /// <typeparam name="T">The notification type</typeparam>
        /// <param name="notification">The delegate returning the notification that will be processed and passed to the handlers</param>
        /// <param name="time">The interval time to execute the task at</param>
        void Recurring<T>(Func<T> notification, TimeSpan time) where T : INotification;
    }
}
