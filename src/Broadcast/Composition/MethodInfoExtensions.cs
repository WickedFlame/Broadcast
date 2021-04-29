using System.Linq;
using System.Reflection;

namespace Broadcast.Composition
{
	/// <summary>
	/// Extension class for MethodInfo
	/// </summary>
	internal static class MethodInfoExtensions
	{
		/// <summary>
		/// Gets the name  of the method
		/// </summary>
		/// <param name="methodInfo"></param>
		/// <returns></returns>
		public static string GetNormalizedName(this MethodInfo methodInfo)
		{
			// Method names containing '.' are considered explicitly implemented interface methods
			// https://stackoverflow.com/a/17854048/1398672
			return methodInfo.Name.Contains(".") && methodInfo.IsFinal && methodInfo.IsPrivate
				? methodInfo.Name.Split('.').Last()
				: methodInfo.Name;
		}
	}
}
