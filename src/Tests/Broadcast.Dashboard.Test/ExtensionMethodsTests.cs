using NUnit.Framework;

namespace Broadcast.Dashboard.Test
{
	public class ExtensionMethodsTests
	{
		[Test]
		public void ExtensionMethods_EnsureTrailingSlash()
		{
			Assert.AreEqual("test/".EnsureTrailingSlash(), "test/");
		}

		[Test]
		public void ExtensionMethods_EnsureTrailingSlash_NoSlash()
		{
			Assert.AreEqual("test".EnsureTrailingSlash(), "test/");
		}

		[Test]
		public void ExtensionMethods_EnsureTrailingSlash_LeadingSlash()
		{
			Assert.AreEqual("/test".EnsureTrailingSlash(), "/test/");
		}

		[Test]
		public void ExtensionMethods_EnsureTrailingSlash_Empty()
		{
			Assert.AreEqual("".EnsureTrailingSlash(), "");
		}

		[Test]
		public void ExtensionMethods_EnsureTrailingSlash_Null()
		{
			Assert.AreEqual(((string)null).EnsureTrailingSlash(), "");
		}

		[Test]
		public void ExtensionMethods_EnsureLeadingSlash()
		{
			Assert.AreEqual("/test".EnsureLeadingSlash(), "/test");
		}

		[Test]
		public void ExtensionMethods_EnsureLeadingSlash_NoSlash()
		{
			Assert.AreEqual("test".EnsureLeadingSlash(), "/test");
		}

		[Test]
		public void ExtensionMethods_EnsureLeadingSlash_TrailingSlash()
		{
			Assert.AreEqual("test/".EnsureLeadingSlash(), "/test/");
		}

		[Test]
		public void ExtensionMethods_EnsureLeadingSlashh_Empty()
		{
			Assert.AreEqual("".EnsureLeadingSlash(), "");
		}

		[Test]
		public void ExtensionMethods_EnsureLeadingSlashh_Null()
		{
			Assert.AreEqual(((string)null).EnsureLeadingSlash(), "");
		}

		[Test]
		public void ExtensionMethods_RemoveLeadingSlash()
		{
			Assert.AreEqual("/test".RemoveLeadingSlash(), "test");
		}

		[Test]
		public void ExtensionMethods_RemoveLeadingSlash_NoSlash()
		{
			Assert.AreEqual("test".RemoveLeadingSlash(), "test");
		}

		[Test]
		public void ExtensionMethods_RemoveLeadingSlash_TrailingSlash()
		{
			Assert.AreEqual("/test/".RemoveLeadingSlash(), "test/");
		}
	}
}