using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Configuration;
using NUnit.Framework;

namespace Broadcast.Test.Configuration
{
	public class ItemFactoryTests
	{
		[Test]
		public void ItemFactory()
		{
			var factory = new ItemFactory<TestItem>(() => new TestItem());
			Assert.IsNotNull(factory.Factory());
		}

		[Test]
		public void ItemFactory_Singleton()
		{
			var factory = new ItemFactory<TestItem>(() => new TestItem());
			Assert.AreSame(factory.Factory(), factory.Factory());
		}

		[Test]
		public void ItemFactory_Overwrite()
		{
			var factory = new ItemFactory<TestItem>(() => new TestItem());
			var inital = factory.Factory();

			factory.Factory = () => new TestItem();

			Assert.AreNotSame(inital, factory.Factory());
		}

		[Test]
		public void ItemFactory_Overwrite_Singleton()
		{
			var factory = new ItemFactory<TestItem>(() => new TestItem());

			factory.Factory = () => new TestItem();

			Assert.AreSame(factory.Factory(), factory.Factory());
		}

		[Test]
		public void ItemFactory_Reset()
		{
			var factory = new ItemFactory<TestItem>(() => new TestItem());
			var inital = factory.Factory();

			factory.Factory = () => new TestItem();

			Assert.AreNotSame(inital, factory.Factory());

			factory.Factory = null;

			Assert.AreSame(inital, factory.Factory());
		}

		public class TestItem{}
	}
}
