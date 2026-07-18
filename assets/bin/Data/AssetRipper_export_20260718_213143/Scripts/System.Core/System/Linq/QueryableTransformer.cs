using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Linq
{
	internal class QueryableTransformer : ExpressionTransformer
	{
		protected override Expression VisitMethodCall(MethodCallExpression methodCall)
		{
			if (IsQueryableExtension(methodCall.Method))
			{
				return ReplaceQueryableMethod(methodCall);
			}
			return base.VisitMethodCall(methodCall);
		}

		protected override Expression VisitLambda(LambdaExpression lambda)
		{
			return lambda;
		}

		protected override Expression VisitConstant(ConstantExpression constant)
		{
			IQueryableEnumerable queryableEnumerable = constant.Value as IQueryableEnumerable;
			if (queryableEnumerable == null)
			{
				return constant;
			}
			return Expression.Constant(queryableEnumerable.GetEnumerable());
		}

		private static bool IsQueryableExtension(MethodInfo method)
		{
			return HasExtensionAttribute(method) && method.GetParameters()[0].ParameterType.IsAssignableTo(typeof(IQueryable));
		}

		private static bool HasExtensionAttribute(MethodInfo method)
		{
			return method.GetCustomAttributes(typeof(ExtensionAttribute), false).Length > 0;
		}

		private MethodCallExpression ReplaceQueryableMethod(MethodCallExpression old)
		{
			Expression obj = null;
			if (old.Object != null)
			{
				obj = Visit(old.Object);
			}
			MethodInfo methodInfo = ReplaceQueryableMethod(old.Method);
			ParameterInfo[] parameters = methodInfo.GetParameters();
			Expression[] array = new Expression[old.Arguments.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = UnquoteIfNeeded(Visit(old.Arguments[i]), parameters[i].ParameterType);
			}
			return new MethodCallExpression(obj, methodInfo, array.ToReadOnlyCollection());
		}

		private static Expression UnquoteIfNeeded(Expression expression, Type delegateType)
		{
			if (expression.NodeType != ExpressionType.Quote)
			{
				return expression;
			}
			LambdaExpression lambdaExpression = (LambdaExpression)((UnaryExpression)expression).Operand;
			if (lambdaExpression.Type == delegateType)
			{
				return lambdaExpression;
			}
			return expression;
		}

		private static Type GetTargetDeclaringType(MethodInfo method)
		{
			return (method.DeclaringType != typeof(Queryable)) ? method.DeclaringType : typeof(Enumerable);
		}

		private static MethodInfo ReplaceQueryableMethod(MethodInfo method)
		{
			MethodInfo matchingMethod = GetMatchingMethod(method, GetTargetDeclaringType(method));
			if (matchingMethod != null)
			{
				return matchingMethod;
			}
			throw new InvalidOperationException(string.Format("There is no method {0} on type {1} that matches the specified arguments", method.Name, method.DeclaringType.FullName));
		}

		private static MethodInfo GetMatchingMethod(MethodInfo method, Type declaring)
		{
			MethodInfo[] methods = declaring.GetMethods();
			foreach (MethodInfo methodInfo in methods)
			{
				if (MethodMatch(methodInfo, method))
				{
					if (method.IsGenericMethod)
					{
						return methodInfo.MakeGenericMethodFrom(method);
					}
					return methodInfo;
				}
			}
			return null;
		}

		private static bool MethodMatch(MethodInfo candidate, MethodInfo method)
		{
			if (candidate.Name != method.Name)
			{
				return false;
			}
			if (!HasExtensionAttribute(candidate))
			{
				return false;
			}
			Type[] parameterTypes = method.GetParameterTypes();
			if (parameterTypes.Length != candidate.GetParameters().Length)
			{
				return false;
			}
			if (method.IsGenericMethod)
			{
				if (!candidate.IsGenericMethod)
				{
					return false;
				}
				if (candidate.GetGenericArguments().Length != method.GetGenericArguments().Length)
				{
					return false;
				}
				candidate = candidate.MakeGenericMethodFrom(method);
			}
			if (!TypeMatch(candidate.ReturnType, method.ReturnType))
			{
				return false;
			}
			Type[] parameterTypes2 = candidate.GetParameterTypes();
			if (parameterTypes2[0] != GetComparableType(parameterTypes[0]))
			{
				return false;
			}
			for (int i = 1; i < parameterTypes2.Length; i++)
			{
				if (!TypeMatch(parameterTypes2[i], parameterTypes[i]))
				{
					return false;
				}
			}
			return true;
		}

		private static bool TypeMatch(Type candidate, Type type)
		{
			if (candidate == type)
			{
				return true;
			}
			return candidate == GetComparableType(type);
		}

		private static Type GetComparableType(Type type)
		{
			if (type.IsGenericInstanceOf(typeof(IQueryable<>)))
			{
				type = typeof(IEnumerable<>).MakeGenericTypeFrom(type);
			}
			else if (type.IsGenericInstanceOf(typeof(IOrderedQueryable<>)))
			{
				type = typeof(IOrderedEnumerable<>).MakeGenericTypeFrom(type);
			}
			else if (type.IsGenericInstanceOf(typeof(Expression<>)))
			{
				type = type.GetFirstGenericArgument();
			}
			else if (type == typeof(IQueryable))
			{
				type = typeof(IEnumerable);
			}
			return type;
		}
	}
}
