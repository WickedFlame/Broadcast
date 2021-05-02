using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Broadcast.EventSourcing;
using Broadcast.Storage.Inmemory;
using Microsoft.AspNetCore.Mvc;

namespace Broadcast.AspNet.Test.Controllers
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
				model.Items = new List<StorageItem>();

				var keys = s.GetKeys(new Storage.StorageKey("")).ToList();
				foreach (var key in keys)
				{
					var item = s.Get<object>(new Storage.StorageKey(key));
					if (item != null)
					{
						if (item is ITask task)
						{
							var itm = new
							{
								task.Id,
								task.Name,
								task.IsRecurring,
								task.State,
								task.StateChanges,
								task.Time
							};
							model.Items.Add(new StorageItem { Key = key, Value = Newtonsoft.Json.JsonConvert.SerializeObject(itm) });
						}
						else
						{
							model.Items.Add(new StorageItem {Key = key, Value = Newtonsoft.Json.JsonConvert.SerializeObject(item)});
						}

						continue;
					}

					var items = s.GetList<object>(new Storage.StorageKey(key));
					if (items != null)
					{
						model.Items.Add(new StorageItem {Key = key, Value = JsonSerializer.Serialize(items)});
						continue;
					}
				}
			});

			return View(model);
		}
	}

	public class StorageModel
	{
		public List<StorageItem> Items { get; set; }
	}

	public class StorageItem
	{
		public string Key { get; set; }

		public string Value{ get; set; }
	}
}
