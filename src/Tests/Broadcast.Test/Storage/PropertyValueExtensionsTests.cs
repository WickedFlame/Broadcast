using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Storage;
using NUnit.Framework;

namespace Broadcast.Test.Storage
{
	public class PropertyValueExtensionsTests
	{
		[Test]
		public void PropertyValueExtensions_ToBool()
		{
			Assert.IsTrue(true.ToBool());
		}

		[Test]
		public void PropertyValueExtensions_ToBool_False()
		{
			Assert.IsFalse(false.ToBool());
		}

		[Test]
		public void PropertyValueExtensions_ToBool_Null()
		{
			Assert.IsFalse(((bool?)null).ToBool());
		}

		[Test]
		public void PropertyValueExtensions_ToBool_String_true()
		{
			Assert.IsTrue("true".ToBool());
		}

		[Test]
		public void PropertyValueExtensions_ToBool_String_1()
		{
			Assert.IsTrue("1".ToBool());
		}

		[Test]
		public void PropertyValueExtensions_ToBool_String_true_CaseInsensitive()
		{
			Assert.IsTrue("TRUE".ToBool());
		}

		[Test]
		public void PropertyValueExtensions_ToBool_String_Invalid()
		{
			Assert.IsFalse("invalid".ToBool());
		}

		[Test]
		public void PropertyValueExtensions_ToBool_Object_Invalid()
		{
			Assert.IsFalse(new object().ToBool());
		}
	}
}
