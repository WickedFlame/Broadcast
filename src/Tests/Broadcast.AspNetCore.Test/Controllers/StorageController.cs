using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Broadcast.EventSourcing;
using Broadcast.Server;
using Broadcast.Storage;
using Broadcast.Storage.Inmemory;
using Microsoft.AspNetCore.Mvc;

namespace Broadcast.AspNetCore.Test.Controllers
{
	public class StorageController : Controller
	{
		private readonly ITaskStore _store;

		public StorageController(ITaskStore store)
		{
			_store = store;
		}

		public IActionResult Index()
		{
			var model = new StorageModel
			{
				
			};

			_store.Storage(s =>
			{
				model.Items = new List<StorageType>();



				// {broadcast}:tasks:recurring:

				// lists
				// {broadcast}:tasks:dequeued
				// {broadcast}:tasks:enqueued

				
				model.Items.Add(GetData("Servers", "{broadcast}:server:", s));
				model.Items.Add(GetData("Tasks", "{broadcast}:task:", s));
				model.Items.Add(GetData("Recurring tasks", "{broadcast}:tasks:recurring:", s));

				var storageType = new StorageType
				{
					Key = "Processing"
				};
				storageType.Items.Add(GetList("{broadcast}:tasks:dequeued", s));
				storageType.Items.Add(GetList("{broadcast}:tasks:enqueued", s));
				model.Items.Add(storageType);
			});

			return View(model);
		}

		private StorageType GetData(string type, string storeKey, IStorage store)
		{
			var storageType = new StorageType
			{
				Key = type
			};

			foreach (var key in store.GetKeys(new Storage.StorageKey(storeKey)).ToList())
			{
				var data = store.Get<DataObject>(new StorageKey(key));

				var item = new StorageItem
				{
					Key = key,
					Values = data.Select(d => new StorageProperty(d.Key, d.Value))
				};

				storageType.Items.Add(item);
			}

			return storageType;
		}

		public StorageItem GetList(string storeKey, IStorage store)
		{
			var data = store.GetList(new StorageKey(storeKey));

			var item = new StorageItem
			{
				Key = storeKey,
				Values = data.Select(d => new StorageProperty("", d))
			};

			return item;
		}
	}

	public class StorageModel
	{
		public List<StorageType> Items { get; set; }
	}

	public class StorageType
	{
		public string Key{ get; set; }

		public List<StorageItem> Items { get; set; } = new List<StorageItem>();
	}

	public class StorageItem
	{
		public string Key { get; set; }

		public IEnumerable<StorageProperty> Values { get; set; }
	}

	public class StorageProperty
	{
		public StorageProperty(string key, object value)  
			: this(key, value.ToString())
		{
		}

		public StorageProperty(string key, string value)
		{
			Key = key;
			Value = value;
		}

		public string Key { get; set; }

		public string Value { get; set; }
	}
}
