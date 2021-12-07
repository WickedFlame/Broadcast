using System;
using System.Threading;
using System.Threading.Tasks;
using Broadcast.Processing;

namespace Broadcast.Server
{
	/// <summary>
	/// BackgroundServerProcess is used to create dispatchers that run in backgroundthreads
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class BackgroundServerProcess<T> : IBackgroundServerProcess<T> where T : class, IServerContext
	{
		private readonly T _context;

		private readonly ThreadList _threadList = new ThreadList();

		/// <summary>
		/// Creates a new instance of the BackgroundServerProcess
		/// </summary>
		/// <param name="context"></param>
		public BackgroundServerProcess(T context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <summary>
		/// Start the <see cref="IBackgroundDispatcher{T}"/> in a new thread and run the Execute method
		/// </summary>
		/// <param name="dispatcher"></param>
		public void StartNew(IBackgroundDispatcher<T> dispatcher)
		{
			var thread = Task.Factory.StartNew(() => dispatcher.Execute(_context), 
                CancellationToken.None, 
                TaskCreationOptions.None, 
                TaskScheduler.Default);

			_threadList.Add(thread);
		}

		/// <summary>
		/// Wait for all threads in the serverprocess to end
		/// </summary>
		public void WaitAll()
		{
			_threadList.WaitAll();
		}
	}
}
