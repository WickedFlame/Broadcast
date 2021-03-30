using System;
using Broadcast.EventSourcing;

namespace Broadcast
{
	/// <summary>
	/// Interface for the Broadcaster
	/// </summary>
    public interface IBroadcaster : IDisposable
    {
        /// <summary>
        /// Gets the ProcessorContext that containes all information create a TaskProcessor
        /// </summary>
        IProcessorContext Context { get; }

		/// <summary>
		/// Gets the Scheduler
		/// </summary>
		IScheduler Scheduler { get; }

		/// <summary>
		/// Process the task
		/// </summary>
		/// <param name="task"></param>
        void Process(ITask task);

        /// <summary>
		/// Wait for all threads to end
		/// </summary>
        void WaitAll();

		/// <summary>
		/// Get the TaskStore
		/// </summary>
		/// <returns></returns>
        ITaskStore GetStore();

		/// <summary>
		/// Register a INotificationTarget that gets called when a INotification of the same type is sent
		/// </summary>
		/// <typeparam name="T">The notification type</typeparam>
		/// <param name="target">The INotificationTarget that handles the INotification</param>
		void RegisterHandler<T>(INotificationTarget<T> target) where T : INotification;

        /// <summary>
        /// Register a delegate that gets called when a INotification of the same type is sent
        /// </summary>
        /// <typeparam name="T">The notification type</typeparam>
        /// <param name="target">The delegate that handles the INotification</param>
        void RegisterHandler<T>(Action<T> target) where T : INotification;
    }
}
