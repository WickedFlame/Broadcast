using Broadcast.EventSourcing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Broadcast.Storage
{
	/// <summary>
	/// Object for storing data from a <see cref="ITask"/>
	/// </summary>
	public class DataObject : IEnumerable<PropertyValue>
	{
		private readonly List<PropertyValue> _properties;

		/// <summary>
		/// Creates a new instance of the DataObject
		/// </summary>
		public DataObject()
		{
			_properties = new List<PropertyValue>();
		}

		/// <summary>
		/// Add a new item to the DataObject
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void Add(string key, object value)
		{
			_properties.Add(new PropertyValue(key, value));
		}

		/// <summary>
		/// Gets or sets a value associated with the key.
		/// If the key is already contained it will be overwiten with the new value
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public object this[string key]
		{
			get
			{
				return _properties.FirstOrDefault(p => p.Key == key)?.Value;
			}
			set
			{
				var exist = _properties.FirstOrDefault(p => p.Key == key);
				if (exist != null)
				{
					_properties.Remove(exist);
				}

				_properties.Add(new PropertyValue(key, value));
			}
		}

		/// <summary>
		/// Gets the enumerator of the collection
		/// </summary>
		/// <returns></returns>
		public IEnumerator<PropertyValue> GetEnumerator()
		{
			return _properties.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
