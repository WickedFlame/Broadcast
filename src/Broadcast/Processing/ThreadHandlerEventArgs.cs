using System;

namespace Broadcast.Processing
{
	/// <summary>
    /// EventArgs for the ThreadCounter
    /// </summary>
    public class ThreadHandlerEventArgs : EventArgs
    {
        /// <summary>
        /// Name of the <see cref="IThreadList"/>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Count of threads contained in the <see cref="IThreadList"/>
        /// </summary>
        public int Count { get; set; }
    }
}
