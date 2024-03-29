﻿
using System;

namespace Broadcast.Dashboard.Dispatchers.Models
{
	/// <summary>
	/// A property of a stored object with the value
	/// </summary>
	public class StorageProperty
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public StorageProperty(string key, object value)
			: this(key, Convert(value))
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public StorageProperty(string key, string value)
		{
			Key = key;
			Value = value;
		}

		/// <summary>
		/// Gets the name of the property
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		/// Gets the value of the property
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// Retruns the string value of the instance
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Key}: {Value}";
		}

		private static string Convert(object value)
		{
			if (value is DateTime dte)
			{
				return dte.ToString("o");
			}

			return value?.ToString();
		}
	}
}
