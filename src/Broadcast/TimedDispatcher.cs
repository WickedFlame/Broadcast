namespace Broadcast
{
    public class TimedDispatcher : IDisposable
    {
        private readonly ManualResetEvent _waitHandle = new(false);
        private readonly int _timeout;
        private readonly Func<bool> _task;

        public TimedDispatcher(int timeout, Func<bool> task)
        {
            _timeout = timeout;
            _task = task;
        }

        public bool IsRunning { get; set; }

        public void StartDispatcher()
        {
            if (IsRunning)
            {
                return;
            }

            IsRunning = true;

            Task.Factory.StartNew(() =>
            {
                while (IsRunning)
                {
                    _waitHandle.Reset();

                    if (!_task())
                    {
                        continue;
                    }

                    _waitHandle.WaitOne(_timeout);
                }
            },
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        /// <summary>
        /// reset the WaitHandle to continue to the next execution
        /// </summary>
        public void Continue()
        {
            _waitHandle.Reset();
        }

        public void Close()
        {
            IsRunning = false;
            _waitHandle.Reset();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }
    }
}
