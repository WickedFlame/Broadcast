using System.Threading;

namespace Broadcast.Processing
{
	/// <summary>
	/// Object used for locking across multiple threads
	/// </summary>
	public class DispatcherLock
	{
		private int _identifier = 0;

		/// <summary>
		/// Gets a boolean indicating if the object is in a locked state
		/// </summary>
		/// <returns></returns>
		public bool IsLocked()
		{
			return Interlocked.CompareExchange(ref _identifier, 1, 1) != 0;
		}

		/// <summary>
		/// Lock the object across multiple threads
		/// </summary>
		public void Lock()
		{
			Interlocked.Exchange(ref _identifier, 1);
		}

		/// <summary>
		/// Unlock the object
		/// </summary>
		public void Unlock()
		{
			if (_identifier < 0)
			{
				return;
			}

			Interlocked.Exchange(ref _identifier, 0);
		}
	}
}
