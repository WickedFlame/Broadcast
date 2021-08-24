using System;
using System.Globalization;

namespace Broadcast.Storage.Serialization
{
	/// <summary>
	/// Converter
	/// </summary>
	public class TypeConverter
	{
		/// <summary>
		/// Convert a object to the type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static T Convert<T>(string value)
		{
			if (value == null)
			{
				return default;
			}

			var itm = Convert(typeof(T), value);
			if (itm == null)
			{
				return default(T);
			}

			return (T)itm;
		}

		/// <summary>
		/// Convert a object to the type
		/// </summary>
		/// <param name="type"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static object Convert(Type type, string value)
		{
			if (type == typeof(string) || type == typeof(object))
			{
				return value.Trim();
			}

			if (type == typeof(bool) || type == typeof(bool?))
			{
				if (value != null)
				{
					return ParseBoolean(value);
				}

				return null;
			}

			if (type == typeof(long) || type == typeof(long?))
			{
				if (long.TryParse(value, out var l))
				{
					return l;
				}

				return null;
			}

			if (type == typeof(int) || type == typeof(int?))
			{
				if (int.TryParse(value, out var i))
				{
					return i;
				}

				return null;
			}

			if (type == typeof(decimal))
			{
				return decimal.Parse(value);
			}

			if (type == typeof(double) || type == typeof(double?))
			{
				if (double.TryParse(value, out var b))
				{
					return b;
				}

				return null;
			}

			if (type == typeof(DateTime) || type == typeof(DateTime?))
			{
				if (DateTime.TryParse(value, out var date))
				{
					return date;
				}

				var formats = new string[]
				{
					"yyyy.MM.ddTHH:mm:ss",
					"yyyy-MM-ddTHH:mm:ss",
					"yyyy/MM/ddTHH:mm:ss",
					"yyyyMMdd"
				};

				if (DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out date))
				{
					return date;
				}

				throw new FormatException($"Value {value} could not be parsed to {type.FullName}. DateTimes have to be in the ISO-8601 format eg. yyyy-MM-dd");
			}

			if (type == typeof(TimeSpan) || type == typeof(TimeSpan?))
			{
				if (TimeSpan.TryParse(value, out var time))
				{
					return time;
				}

				return null;
			}

			if (type == typeof(Guid))
			{
				if (Guid.TryParse(value, out var guid))
				{
					return guid;
				}

				if (Guid.TryParseExact(value, "B", out guid))
				{
					return guid;
				}

				return null;
			}

			if (type == typeof(Type))
			{
				return Type.GetType(value);
			}

			if (type.IsEnum)
			{
				try
				{
					return Enum.Parse(type, value, true);
				}
				catch (ArgumentException)
				{
					// returns null anyway so do nothing
				}
			}

			return null;
		}

		/// <summary>
		/// Parse a boolean. Valid is 1,y,yes,true,0,n,no,false
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool ParseBoolean(object value)
		{
			if (value == null || value == DBNull.Value)
			{
				return false;
			}

			switch (value.ToString().ToLowerInvariant())
			{
				case "1":
				case "y":
				case "yes":
				case "true":
					return true;

				case "0":
				case "n":
				case "no":
				case "false":
				default:
					return false;
			}
		}
	}
}
