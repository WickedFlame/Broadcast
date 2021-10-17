using System.Collections.Generic;

namespace Broadcast.Dashboard.Dispatchers.Models
{
	/// <summary>
	/// 
	/// </summary>
	public class StorageType
	{
		/// <summary>
		/// Key of the storage
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		/// Items contained in the Key
		/// </summary>
		public List<StorageItem> Items { get; set; } = new List<StorageItem>();
	}
}
