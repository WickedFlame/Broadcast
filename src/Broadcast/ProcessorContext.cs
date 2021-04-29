using Broadcast.Configuration;
using Broadcast.Processing;
using Broadcast.Server;

namespace Broadcast
{
    /// <summary>
    /// Represents the Context that provides all elements needed by the TaskProcessor
    /// </summary>
    public class ProcessorContext : IProcessorContext, IServerContext
    {
		/// <summary>
		/// Creates a new instance of the ProcessorContext
		/// </summary>
        public ProcessorContext()
        {
            Options = Options.Default;
        }

		/// <summary>
		/// Gets or sets the <see cref="Options"/>
		/// </summary>
        public Options Options { get; set; }
    }
}
