using System.Collections.Generic;

namespace Broadcast.Dashboard.Dispatchers.Models
{
	/// <summary>
	/// A group of properties
	/// </summary>
	public class StoragePropertyGroup
	{
		/// <summary>
		/// Gets the title of the group
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Gets the properties of the group
		/// </summary>
		public IEnumerable<StorageProperty> Values { get; set; }
	}
}
