﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Broadcast.EventSourcing;

namespace Broadcast.Composition
{
	/// <summary>
	/// Extension methods for Type
	/// </summary>
	public static class TypeExtensions
	{
		/// <summary>
		/// Creates a name based on the type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string ToGenericTypeString(this Type type)
		{
			if (!type.GetTypeInfo().IsGenericType)
			{
				return type.GetFullNameWithoutNamespace()
					.ReplacePlusWithDotInNestedTypeName();
			}

			return type.GetGenericTypeDefinition()
				.GetFullNameWithoutNamespace()
				.ReplacePlusWithDotInNestedTypeName()
				.ReplaceGenericParametersInGenericTypeName(type);
		}

		/// <summary>
		/// Gets the methodinfo
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name"></param>
		/// <param name="parameterTypes"></param>
		/// <returns></returns>
		public static MethodInfo GetNonOpenMatchingMethod(this Type type, string name, Type[] parameterTypes)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			parameterTypes = parameterTypes ?? new Type[0];

			var methodCandidates = new List<MethodInfo>(type.GetRuntimeMethods());

			if (type.GetTypeInfo().IsInterface)
			{
				methodCandidates.AddRange(type.GetTypeInfo().ImplementedInterfaces.SelectMany(x => x.GetRuntimeMethods()));
			}

			foreach (var methodCandidate in methodCandidates)
			{
				if (!methodCandidate.GetNormalizedName().Equals(name, StringComparison.Ordinal))
				{
					continue;
				}

				var parameters = methodCandidate.GetParameters();
				if (parameters.Length != parameterTypes.Length)
				{
					continue;
				}

				var parameterTypesMatched = true;

				var genericArguments = methodCandidate.ContainsGenericParameters
					? new Type[methodCandidate.GetGenericArguments().Length]
					: null;

				// Determining whether we can use this method candidate with
				// current parameter types.
				for (var i = 0; i < parameters.Length; i++)
				{
					var parameterType = parameters[i].ParameterType.GetTypeInfo();
					var actualType = parameterTypes[i].GetTypeInfo();

					if (!TypesMatchRecursive(parameterType, actualType, genericArguments))
					{
						parameterTypesMatched = false;
						break;
					}
				}

				if (parameterTypesMatched)
				{
					if (genericArguments != null)
					{
						var genericArgumentsResolved = true;

						foreach (var genericArgument in genericArguments)
						{
							if (genericArgument == null)
							{
								genericArgumentsResolved = false;
							}
						}

						if (genericArgumentsResolved)
						{
							return methodCandidate.MakeGenericMethod(genericArguments);
						}
					}
					else
					{
						// Return first found method candidate with matching parameters.
						return methodCandidate;
					}
				}
			}

			return null;
		}

		private static bool TypesMatchRecursive(TypeInfo parameterType, TypeInfo actualType, IList<Type> genericArguments)
		{
			if (parameterType.IsGenericParameter)
			{
				var position = parameterType.GenericParameterPosition;

				// Return false if this generic parameter has been identified and it's not the same as actual type
				if (genericArguments[position] != null && genericArguments[position].GetTypeInfo() != actualType)
				{
					return false;
				}

				genericArguments[position] = actualType.AsType();
				return true;
			}

			if (parameterType.ContainsGenericParameters)
			{
				if (parameterType.IsArray)
				{
					// Return false if parameterType is array whereas actualType isn't
					if (!actualType.IsArray) return false;

					var parameterElementType = parameterType.GetElementType();
					var actualElementType = actualType.GetElementType();

					return TypesMatchRecursive(parameterElementType.GetTypeInfo(), actualElementType.GetTypeInfo(), genericArguments);
				}

				if (!actualType.IsGenericType || parameterType.GetGenericTypeDefinition() != actualType.GetGenericTypeDefinition())
				{
					return false;
				}

				for (var i = 0; i < parameterType.GenericTypeArguments.Length; i++)
				{
					var parameterGenericArgument = parameterType.GenericTypeArguments[i];
					var actualGenericArgument = actualType.GenericTypeArguments[i];

					if (!TypesMatchRecursive(parameterGenericArgument.GetTypeInfo(), actualGenericArgument.GetTypeInfo(), genericArguments))
					{
						return false;
					}
				}

				return true;
			}

			return parameterType == actualType;
		}







		private static string GetFullNameWithoutNamespace(this Type type)
		{
			if (type.IsGenericParameter)
			{
				return type.Name;
			}

			const int dotLength = 1;
			// ReSharper disable once PossibleNullReferenceException
			return !String.IsNullOrEmpty(type.Namespace)
				? type.FullName.Substring(type.Namespace.Length + dotLength)
				: type.FullName;
		}

		private static string ReplacePlusWithDotInNestedTypeName(this string typeName)
		{
			return typeName.Replace('+', '.');
		}

		public static Type[] GetAllGenericArguments(this TypeInfo type)
		{
			return type.GenericTypeArguments.Length > 0 ? type.GenericTypeArguments : type.GenericTypeParameters;
		}

		private static string ReplaceGenericParametersInGenericTypeName(this string typeName, Type type)
		{
			var genericArguments = type.GetTypeInfo().GetAllGenericArguments();

			const string regexForGenericArguments = @"`[1-9]\d*";

			var rgx = new Regex(regexForGenericArguments);

			typeName = rgx.Replace(typeName, match =>
			{
				var currentGenericArgumentNumbers = int.Parse(match.Value.Substring(1));
				var currentArguments = string.Join(",", genericArguments.Take(currentGenericArgumentNumbers).Select(ToGenericTypeString));
				genericArguments = genericArguments.Skip(currentGenericArgumentNumbers).ToArray();
				return string.Concat("<", currentArguments, ">");
			});

			return typeName;
		}
	}
}
