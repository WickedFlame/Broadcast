using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Broadcast.Composition;
using Broadcast.EventSourcing;
using Broadcast.Storage.Serialization;
using NUnit.Framework;
using Polaroider;

namespace Broadcast.Test.Storage.Serialization
{
	public class BroadcastTaskSerializerTests
	{
		[Test]
		public void BroadcastTaskSerializer_ctor()
		{
			Assert.DoesNotThrow(() => new BroadcastTaskSerializer());
		}

		[Test]
		public void BroadcastTaskSerializer_Serialize()
		{
			var task = TaskFactory.CreateTask(() => System.Diagnostics.Trace.WriteLine("test")) as BroadcastTask;
			var serializer = new BroadcastTaskSerializer();
			var serialized = serializer.Serialize(task);

			serialized.MatchSnapshot(SnapshotOptions.Create(o => o.AddDirective(d => d.ReplaceDateTime()).AddDirective(d => d.ReplaceGuid())));
		}

		[Test]
		public void BroadcastTaskSerializer_Serialize_WrongObject()
		{
			var serializer = new BroadcastTaskSerializer();
			Assert.IsNull(serializer.Serialize(new object()));
		}

		[Test]
		public void BroadcastTaskSerializer_Deserialize()
		{
			var task = TaskFactory.CreateTask(() => System.Diagnostics.Trace.WriteLine("test")) as BroadcastTask;

			var hash = new List<HashValue>
			{
				new HashValue("Id", "id"),
				new HashValue("Name", "name"),
				new HashValue("State", TaskState.New.ToString()),
				new HashValue("Type", $"{task.Type.FullName}, {task.Type.Assembly.GetName().Name}"),
				new HashValue("IsRecurring", false.ToString()),
				new HashValue("Time", TimeSpan.FromSeconds(1).ToString()),
				new HashValue("StateChanges:New", DateTime.Now.ToString("o")),
				new HashValue("Method", "WriteLine"),
				new HashValue($"ArgsType:0", $"{typeof(string).FullName}, {typeof(string).Assembly.GetName().Name}"),
				new HashValue($"ArgsValue:0", "test")
			};

			var serializer = new BroadcastTaskSerializer();
			var serialized = serializer.Deserialize<BroadcastTask>(hash);

			serialized.MatchSnapshot(SnapshotOptions.Create(o => o.MockDateTimes()));
		}

		[Test]
		public void BroadcastTaskSerializer_Deserialize_NoType()
		{
			var task = TaskFactory.CreateTask(() => System.Diagnostics.Trace.WriteLine("test")) as BroadcastTask;

			var hash = new List<HashValue>
			{
				new HashValue("Id", "id"),
				new HashValue("Name", "name"),
				new HashValue("State", TaskState.New.ToString()),
				new HashValue("IsRecurring", false.ToString()),
				new HashValue("Time", TimeSpan.FromSeconds(1).ToString()),
				new HashValue("StateChanges:New", DateTime.Now.ToString("o")),
				new HashValue("Method", "WriteLine"),
				new HashValue($"ArgsType:0", $"{typeof(string).FullName}, {typeof(string).Assembly.GetName().Name}"),
				new HashValue($"ArgsValue:0", "test")
			};

			var serializer = new BroadcastTaskSerializer();
			Assert.Throws<InvalidOperationException>(() => serializer.Deserialize<BroadcastTask>(hash));
		}

		[Test]
		public void BroadcastTaskSerializer_Deserialize_NoMethod()
		{
			var task = TaskFactory.CreateTask(() => System.Diagnostics.Trace.WriteLine("test")) as BroadcastTask;

			var hash = new List<HashValue>
			{
				new HashValue("Id", "id"),
				new HashValue("Name", "name"),
				new HashValue("State", TaskState.New.ToString()),
				new HashValue("Type", $"{task.Type.FullName}, {task.Type.Assembly.GetName().Name}"),
				new HashValue("IsRecurring", false.ToString()),
				new HashValue("Time", TimeSpan.FromSeconds(1).ToString()),
				new HashValue("StateChanges:New", DateTime.Now.ToString("o")),
				new HashValue($"ArgsType:0", $"{typeof(string).FullName}, {typeof(string).Assembly.GetName().Name}"),
				new HashValue($"ArgsValue:0", "test")
			};

			var serializer = new BroadcastTaskSerializer();
			Assert.Throws<InvalidOperationException>(() => serializer.Deserialize<BroadcastTask>(hash));
		}

		[Test]
		public void BroadcastTaskSerializer_Deserialize_EmptyValues()
		{
			var task = TaskFactory.CreateTask(() => System.Diagnostics.Trace.WriteLine("test")) as BroadcastTask;

			var hash = new List<HashValue>();

			var serializer = new BroadcastTaskSerializer();
			Assert.Throws<ArgumentException>(() => serializer.Deserialize<BroadcastTask>(hash));
		}

		[Test]
		public void BroadcastTaskSerializer_Deserialize_InvalidType()
		{
			var task = TaskFactory.CreateTask(() => System.Diagnostics.Trace.WriteLine("test")) as BroadcastTask;

			var hash = new List<HashValue>
			{
				new HashValue("Id", "id"),
				new HashValue("Name", "name"),
				new HashValue("State", TaskState.New.ToString()),
				new HashValue("Type", $"{task.Type.FullName}, {task.Type.Assembly.GetName().Name}"),
				new HashValue("IsRecurring", false.ToString()),
				new HashValue("Time", TimeSpan.FromSeconds(1).ToString()),
				new HashValue("StateChanges:New", DateTime.Now.ToString("o")),
				new HashValue("Method", "WriteLine"),
				new HashValue($"ArgsType:0", $"{typeof(string).FullName}, {typeof(string).Assembly.GetName().Name}"),
				new HashValue($"ArgsValue:0", "test")
			};

			var serializer = new BroadcastTaskSerializer();
			Assert.IsNull(serializer.Deserialize<BroadcastTaskSerializer>(hash));
		}
	}
}
