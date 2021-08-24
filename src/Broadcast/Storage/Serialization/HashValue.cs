
namespace Broadcast.Storage.Serialization
{
	/// <summary>
	/// A item that stores the key and value of a property of a serialized object
	/// </summary>
	public class HashValue
	{
		/// <summary>
		/// Creates a new HashValue that stores a value and its associated property name
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public HashValue(string name, string value)
		{
			Name = name;
			Value = value;
		}

		/// <summary>
		/// Gets the name of the serialized property
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets the value of the serialized property 
		/// </summary>
		public string Value { get; set; }
	}
}
