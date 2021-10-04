using System;
using Broadcast.Configuration;
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

		/// <summary>
		/// Gets the <see cref="ITaskStore"/>
		/// </summary>
		ITaskStore Store { get; }

		/// <summary>
		/// Gets the name of the instance. This is equal to the <see cref="Options.ServerName"/>
		/// </summary>
		string Name { get; }

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
