using System;

namespace Broadcast.EventSourcing
{
    public class TaskStoreFactory
    {
        static Func<ITaskStore> _storeFactory;
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

        public static ITaskStore GetStore()
        {
            if (_defaultStore == null)
                _defaultStore = StoreFactory();

            return _defaultStore;
        }
    }
}
