using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Broadcast.Composition
{
	internal static class MethodInfoExtensions
	{
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
