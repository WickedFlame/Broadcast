using System;

namespace Broadcast.EventSourcing
{
    /// <summary>
    /// Factory class that is used to create the default TaskStore
    /// </summary>
    public static class TaskStoreFactory
    {
        static Func<ITaskStore> _storeFactory;

        /// <summary>
        /// The delegate that creates the default TaskStore
        /// </summary>
        public static Func<ITaskStore> StoreFactory
        {
            get
            {
                if (_storeFactory == null)
                    _storeFactory = () => new TaskStore();
                return _storeFactory;
            }
            set
            {
                _storeFactory = value;
                _defaultStore = null;
            }
        }

        static ITaskStore _defaultStore;

        /// <summary>
        /// Gets the default TaskStore
        /// </summary>
        /// <returns></returns>
        public static ITaskStore GetStore()
        {
            if (_defaultStore == null)
                _defaultStore = StoreFactory();

            return _defaultStore;
        }
    }
}
