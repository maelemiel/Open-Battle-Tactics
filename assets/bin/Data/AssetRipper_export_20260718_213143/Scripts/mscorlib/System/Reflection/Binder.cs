using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Serializable]
	[ClassInterface(ClassInterfaceType.AutoDual)]
	[ComVisible(true)]
	public abstract class Binder
	{
		internal sealed class Default : Binder
		{
			public override FieldInfo BindToField(BindingFlags bindingAttr, FieldInfo[] match, object value, CultureInfo culture)
			{
				if (match == null)
				{
					throw new ArgumentNullException("match");
				}
				foreach (FieldInfo fieldInfo in match)
				{
					if (check_type(value.GetType(), fieldInfo.FieldType))
					{
						return fieldInfo;
					}
				}
				return null;
			}

			public override MethodBase BindToMethod(BindingFlags bindingAttr, MethodBase[] match, ref object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] names, out object state)
			{
				Type[] array;
				if (args == null)
				{
					array = Type.EmptyTypes;
				}
				else
				{
					array = new Type[args.Length];
					for (int i = 0; i < args.Length; i++)
					{
						if (args[i] != null)
						{
							array[i] = args[i].GetType();
						}
					}
				}
				MethodBase methodBase = SelectMethod(bindingAttr, match, array, modifiers, true);
				state = null;
				if (names != null)
				{
					ReorderParameters(names, ref args, methodBase);
				}
				return methodBase;
			}

			private void ReorderParameters(string[] names, ref object[] args, MethodBase selected)
			{
				object[] array = new object[args.Length];
				Array.Copy(args, array, args.Length);
				ParameterInfo[] parameters = selected.GetParameters();
				for (int i = 0; i < names.Length; i++)
				{
					for (int j = 0; j < parameters.Length; j++)
					{
						if (names[i] == parameters[j].Name)
						{
							array[j] = args[i];
							break;
						}
					}
				}
				Array.Copy(array, args, args.Length);
			}

			private static bool IsArrayAssignable(Type object_type, Type target_type)
			{
				if (object_type.IsArray && target_type.IsArray)
				{
					return IsArrayAssignable(object_type.GetElementType(), target_type.GetElementType());
				}
				if (target_type.IsAssignableFrom(object_type))
				{
					return true;
				}
				return false;
			}

			public override object ChangeType(object value, Type type, CultureInfo culture)
			{
				if (value == null)
				{
					return null;
				}
				Type type2 = value.GetType();
				if (type.IsByRef)
				{
					type = type.GetElementType();
				}
				if (type2 == type || type.IsInstanceOfType(value))
				{
					return value;
				}
				if (type2.IsArray && type.IsArray && IsArrayAssignable(type2.GetElementType(), type.GetElementType()))
				{
					return value;
				}
				if (check_type(type2, type))
				{
					if (type.IsEnum)
					{
						return Enum.ToObject(type, value);
					}
					if (type2 == typeof(char))
					{
						if (type == typeof(double))
						{
							return (double)(int)(char)value;
						}
						if (type == typeof(float))
						{
							return (float)(int)(char)value;
						}
					}
					if (type2 == typeof(IntPtr) && type.IsPointer)
					{
						return value;
					}
					return Convert.ChangeType(value, type);
				}
				return null;
			}

			[MonoTODO("This method does not do anything in Mono")]
			public override void ReorderArgumentArray(ref object[] args, object state)
			{
			}

			private static bool check_type(Type from, Type to)
			{
				if (from == to)
				{
					return true;
				}
				if (from == null)
				{
					return true;
				}
				if (to.IsByRef != from.IsByRef)
				{
					return false;
				}
				if (to.IsInterface)
				{
					return to.IsAssignableFrom(from);
				}
				if (to.IsEnum)
				{
					to = Enum.GetUnderlyingType(to);
					if (from == to)
					{
						return true;
					}
				}
				if (to.IsGenericType && to.GetGenericTypeDefinition() == typeof(Nullable<>) && to.GetGenericArguments()[0] == from)
				{
					return true;
				}
				TypeCode typeCode = Type.GetTypeCode(from);
				TypeCode typeCode2 = Type.GetTypeCode(to);
				switch (typeCode)
				{
				case TypeCode.Char:
					switch (typeCode2)
					{
					case TypeCode.UInt16:
					case TypeCode.Int32:
					case TypeCode.UInt32:
					case TypeCode.Int64:
					case TypeCode.UInt64:
					case TypeCode.Single:
					case TypeCode.Double:
						return true;
					default:
						return to == typeof(object);
					}
				case TypeCode.Byte:
					switch (typeCode2)
					{
					case TypeCode.Char:
					case TypeCode.Int16:
					case TypeCode.UInt16:
					case TypeCode.Int32:
					case TypeCode.UInt32:
					case TypeCode.Int64:
					case TypeCode.UInt64:
					case TypeCode.Single:
					case TypeCode.Double:
						return true;
					default:
						return to == typeof(object) || (from.IsEnum && to == typeof(Enum));
					}
				case TypeCode.SByte:
					switch (typeCode2)
					{
					case TypeCode.Int16:
					case TypeCode.Int32:
					case TypeCode.Int64:
					case TypeCode.Single:
					case TypeCode.Double:
						return true;
					default:
						return to == typeof(object) || (from.IsEnum && to == typeof(Enum));
					}
				case TypeCode.UInt16:
					switch (typeCode2)
					{
					case TypeCode.Int32:
					case TypeCode.UInt32:
					case TypeCode.Int64:
					case TypeCode.UInt64:
					case TypeCode.Single:
					case TypeCode.Double:
						return true;
					default:
						return to == typeof(object) || (from.IsEnum && to == typeof(Enum));
					}
				case TypeCode.Int16:
					switch (typeCode2)
					{
					case TypeCode.Int32:
					case TypeCode.Int64:
					case TypeCode.Single:
					case TypeCode.Double:
						return true;
					default:
						return to == typeof(object) || (from.IsEnum && to == typeof(Enum));
					}
				case TypeCode.UInt32:
					switch (typeCode2)
					{
					case TypeCode.Int64:
					case TypeCode.UInt64:
					case TypeCode.Single:
					case TypeCode.Double:
						return true;
					default:
						return to == typeof(object) || (from.IsEnum && to == typeof(Enum));
					}
				case TypeCode.Int32:
					switch (typeCode2)
					{
					case TypeCode.Int64:
					case TypeCode.Single:
					case TypeCode.Double:
						return true;
					default:
						return to == typeof(object) || (from.IsEnum && to == typeof(Enum));
					}
				case TypeCode.Int64:
				case TypeCode.UInt64:
				{
					TypeCode typeCode3 = typeCode2;
					if (typeCode3 == TypeCode.Single || typeCode3 == TypeCode.Double)
					{
						return true;
					}
					return to == typeof(object) || (from.IsEnum && to == typeof(Enum));
				}
				case TypeCode.Single:
					return typeCode2 == TypeCode.Double || to == typeof(object);
				default:
					if (to == typeof(object) && from.IsValueType)
					{
						return true;
					}
					if (to.IsPointer && from == typeof(IntPtr))
					{
						return true;
					}
					return to.IsAssignableFrom(from);
				}
			}

			private static bool check_arguments(Type[] types, ParameterInfo[] args, bool allowByRefMatch)
			{
				for (int i = 0; i < types.Length; i++)
				{
					bool flag = check_type(types[i], args[i].ParameterType);
					if (!flag && allowByRefMatch)
					{
						Type parameterType = args[i].ParameterType;
						if (parameterType.IsByRef)
						{
							flag = check_type(types[i], parameterType.GetElementType());
						}
					}
					if (!flag)
					{
						return false;
					}
				}
				return true;
			}

			public override MethodBase SelectMethod(BindingFlags bindingAttr, MethodBase[] match, Type[] types, ParameterModifier[] modifiers)
			{
				return SelectMethod(bindingAttr, match, types, modifiers, false);
			}

			private MethodBase SelectMethod(BindingFlags bindingAttr, MethodBase[] match, Type[] types, ParameterModifier[] modifiers, bool allowByRefMatch)
			{
				if (match == null)
				{
					throw new ArgumentNullException("match");
				}
				foreach (MethodBase methodBase in match)
				{
					ParameterInfo[] parameters = methodBase.GetParameters();
					if (parameters.Length == types.Length)
					{
						int j;
						for (j = 0; j < types.Length && types[j] == parameters[j].ParameterType; j++)
						{
						}
						if (j == types.Length)
						{
							return methodBase;
						}
					}
				}
				bool flag = false;
				Type type = null;
				foreach (MethodBase methodBase in match)
				{
					ParameterInfo[] parameters2 = methodBase.GetParameters();
					if (parameters2.Length <= types.Length && parameters2.Length != 0 && Attribute.IsDefined(parameters2[parameters2.Length - 1], typeof(ParamArrayAttribute)))
					{
						type = parameters2[parameters2.Length - 1].ParameterType.GetElementType();
						int j;
						for (j = 0; j < types.Length && (j >= parameters2.Length - 1 || types[j] == parameters2[j].ParameterType) && (j < parameters2.Length - 1 || types[j] == type); j++)
						{
						}
						if (j == types.Length)
						{
							return methodBase;
						}
					}
				}
				if ((bindingAttr & BindingFlags.ExactBinding) != BindingFlags.Default)
				{
					return null;
				}
				MethodBase methodBase2 = null;
				foreach (MethodBase methodBase in match)
				{
					ParameterInfo[] parameters3 = methodBase.GetParameters();
					if (parameters3.Length == types.Length && check_arguments(types, parameters3, allowByRefMatch))
					{
						methodBase2 = ((methodBase2 == null) ? methodBase : GetBetterMethod(methodBase2, methodBase, types));
					}
				}
				return methodBase2;
			}

			private MethodBase GetBetterMethod(MethodBase m1, MethodBase m2, Type[] types)
			{
				if (m1.IsGenericMethodDefinition && !m2.IsGenericMethodDefinition)
				{
					return m2;
				}
				if (m2.IsGenericMethodDefinition && !m1.IsGenericMethodDefinition)
				{
					return m1;
				}
				ParameterInfo[] parameters = m1.GetParameters();
				ParameterInfo[] parameters2 = m2.GetParameters();
				int num = 0;
				for (int i = 0; i < parameters.Length; i++)
				{
					int num2 = CompareCloserType(parameters[i].ParameterType, parameters2[i].ParameterType);
					if (num2 != 0 && num != 0 && num != num2)
					{
						throw new AmbiguousMatchException();
					}
					if (num2 != 0)
					{
						num = num2;
					}
				}
				if (num != 0)
				{
					return (num <= 0) ? m1 : m2;
				}
				Type declaringType = m1.DeclaringType;
				Type declaringType2 = m2.DeclaringType;
				if (declaringType != declaringType2)
				{
					if (declaringType.IsSubclassOf(declaringType2))
					{
						return m1;
					}
					if (declaringType2.IsSubclassOf(declaringType))
					{
						return m2;
					}
				}
				bool flag = (m1.CallingConvention & CallingConventions.VarArgs) != 0;
				bool flag2 = (m2.CallingConvention & CallingConventions.VarArgs) != 0;
				if (flag && !flag2)
				{
					return m2;
				}
				if (flag2 && !flag)
				{
					return m1;
				}
				throw new AmbiguousMatchException();
			}

			private int CompareCloserType(Type t1, Type t2)
			{
				if (t1 == t2)
				{
					return 0;
				}
				if (t1.IsGenericParameter && !t2.IsGenericParameter)
				{
					return 1;
				}
				if (!t1.IsGenericParameter && t2.IsGenericParameter)
				{
					return -1;
				}
				if (t1.HasElementType && t2.HasElementType)
				{
					return CompareCloserType(t1.GetElementType(), t2.GetElementType());
				}
				if (t1.IsSubclassOf(t2))
				{
					return -1;
				}
				if (t2.IsSubclassOf(t1))
				{
					return 1;
				}
				if (t1.IsInterface && Array.IndexOf(t2.GetInterfaces(), t1) >= 0)
				{
					return 1;
				}
				if (t2.IsInterface && Array.IndexOf(t1.GetInterfaces(), t2) >= 0)
				{
					return -1;
				}
				return 0;
			}

			public override PropertyInfo SelectProperty(BindingFlags bindingAttr, PropertyInfo[] match, Type returnType, Type[] indexes, ParameterModifier[] modifiers)
			{
				if (match == null || match.Length == 0)
				{
					throw new ArgumentException("No properties provided", "match");
				}
				bool flag = returnType != null;
				int num = ((indexes == null) ? (-1) : indexes.Length);
				PropertyInfo propertyInfo = null;
				int num2 = 2147483646;
				int num3 = int.MaxValue;
				int num4 = 0;
				for (int num5 = match.Length - 1; num5 >= 0; num5--)
				{
					PropertyInfo propertyInfo2 = match[num5];
					ParameterInfo[] indexParameters = propertyInfo2.GetIndexParameters();
					if ((num >= 0 && num != indexParameters.Length) || (flag && propertyInfo2.PropertyType != returnType))
					{
						continue;
					}
					int num6 = 2147483646;
					if (num > 0)
					{
						num6 = check_arguments_with_score(indexes, indexParameters);
						if (num6 == -1)
						{
							continue;
						}
					}
					int derivedLevel = GetDerivedLevel(propertyInfo2.DeclaringType);
					if (propertyInfo != null)
					{
						if (num2 < num6)
						{
							continue;
						}
						if (num2 == num6)
						{
							if (num4 == derivedLevel)
							{
								num3 = num6;
								continue;
							}
							if (num4 > derivedLevel)
							{
								continue;
							}
						}
					}
					propertyInfo = propertyInfo2;
					num2 = num6;
					num4 = derivedLevel;
				}
				if (num3 <= num2)
				{
					throw new AmbiguousMatchException();
				}
				return propertyInfo;
			}

			private static int check_arguments_with_score(Type[] types, ParameterInfo[] args)
			{
				int num = -1;
				for (int i = 0; i < types.Length; i++)
				{
					int num2 = check_type_with_score(types[i], args[i].ParameterType);
					if (num2 == -1)
					{
						return -1;
					}
					if (num < num2)
					{
						num = num2;
					}
				}
				return num;
			}

			private static int check_type_with_score(Type from, Type to)
			{
				if (from == null)
				{
					return to.IsValueType ? (-1) : 0;
				}
				if (from == to)
				{
					return 0;
				}
				if (to == typeof(object))
				{
					return 4;
				}
				TypeCode typeCode = Type.GetTypeCode(from);
				TypeCode typeCode2 = Type.GetTypeCode(to);
				switch (typeCode)
				{
				case TypeCode.Char:
					switch (typeCode2)
					{
					case TypeCode.UInt16:
						return 0;
					case TypeCode.Int32:
					case TypeCode.UInt32:
					case TypeCode.Int64:
					case TypeCode.UInt64:
					case TypeCode.Single:
					case TypeCode.Double:
						return 2;
					default:
						return -1;
					}
				case TypeCode.Byte:
					switch (typeCode2)
					{
					case TypeCode.Char:
					case TypeCode.Int16:
					case TypeCode.UInt16:
					case TypeCode.Int32:
					case TypeCode.UInt32:
					case TypeCode.Int64:
					case TypeCode.UInt64:
					case TypeCode.Single:
					case TypeCode.Double:
						return 2;
					default:
						return (from.IsEnum && to == typeof(Enum)) ? 1 : (-1);
					}
				case TypeCode.SByte:
					switch (typeCode2)
					{
					case TypeCode.Int16:
					case TypeCode.Int32:
					case TypeCode.Int64:
					case TypeCode.Single:
					case TypeCode.Double:
						return 2;
					default:
						return (from.IsEnum && to == typeof(Enum)) ? 1 : (-1);
					}
				case TypeCode.UInt16:
					switch (typeCode2)
					{
					case TypeCode.Int32:
					case TypeCode.UInt32:
					case TypeCode.Int64:
					case TypeCode.UInt64:
					case TypeCode.Single:
					case TypeCode.Double:
						return 2;
					default:
						return (from.IsEnum && to == typeof(Enum)) ? 1 : (-1);
					}
				case TypeCode.Int16:
					switch (typeCode2)
					{
					case TypeCode.Int32:
					case TypeCode.Int64:
					case TypeCode.Single:
					case TypeCode.Double:
						return 2;
					default:
						return (from.IsEnum && to == typeof(Enum)) ? 1 : (-1);
					}
				case TypeCode.UInt32:
					switch (typeCode2)
					{
					case TypeCode.Int64:
					case TypeCode.UInt64:
					case TypeCode.Single:
					case TypeCode.Double:
						return 2;
					default:
						return (from.IsEnum && to == typeof(Enum)) ? 1 : (-1);
					}
				case TypeCode.Int32:
					switch (typeCode2)
					{
					case TypeCode.Int64:
					case TypeCode.Single:
					case TypeCode.Double:
						return 2;
					default:
						return (from.IsEnum && to == typeof(Enum)) ? 1 : (-1);
					}
				case TypeCode.Int64:
				case TypeCode.UInt64:
				{
					TypeCode typeCode3 = typeCode2;
					if (typeCode3 == TypeCode.Single || typeCode3 == TypeCode.Double)
					{
						return 2;
					}
					return (from.IsEnum && to == typeof(Enum)) ? 1 : (-1);
				}
				case TypeCode.Single:
					return (typeCode2 != TypeCode.Double) ? (-1) : 2;
				default:
					return (!to.IsAssignableFrom(from)) ? (-1) : 3;
				}
			}
		}

		private static Binder default_binder = new Default();

		internal static Binder DefaultBinder
		{
			get
			{
				return default_binder;
			}
		}

		public abstract FieldInfo BindToField(BindingFlags bindingAttr, FieldInfo[] match, object value, CultureInfo culture);

		public abstract MethodBase BindToMethod(BindingFlags bindingAttr, MethodBase[] match, ref object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] names, out object state);

		public abstract object ChangeType(object value, Type type, CultureInfo culture);

		public abstract void ReorderArgumentArray(ref object[] args, object state);

		public abstract MethodBase SelectMethod(BindingFlags bindingAttr, MethodBase[] match, Type[] types, ParameterModifier[] modifiers);

		public abstract PropertyInfo SelectProperty(BindingFlags bindingAttr, PropertyInfo[] match, Type returnType, Type[] indexes, ParameterModifier[] modifiers);

		internal static bool ConvertArgs(Binder binder, object[] args, ParameterInfo[] pinfo, CultureInfo culture)
		{
			if (args == null)
			{
				if (pinfo.Length == 0)
				{
					return true;
				}
				throw new TargetParameterCountException();
			}
			if (pinfo.Length != args.Length)
			{
				throw new TargetParameterCountException();
			}
			for (int i = 0; i < args.Length; i++)
			{
				object obj = binder.ChangeType(args[i], pinfo[i].ParameterType, culture);
				if (obj == null && args[i] != null)
				{
					return false;
				}
				args[i] = obj;
			}
			return true;
		}

		internal static int GetDerivedLevel(Type type)
		{
			Type type2 = type;
			int num = 1;
			while (type2.BaseType != null)
			{
				num++;
				type2 = type2.BaseType;
			}
			return num;
		}

		internal static MethodBase FindMostDerivedMatch(MethodBase[] match)
		{
			int num = 0;
			int num2 = -1;
			int num3 = match.Length;
			for (int i = 0; i < num3; i++)
			{
				MethodBase methodBase = match[i];
				int derivedLevel = GetDerivedLevel(methodBase.DeclaringType);
				if (derivedLevel == num)
				{
					throw new AmbiguousMatchException();
				}
				if (num2 >= 0)
				{
					ParameterInfo[] parameters = methodBase.GetParameters();
					ParameterInfo[] parameters2 = match[num2].GetParameters();
					bool flag = true;
					if (parameters.Length != parameters2.Length)
					{
						flag = false;
					}
					else
					{
						for (int j = 0; j < parameters.Length; j++)
						{
							if (parameters[j].ParameterType != parameters2[j].ParameterType)
							{
								flag = false;
								break;
							}
						}
					}
					if (!flag)
					{
						throw new AmbiguousMatchException();
					}
				}
				if (derivedLevel > num)
				{
					num = derivedLevel;
					num2 = i;
				}
			}
			return match[num2];
		}
	}
}
