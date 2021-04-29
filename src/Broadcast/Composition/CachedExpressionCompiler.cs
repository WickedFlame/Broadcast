﻿using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Broadcast.Composition
{
	internal static class CachedExpressionCompiler
	{
		private static readonly ParameterExpression UnusedParameterExpr = Expression.Parameter(typeof(object), "_unused");

		/// <summary>
		/// Evaluates an expression (not a LambdaExpression), e.g. 2 + 2.
		/// </summary>
		/// <param name="arg"></param>
		/// <returns>Expression result.</returns>
		public static object Evaluate(Expression arg)
		{
			if (arg == null)
			{
				throw new ArgumentNullException(nameof(arg));
			}

			Func<object, object> func = Wrap(arg);
			return func(null);
		}

		private static Func<object, object> Wrap(Expression arg)
		{
			var lambdaExpr = Expression.Lambda<Func<object, object>>(Expression.Convert(arg, typeof(object)), UnusedParameterExpr);
			return CachedExpressionCompiler.Process(lambdaExpr);
		}

		// This is the entry point to the cached expression compilation system. The system
		// will try to turn the expression into an actual delegate as quickly as possible,
		// relying on cache lookups and other techniques to save time if appropriate.
		// If the provided expression is particularly obscure and the system doesn't know
		// how to handle it, we'll just compile the expression as normal.
		public static Func<TModel, TValue> Process<TModel, TValue>(Expression<Func<TModel, TValue>> lambdaExpression)
		{
			return Compiler<TModel, TValue>.Compile(lambdaExpression);
		}

		private static class Compiler<TIn, TOut>
		{
			private static Func<TIn, TOut> _identityFunc;

			private static readonly ConcurrentDictionary<MemberInfo, Func<TIn, TOut>> _simpleMemberAccessDict =
				new ConcurrentDictionary<MemberInfo, Func<TIn, TOut>>();

			private static readonly ConcurrentDictionary<MemberInfo, Func<object, TOut>> _constMemberAccessDict =
				new ConcurrentDictionary<MemberInfo, Func<object, TOut>>();

			//private static readonly ConcurrentDictionary<ExpressionFingerprintChain, Hoisted<TIn, TOut>> _fingerprintedCache =
			//	new ConcurrentDictionary<ExpressionFingerprintChain, Hoisted<TIn, TOut>>();

			public static Func<TIn, TOut> Compile(Expression<Func<TIn, TOut>> expr)
			{
				return CompileFromIdentityFunc(expr)
					   ?? CompileFromConstLookup(expr)
					   ?? CompileFromMemberAccess(expr)
					   ?? CompileSlow(expr);
			}

			private static Func<TIn, TOut> CompileFromConstLookup(Expression<Func<TIn, TOut>> expr)
			{
				if (expr.Body is ConstantExpression constExpr)
				{
					// model => {const}

					var constantValue = (TOut)constExpr.Value;
					return _ => constantValue;
				}

				return null;
			}

			private static Func<TIn, TOut> CompileFromIdentityFunc(Expression<Func<TIn, TOut>> expr)
			{
				if (expr.Body == expr.Parameters[0])
				{
					// model => model

					// don't need to lock, as all identity funcs are identical
					return _identityFunc ?? (_identityFunc = expr.Compile());
				}

				return null;
			}

			private static Func<TIn, TOut> CompileFromMemberAccess(Expression<Func<TIn, TOut>> expr)
			{
				// Performance tests show that on the x64 platform, special-casing static member and
				// captured local variable accesses is faster than letting the fingerprinting system
				// handle them. On the x86 platform, the fingerprinting system is faster, but only
				// by around one microsecond, so it's not worth it to complicate the logic here with
				// an architecture check.

				if (expr.Body is MemberExpression memberExpr)
				{
					if (memberExpr.Expression == expr.Parameters[0] || memberExpr.Expression == null)
					{
						// model => model.Member or model => StaticMember
						return _simpleMemberAccessDict.GetOrAdd(memberExpr.Member, _ => expr.Compile());
					}

					if (memberExpr.Expression is ConstantExpression constExpr)
					{
						// model => {const}.Member (captured local variable)
						var del = _constMemberAccessDict.GetOrAdd(memberExpr.Member, _ =>
						{
							// rewrite as capturedLocal => ((TDeclaringType)capturedLocal).Member
							var constParamExpr = Expression.Parameter(typeof(object), "capturedLocal");
							var constCastExpr = Expression.Convert(constParamExpr, memberExpr.Member.DeclaringType);
							var newMemberAccessExpr = memberExpr.Update(constCastExpr);
							var newLambdaExpr = Expression.Lambda<Func<object, TOut>>(newMemberAccessExpr, constParamExpr);
							return newLambdaExpr.Compile();
						});

						var capturedLocal = constExpr.Value;
						return _ => del(capturedLocal);
					}
				}

				return null;
			}

			private static Func<TIn, TOut> CompileSlow(Expression<Func<TIn, TOut>> expr)
			{
				// fallback compilation system - just compile the expression directly
				return expr.Compile();
			}
		}
	}
}
