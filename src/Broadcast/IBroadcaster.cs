using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Broadcast.EventSourcing;

namespace Broadcast
{
    public interface IBroadcaster : IDisposable
    {
        /// <summary>
        /// Gets the ProcessorContext that containes all information create a TaskProcessor
        /// </summary>
        IProcessorContext Context { get; }

        void Process(ITask task);

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
