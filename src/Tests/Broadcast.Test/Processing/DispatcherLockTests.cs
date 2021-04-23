using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Processing;
using NUnit.Framework;

namespace Broadcast.Test.Processing
{
	public class DispatcherLockTests
	{
		[Test]
		public void DispatcherLock_ctor()
		{
			Assert.DoesNotThrow(() => new DispatcherLock());
		}

		[Test]
		public void DispatcherLock_Default()
		{
			var locker = new DispatcherLock();
			
			Assert.IsFalse(locker.IsLocked());
		}

		[Test]
		public void DispatcherLock_Lock()
		{
			var locker = new DispatcherLock();
			locker.Lock();

			Assert.IsTrue(locker.IsLocked());
		}

		[Test]
		public void DispatcherLock_Unlock()
		{
			var locker = new DispatcherLock();
			locker.Unlock();

			Assert.IsFalse(locker.IsLocked());
		}

		[Test]
		public void DispatcherLock_Unlock_AfterLock()
		{
			var locker = new DispatcherLock();
			locker.Lock();
			locker.Unlock();

			Assert.IsFalse(locker.IsLocked());
		}
	}
}
