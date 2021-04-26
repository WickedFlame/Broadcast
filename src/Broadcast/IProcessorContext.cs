using Broadcast.EventSourcing;
using System.Collections.Generic;
using Broadcast.Configuration;
using Broadcast.Server;

namespace Broadcast
{
    /// <summary>
    /// Represents the Context that provides all elements needed by the TaskProcessor
    /// </summary>
    public interface IProcessorContext : IServerContext
    {
	    Options Options { get; set; }
		
        /// <summary>
        /// Gets the store of the NotificationHandlers
        /// </summary>
        INotificationHandlerStore NotificationHandlers { get; }
    }
}
