using System.Collections.Generic;

namespace Broadcast.Dashboard.Dispatchers.Models
{
	/// <summary>
	/// All data of a stored object
	/// </summary>
	public class StorageItem
	{
		/// <summary>
		/// Gets or sets the id of the stored object
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		/// Gets the title of the stored object
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// A list of all known properties and values of the object
		/// </summary>
		public IEnumerable<StorageProperty> Values { get; set; }

		/// <summary>
		/// All known properties and values grouped
		/// </summary>
		public IEnumerable<StoragePropertyGroup> Groups { get; set; }
	}
}
