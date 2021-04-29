using Broadcast.Configuration;
using Broadcast.Server;

namespace Broadcast.Processing
{
    /// <summary>
    /// Represents the Context that provides all elements needed by the TaskProcessor
    /// </summary>
    public interface IProcessorContext : IServerContext
    {
	    Options Options { get; set; }
    }
}
