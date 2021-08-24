using System;
using System.Collections.Generic;
using System.Text;
using Broadcast.Composition;
using Broadcast.EventSourcing;
using Broadcast.Storage;
using Broadcast.Storage.Serialization;
using NUnit.Framework;
using Polaroider;

namespace Broadcast.Test.Storage.Serialization
{
	public class SerializerExtensionsTests
	{
		[Test]
		public void SerializerExtensions_Serialize_BroadcastTask()
		{
			var task = TaskFactory.CreateTask(() => System.Diagnostics.Trace.WriteLine("test"));
			var hash = task.Serialize();

			hash.MatchSnapshot(SnapshotOptions.Create(o => o.AddDirective(d => d.ReplaceDateTime()).AddDirective(d => d.ReplaceGuid())));
		}

		[Test]
		public void SerializerExtensions_Serialize_Object()
		{
			var model = new SerializerExtensionsTestModel
			{
				Id = 1,
				Name = "SerializerExtensions"
			};

			var hash = model.Serialize();

			hash.MatchSnapshot();
		}

		[Test]
		public void SerializerExtensions_Serialize_String()
		{
			var hash = "SerializerExtensions".Serialize();

			hash.MatchSnapshot();
		}

		[Test]
		public void SerializerExtensions_Deserialize_BroadcastTask()
		{
			var hash = new List<HashValue>
			{
				new HashValue("Id", "id"),
				new HashValue("Name", "name"),
				new HashValue("State", TaskState.New.ToString()),
				new HashValue("Type", "System.Diagnostics.Trace, System.Diagnostics.TraceSource"),
				new HashValue("IsRecurring", false.ToString()),
				new HashValue("Time", TimeSpan.FromSeconds(1).ToString()),
				new HashValue("StateChanges:New", DateTime.Now.ToString("o")),
				new HashValue($"ArgsType:0", "System.String, System.Private.CoreLib"),
				new HashValue($"ArgsValue:0", "test")
			};

			var value = hash.Deserialize<BroadcastTask>();

			value.MatchSnapshot(SnapshotOptions.Create(o => o.MockDateTimes().AddDirective(d => d.ReplaceGuid())));
		}

		[Test]
		public void SerializerExtensions_Deserialize_DataObject()
		{
			var hash = new List<HashValue>
			{
				new HashValue("Id", "1"),
				new HashValue("Name", "name")
			};

			var value = hash.Deserialize<DataObject>();

			Assert.AreEqual(1, value["Id"]);
			Assert.AreEqual("name", value["Name"]);
		}

		[Test]
		public void SerializerExtensions_Deserialize_Object()
		{
			var hash = new List<HashValue>
			{
				new HashValue("Id", "1"),
				new HashValue("Name", "name")
			};

			var value = hash.Deserialize<SerializerExtensionsTestModel>();

			value.MatchSnapshot();
		}

		[Test]
		public void SerializerExtensions_Deserialize_String()
		{
			var hash = new List<HashValue>
			{
				new HashValue("String", "string value")
			};

			var value = hash.Deserialize<string>();

			Assert.AreEqual("string value", value);
		}

		public class SerializerExtensionsTestModel
		{
			public string Name { get; set; }

			public int Id { get; set; }
		}
	}
}
