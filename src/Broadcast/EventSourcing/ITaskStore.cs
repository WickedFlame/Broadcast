using System.Collections.Generic;

namespace Broadcast.EventSourcing
{
    /// <summary>
    /// Represents a store conatining all Tasks
    /// </summary>
    public interface ITaskStore : IEnumerable<WorkerTask>
    {
        /// <summary>
        /// Copies all unprocessed Tasks that are contained in the Queued to ne new List
        /// </summary>
        /// <returns></returns>
        IEnumerable<WorkerTask> CopyQueue();

        /// <summary>
        /// Counts all unprocessed Tasks that are contained in the Queue
        /// </summary>
        int CountQueue { get; }

        /// <summary>
        /// Adds a new Task to the queue to be processed
        /// </summary>
        /// <param name="task"></param>
        void Add(WorkerTask task);

        /// <summary>
        /// Sets tha task to InProcess mode
        /// </summary>
        /// <param name="task"></param>
        void SetInprocess(WorkerTask task);

        /// <summary>
        /// Sets the task to Processed mode and removes it from the process queue
        /// </summary>
        /// <param name="task"></param>
        void SetProcessed(WorkerTask task);
    }
}
