
namespace Broadcast.Storage
{
	/// <summary>
	/// Extension helper methods for <see cref="PropertyValue"/> and objec
	/// </summary>
	public static class PropertyValueExtensions
	{
		/// <summary>
		/// Convert a object to a bool
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static bool ToBool(this object obj)
		{
			if (obj == null)
			{
				return false;
			}

			if (obj is bool b)
			{
				return b;
			}

			switch (obj.ToString().ToLower())
			{
				case "true":
				case "1":
					return true;
			}

			return false;
		}
	}
}
