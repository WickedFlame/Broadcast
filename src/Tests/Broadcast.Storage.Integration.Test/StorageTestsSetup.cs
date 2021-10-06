using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Broadcast.Storage.Integration.Test
{
	public partial class StorageTests
	{
		[SetUp]
		public void Setup()
		{

		}

		public IStorage BuildStorage()
		{
			return new InmemoryStorage();
		}
	}
}
