using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions
{
	internal static class Extensions
	{
		public static bool IsGenericInstanceOf(this Type self, Type type)
		{
			if (!self.IsGenericType)
			{
				return false;
			}
			return self.GetGenericTypeDefinition() == type;
		}

		public static bool IsNullable(this Type self)
		{
			return self.IsValueType && self.IsGenericInstanceOf(typeof(Nullable<>));
		}

		public static bool IsExpression(this Type self)
		{
			return self == typeof(Expression) || self.IsSubclassOf(typeof(Expression));
		}

		public static bool IsGenericImplementationOf(this Type self, Type type)
		{
			Type[] interfaces = self.GetInterfaces();
			foreach (Type self2 in interfaces)
			{
				if (self2.IsGenericInstanceOf(type))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsAssignableTo(this Type self, Type type)
		{
			return type.IsAssignableFrom(self) || ArrayTypeIsAssignableTo(self, type);
		}

		public static Type GetFirstGenericArgument(this Type self)
		{
			return self.GetGenericArguments()[0];
		}

		public static Type MakeGenericTypeFrom(this Type self, Type type)
		{
			return self.MakeGenericType(type.GetGenericArguments());
		}

		public static Type MakeNullableType(this Type self)
		{
			return typeof(Nullable<>).MakeGenericType(self);
		}

		public static Type GetNotNullableType(this Type self)
		{
			return (!self.IsNullable()) ? self : self.GetFirstGenericArgument();
		}

		public static MethodInfo GetInvokeMethod(this Type self)
		{
			return self.GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public);
		}

		public static MethodInfo MakeGenericMethodFrom(this MethodInfo self, MethodInfo method)
		{
			return self.MakeGenericMethod(method.GetGenericArguments());
		}

		public static Type[] GetParameterTypes(this MethodBase self)
		{
			ParameterInfo[] parameters = self.GetParameters();
			Type[] array = new Type[parameters.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = parameters[i].ParameterType;
			}
			return array;
		}

		private static bool ArrayTypeIsAssignableTo(Type type, Type candidate)
		{
			if (!type.IsArray || !candidate.IsArray)
			{
				return false;
			}
			if (type.GetArrayRank() != candidate.GetArrayRank())
			{
				return false;
			}
			return type.GetElementType().IsAssignableTo(candidate.GetElementType());
		}

		public static void OnFieldOrProperty(this MemberInfo self, Action<FieldInfo> onfield, Action<PropertyInfo> onprop)
		{
			switch (self.MemberType)
			{
			case MemberTypes.Field:
				onfield((FieldInfo)self);
				break;
			case MemberTypes.Property:
				onprop((PropertyInfo)self);
				break;
			default:
				throw new ArgumentException();
			}
		}

		public static T OnFieldOrProperty<T>(this MemberInfo self, Func<FieldInfo, T> onfield, Func<PropertyInfo, T> onprop)
		{
			switch (self.MemberType)
			{
			case MemberTypes.Field:
				return onfield((FieldInfo)self);
			case MemberTypes.Property:
				return onprop((PropertyInfo)self);
			default:
				throw new ArgumentException();
			}
		}

		public static Type MakeStrongBoxType(this Type self)
		{
			return typeof(StrongBox<>).MakeGenericType(self);
		}
	}
}
