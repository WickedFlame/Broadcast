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
    }
}
