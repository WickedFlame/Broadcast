using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Configuration;
using Broadcast.EventSourcing;
using Broadcast.Setup;
using NUnit.Framework;

namespace Broadcast.Test.Setup
{
	public class ServerSetupExtensionsTests
	{
		[Test]
		public void ServerSetup_UseTaskStore()
		{
			var store = new TaskStore();

			var setup = new ServerSetup();
			setup.UseTaskStore(store);

			Assert.AreSame(store, setup.Context[nameof(ITaskStore)]);
		}

		[Test]
		public void ServerSetup_UseTaskStore_ContainsKey()
		{
			var setup = new ServerSetup();
			setup.UseTaskStore(new TaskStore());

			Assert.IsTrue(setup.Context.ContainsKey(nameof(ITaskStore)));
		}

		[Test]
		public void ServerSetup_UseOptions()
		{
			var options = new ProcessorOptions();

			var setup = new ServerSetup();
			setup.UseOptions(options);

			Assert.AreSame(options, setup.Context[nameof(ProcessorOptions)]);
		}

		[Test]
		public void ServerSetup_UseOptions_ContainsKey()
		{
			var setup = new ServerSetup();
			setup.UseOptions(new ProcessorOptions());

			Assert.IsTrue(setup.Context.ContainsKey(nameof(ProcessorOptions)));
		}

		[Test]
		public void ServerSetup_Register()
		{
			var item = new SetupItem();

			var setup = new ServerSetup();
			setup.Register(item);

			Assert.AreSame(item, setup.Context[nameof(SetupItem)]);
		}

		[Test]
		public void ServerSetup_Register_ContainsKey()
		{
			var setup = new ServerSetup();
			setup.Register(new SetupItem());

			Assert.IsTrue(setup.Context.ContainsKey(nameof(SetupItem)));
		}

		[Test]
		public void ServerSetup_Resolve()
		{
			var item = new SetupItem();

			var setup = new ServerSetup();
			setup.Context.Add(nameof(SetupItem), item);

			Assert.AreSame(item, setup.Resolve<SetupItem>());
		}

		[Test]
		public void ServerSetup_Resolve_Unregistered()
		{
			var setup = new ServerSetup();

			Assert.IsNull(setup.Resolve<SetupItem>());
		}

		[Test]
		public void ServerSetup_Resolve_InvalidTypeRegistration()
		{
			// This should not happen because register always uses the correct type to register
			var setup = new ServerSetup();
			setup.Context.Add(nameof(ServerSetup), new SetupItem());

			Assert.Throws<InvalidCastException>(() => setup.Resolve<ServerSetup>());
		}

		public class SetupItem
		{
		}
	}
}
