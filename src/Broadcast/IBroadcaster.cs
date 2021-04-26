using System;
using Broadcast.EventSourcing;
using Broadcast.Processing;

namespace Broadcast
{
	/// <summary>
	/// Interface for the Broadcaster
	/// </summary>
    public interface IBroadcaster : IDisposable
    {
		/// <summary>
		/// Gets the <see cref="IScheduler"/>
		/// </summary>
		IScheduler Scheduler { get; }

		/// <summary>
		/// Gets the <see cref="ITaskProcessor"/>
		/// </summary>
		ITaskProcessor Processor { get; }

		ITaskStore Store { get; }

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
