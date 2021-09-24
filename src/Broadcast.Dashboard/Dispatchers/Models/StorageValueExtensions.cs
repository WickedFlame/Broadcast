using System;

namespace Broadcast.Dashboard.Dispatchers.Models
{
	/// <summary>
	/// Extensiopns for storage values
	/// </summary>
	public static class StorageValueExtensions
	{
		/// <summary>
		/// Tries to parse a object to represent a duration in milliseconds
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string ToDuration(this object value)
		{
			if (long.TryParse(value?.ToString(), out var duration))
			{
				return $"{TimeSpan.FromMilliseconds(duration).TotalSeconds} s";
			}

			return null;
		}

		/// <summary>
		/// Tries to format a object to a datetime with format 'yyyy/MM/dd hh:mm:ss.fff'
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string ToFormattedDateTime(this object value)
		{
			if (DateTime.TryParse(value?.ToString(), out var date))
			{
				return date.ToString("yyyy/MM/dd hh:mm:ss.fff");
			}

			return null;
		}
	}
}
