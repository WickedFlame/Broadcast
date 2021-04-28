using System.Collections.Generic;
using System.Linq;

namespace Broadcast.Storage.Inmemory
{
	/// <summary>
	/// ListItem for the inmemory storage
	/// </summary>
	public class ListItem : IStorageItem
	{
		private readonly object _lockHandle = new object();

		/// <summary>
		/// Gets the list of items
		/// </summary>
		public List<IStorageItem> Items { get; } = new List<IStorageItem>();

		/// <summary>
		/// Add a new value to the list
		/// </summary>
		/// <param name="value"></param>
		public void SetValue(object value)
		{
			Set(new ValueItem(value));
		}

		/// <summary>
		/// Add a new <see cref="ValueItem"/> to the list
		/// </summary>
		/// <param name="item"></param>
		public void Set(ValueItem item)
		{
			lock (_lockHandle)
			{
				Items.Add(item);
			}
		}

		/// <summary>
		/// Get the values from the list
		/// </summary>
		/// <returns></returns>
		public object GetValue()
		{
			lock(_lockHandle)
			{
				return Items.Select(s => s.GetValue()).ToList();
			}
		}
	}
}
