using Broadcast.Configuration;
using Broadcast.EventSourcing;
using Broadcast.Server;

namespace Broadcast.Processing
{
    /// <summary>
    /// Represents the Context that provides all elements needed by the TaskProcessor
    /// </summary>
    public interface IProcessorContext : IServerContext
    {
		/// <summary>
		/// Gets the <see cref="Options"/>
		/// </summary>
        ProcessorOptions Options { get; set; }

		/// <summary>
		/// Gets the <see cref="ITaskStore"/> associated with the <see cref="ITaskProcessor"/>
		/// </summary>
		ITaskStore Store { get; set; }
    }
}
