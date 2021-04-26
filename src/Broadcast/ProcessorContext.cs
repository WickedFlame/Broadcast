using Broadcast.EventSourcing;
using Broadcast.Configuration;
using Broadcast.Server;

namespace Broadcast
{
    /// <summary>
    /// Represents the Context that provides all elements needed by the TaskProcessor
    /// </summary>
    public class ProcessorContext : IProcessorContext, IServerContext
    {
        readonly INotificationHandlerStore _notificationHandlers;

		/// <summary>
		/// Creates a new instance of the ProcessorContext
		/// </summary>
        public ProcessorContext()
        {
            _notificationHandlers = new NotificationHandlerStore();
            Options = Options.Default;
        }

		/// <summary>
		/// Gets or sets the <see cref="Options"/>
		/// </summary>
        public Options Options { get; set; }

        /// <summary>
        /// Gets the store of the NotificationHandlers
        /// </summary>
        public INotificationHandlerStore NotificationHandlers => _notificationHandlers;
    }
}
