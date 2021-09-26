
namespace Broadcast.Storage
{
	/// <summary>
	/// Property with key and value for the <see cref="DataObject"/>
	/// </summary>
	public class PropertyValue
	{
		/// <summary>
		/// Creates a new instance of the PropertyValue
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public PropertyValue(string key, object value)
		{
			Key = key;
			Value = value;
		}

		/// <summary>
		/// Gets or sets the key of the Property
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		/// Gets or sets the value of the property
		/// </summary>
		public object Value { get; set; }

		/// <summary>
		/// Gets the stringvalue representing the insance
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Key}: {Value}";
		}
	}
}
