using System.Text.RegularExpressions;

namespace Broadcast.Dashboard
{
	/// <summary>
	/// 
	/// </summary>
	public static class ExtensionMethods
	{
		/// <summary>
		/// Ensures there is a trailing slash in the path
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string EnsureTrailingSlash(this string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return string.Empty;
			}

			return Regex.Replace(input, "/+$", string.Empty) + "/";
		}

		/// <summary>
		/// Ensures there is a leading slash in the path
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string EnsureLeadingSlash(this string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return string.Empty;
			}

			return "/" + Regex.Replace(input, "^/", string.Empty);
		}

		/// <summary>
		/// Remove the leading slash from the path
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string RemoveLeadingSlash(this string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return string.Empty;
			}

			return Regex.Replace(input, "^/", string.Empty);
		}
	}
}
