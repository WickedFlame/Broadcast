using System;

namespace Broadcast.Configuration
{
    public class ProcessorOptions
    {
        /// <summary>
        /// Gets or sets the designated name of the <see cref="IBroadcaster"/> server.
        /// Each <see cref="IBroadcaster"/> gets an individual Id that is generated with each creation.
        /// </summary>
        public string ServerName { get; set; } = Environment.MachineName;

        /// <summary>
        /// Gets or set the milliseconds that the Hearbeat is propagated to the <see cref="IStorage"/>
        /// </summary>
        public int HeartbeatInterval { get; set; } = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;

        /// <summary>
        /// Gets or sets the <see cref="IActivationContext"/> for resolving objects
        /// </summary>
        public IActivationContext ActivationContext { get; set; } = new ActivationContext();
    }
}
