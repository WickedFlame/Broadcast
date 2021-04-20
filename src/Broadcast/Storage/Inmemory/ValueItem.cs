namespace Broadcast.Storage.Inmemory
{
	/// <summary>
	/// ValueItem for the inmemory storage
	/// </summary>
	public class ValueItem : IStorageItem
	{
		/// <summary>
		/// Creates a new instance of the ValueItem
		/// </summary>
		public ValueItem()
		{
		}

		/// <summary>
		/// Creates a new instance of the ValueItem
		/// </summary>
		/// <param name="value"></param>
		public ValueItem(object value)
		{
			Value = value;
		}

		/// <summary>
		/// Gets the value
		/// </summary>
		public object Value { get; private set; }

		/// <summary>
		/// Set a new value
		/// </summary>
		/// <param name="value"></param>
		public void SetValue(object value)
		{
			Value = value;
		}

		/// <summary>
		/// Get the value
		/// </summary>
		/// <returns></returns>
		public object GetValue()
		{
			return Value;
		}
	}
}
