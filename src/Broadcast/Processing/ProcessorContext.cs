using Broadcast.Configuration;
using Broadcast.Server;

namespace Broadcast.Processing
{
    /// <summary>
    /// Represents the Context that provides all elements needed by the TaskProcessor
    /// </summary>
    public class ProcessorContext : IProcessorContext, IServerContext
    {
		/// <summary>
		/// Creates a new instance of the ProcessorContext
		/// </summary>
		/// <param name="store"></param>
		public ProcessorContext(ITaskStore store)
		    : this(store, new Options())
	    {
	    }

		/// <summary>
		/// Creates a new instance of the ProcessorContext
		/// </summary>
		/// <param name="store"></param>
		/// <param name="options"></param>
		public ProcessorContext(ITaskStore store, Options options)
        {
            Options = options;
			Store = store;
        }

		/// <summary>
		/// Gets or sets the <see cref="Options"/>
		/// </summary>
        public Options Options { get; set; }

		/// <summary>
		/// Gets the <see cref="ITaskStore"/> associated with the <see cref="ITaskProcessor"/>
		/// </summary>
		public ITaskStore Store { get; set; }
    }
}
