using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Dashboard.Dispatchers.Models;
using NUnit.Framework;

namespace Broadcast.Dashboard.Test
{
	public class StorageValueExtensionsTests
	{
		[Test]
		public void StorageValueExtensions_ToDuration()
		{
			var ms = "31".ToDuration();
			Assert.AreEqual("0.0031 ms", ms);
		}

		[Test]
		public void StorageValueExtensions_ToDuration_Int()
		{
			var ms = 31.ToDuration();
			Assert.AreEqual("0.0031 ms", ms);
		}

		[Test]
		public void StorageValueExtensions_ToDuration_Decimal()
		{
			var ms = (31.0).ToDuration();
			Assert.AreEqual("0.0031 ms", ms);
		}

		[Test]
		public void StorageValueExtensions_ToDuration_InvalidString()
		{
			var ms = "test".ToDuration();
			Assert.IsNull(ms);
		}

		[Test]
		public void StorageValueExtensions_ToDuration_InvalidObject()
		{
			var ms = new object().ToDuration();
			Assert.IsNull(ms);
		}

		[Test]
		public void StorageValueExtensions_ToDuration_Null()
		{
			var ms = ((object)null).ToDuration();
			Assert.IsNull(ms);
		}
		
		[Test]
		public void StorageValueExtensions_ToFormattedDateTime()
		{
			var date = "2021/12/21T12:12:12".ToFormattedDateTime();
			Assert.AreEqual("2021/12/21 12:12:12.000", date);
		}

		[Test]
		public void StorageValueExtensions_ToFormattedDateTime_DateTime()
		{
			var date = DateTime.Parse("2021/12/21T12:12:12").ToFormattedDateTime();
			Assert.AreEqual("2021/12/21 12:12:12.000", date);
		}


		[Test]
		public void StorageValueExtensions_ToFormattedDateTime_InvalidString()
		{
			var ms = "test".ToFormattedDateTime();
			Assert.IsNull(ms);
		}

		[Test]
		public void StorageValueExtensions_ToFormattedDateTime_InvalidObject()
		{
			var ms = new object().ToFormattedDateTime();
			Assert.IsNull(ms);
		}

		[Test]
		public void StorageValueExtensions_ToFormattedDateTime_Null()
		{
			var ms = ((object)null).ToFormattedDateTime();
			Assert.IsNull(ms);
		}
	}
}
