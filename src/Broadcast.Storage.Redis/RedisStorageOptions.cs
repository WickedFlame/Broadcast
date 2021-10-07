using System;
using System.Collections.Generic;
using System.Text;

namespace Broadcast.Storage.Redis
{
	public class RedisStorageOptions
	{
		public int Db { get; set; }

		public string KeySpacePrefix { get; set; } = "{broadcast}";
	}
}
