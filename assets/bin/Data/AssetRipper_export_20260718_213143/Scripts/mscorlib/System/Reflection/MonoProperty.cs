using System.Globalization;
using System.Runtime.Serialization;
using System.Security;

namespace System.Reflection
{
	[Serializable]
	internal class MonoProperty : PropertyInfo, ISerializable
	{
		private delegate object GetterAdapter(object _this);

		private delegate R Getter<T, R>(T _this);

		private delegate R StaticGetter<R>();

		internal IntPtr klass;

		internal IntPtr prop;

		private MonoPropertyInfo info;

		private PInfo cached;

		private GetterAdapter cached_getter;

		public override PropertyAttributes Attributes
		{
			get
			{
				CachePropertyInfo(PInfo.Attributes);
				return info.attrs;
			}
		}

		public override bool CanRead
		{
			get
			{
				CachePropertyInfo(PInfo.GetMethod);
				return info.get_method != null;
			}
		}

		public override bool CanWrite
		{
			get
			{
				CachePropertyInfo(PInfo.SetMethod);
				return info.set_method != null;
			}
		}

		public override Type PropertyType
		{
			get
			{
				CachePropertyInfo(PInfo.GetMethod | PInfo.SetMethod);
				if (info.get_method != null)
				{
					return info.get_method.ReturnType;
				}
				ParameterInfo[] parameters = info.set_method.GetParameters();
				return parameters[parameters.Length - 1].ParameterType;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				CachePropertyInfo(PInfo.ReflectedType);
				return info.parent;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				CachePropertyInfo(PInfo.DeclaringType);
				return info.parent;
			}
		}

		public override string Name
		{
			get
			{
				CachePropertyInfo(PInfo.Name);
				return info.name;
			}
		}

		private void CachePropertyInfo(PInfo flags)
		{
			if ((cached & flags) != flags)
			{
				MonoPropertyInfo.get_property_info(this, ref info, flags);
				cached |= flags;
			}
		}

		public override MethodInfo[] GetAccessors(bool nonPublic)
		{
			int num = 0;
			int num2 = 0;
			CachePropertyInfo(PInfo.GetMethod | PInfo.SetMethod);
			if (info.set_method != null && (nonPublic || info.set_method.IsPublic))
			{
				num2 = 1;
			}
			if (info.get_method != null && (nonPublic || info.get_method.IsPublic))
			{
				num = 1;
			}
			MethodInfo[] array = new MethodInfo[num + num2];
			int num3 = 0;
			if (num2 != 0)
			{
				array[num3++] = info.set_method;
			}
			if (num != 0)
			{
				array[num3++] = info.get_method;
			}
			return array;
		}

		public override MethodInfo GetGetMethod(bool nonPublic)
		{
			CachePropertyInfo(PInfo.GetMethod);
			if (info.get_method != null && (nonPublic || info.get_method.IsPublic))
			{
				return info.get_method;
			}
			return null;
		}

		public override ParameterInfo[] GetIndexParameters()
		{
			CachePropertyInfo(PInfo.GetMethod | PInfo.SetMethod);
			ParameterInfo[] array;
			if (info.get_method != null)
			{
				array = info.get_method.GetParameters();
			}
			else
			{
				if (info.set_method == null)
				{
					return new ParameterInfo[0];
				}
				ParameterInfo[] parameters = info.set_method.GetParameters();
				array = new ParameterInfo[parameters.Length - 1];
				Array.Copy(parameters, array, array.Length);
			}
			for (int i = 0; i < array.Length; i++)
			{
				ParameterInfo pinfo = array[i];
				array[i] = new ParameterInfo(pinfo, this);
			}
			return array;
		}

		public override MethodInfo GetSetMethod(bool nonPublic)
		{
			CachePropertyInfo(PInfo.SetMethod);
			if (info.set_method != null && (nonPublic || info.set_method.IsPublic))
			{
				return info.set_method;
			}
			return null;
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.IsDefined(this, attributeType, false);
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, false);
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, attributeType, false);
		}

		private static object GetterAdapterFrame<T, R>(Getter<T, R> getter, object obj)
		{
			return getter((T)obj);
		}

		private static object StaticGetterAdapterFrame<R>(StaticGetter<R> getter, object obj)
		{
			return getter();
		}

		private static GetterAdapter CreateGetterDelegate(MethodInfo method)
		{
			Type[] typeArguments;
			Type typeFromHandle;
			string name;
			if (method.IsStatic)
			{
				typeArguments = new Type[1] { method.ReturnType };
				typeFromHandle = typeof(StaticGetter<>);
				name = "StaticGetterAdapterFrame";
			}
			else
			{
				typeArguments = new Type[2] { method.DeclaringType, method.ReturnType };
				typeFromHandle = typeof(Getter<, >);
				name = "GetterAdapterFrame";
			}
			Type type = typeFromHandle.MakeGenericType(typeArguments);
			object obj = Delegate.CreateDelegate(type, method, false);
			if (obj == null)
			{
				throw new MethodAccessException();
			}
			MethodInfo method2 = typeof(MonoProperty).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic);
			method2 = method2.MakeGenericMethod(typeArguments);
			return (GetterAdapter)Delegate.CreateDelegate(typeof(GetterAdapter), obj, method2, true);
		}

		public override object GetValue(object obj, object[] index)
		{
			if (index == null || index.Length == 0)
			{
			}
			return GetValue(obj, BindingFlags.Default, null, index, null);
		}

		public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
		{
			object obj2 = null;
			MethodInfo getMethod = GetGetMethod(true);
			if (getMethod == null)
			{
				throw new ArgumentException("Get Method not found for '" + Name + "'");
			}
			try
			{
				if (index == null || index.Length == 0)
				{
					return getMethod.Invoke(obj, invokeAttr, binder, null, culture);
				}
				return getMethod.Invoke(obj, invokeAttr, binder, index, culture);
			}
			catch (SecurityException inner)
			{
				throw new TargetInvocationException(inner);
			}
		}

		public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
		{
			MethodInfo setMethod = GetSetMethod(true);
			if (setMethod == null)
			{
				throw new ArgumentException("Set Method not found for '" + Name + "'");
			}
			object[] array;
			if (index == null || index.Length == 0)
			{
				array = new object[1] { value };
			}
			else
			{
				int num = index.Length;
				array = new object[num + 1];
				index.CopyTo(array, 0);
				array[num] = value;
			}
			setMethod.Invoke(obj, invokeAttr, binder, array, culture);
		}

		public override string ToString()
		{
			return PropertyType.ToString() + " " + Name;
		}

		public override Type[] GetOptionalCustomModifiers()
		{
			Type[] typeModifiers = MonoPropertyInfo.GetTypeModifiers(this, true);
			if (typeModifiers == null)
			{
				return Type.EmptyTypes;
			}
			return typeModifiers;
		}

		public override Type[] GetRequiredCustomModifiers()
		{
			Type[] typeModifiers = MonoPropertyInfo.GetTypeModifiers(this, false);
			if (typeModifiers == null)
			{
				return Type.EmptyTypes;
			}
			return typeModifiers;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			MemberInfoSerializationHolder.Serialize(info, Name, ReflectedType, ToString(), MemberTypes.Property);
		}
	}
}
