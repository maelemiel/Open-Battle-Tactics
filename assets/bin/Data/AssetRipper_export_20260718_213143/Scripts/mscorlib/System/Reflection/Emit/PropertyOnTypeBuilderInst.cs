using System.Globalization;

namespace System.Reflection.Emit
{
	internal class PropertyOnTypeBuilderInst : PropertyInfo
	{
		private MonoGenericClass instantiation;

		private PropertyInfo prop;

		public override PropertyAttributes Attributes
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override bool CanRead
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override bool CanWrite
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override Type PropertyType
		{
			get
			{
				return instantiation.InflateType(prop.PropertyType);
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return instantiation.InflateType(prop.DeclaringType);
			}
		}

		public override Type ReflectedType
		{
			get
			{
				return instantiation;
			}
		}

		public override string Name
		{
			get
			{
				return prop.Name;
			}
		}

		internal PropertyOnTypeBuilderInst(MonoGenericClass instantiation, PropertyInfo prop)
		{
			this.instantiation = instantiation;
			this.prop = prop;
		}

		public override MethodInfo[] GetAccessors(bool nonPublic)
		{
			MethodInfo getMethod = GetGetMethod(nonPublic);
			MethodInfo setMethod = GetSetMethod(nonPublic);
			int num = 0;
			if (getMethod != null)
			{
				num++;
			}
			if (setMethod != null)
			{
				num++;
			}
			MethodInfo[] array = new MethodInfo[num];
			num = 0;
			if (getMethod != null)
			{
				array[num++] = getMethod;
			}
			if (setMethod != null)
			{
				array[num] = setMethod;
			}
			return array;
		}

		public override MethodInfo GetGetMethod(bool nonPublic)
		{
			MethodInfo methodInfo = prop.GetGetMethod(nonPublic);
			if (methodInfo != null && prop.DeclaringType == instantiation.generic_type)
			{
				methodInfo = TypeBuilder.GetMethod(instantiation, methodInfo);
			}
			return methodInfo;
		}

		public override ParameterInfo[] GetIndexParameters()
		{
			MethodInfo getMethod = GetGetMethod(true);
			if (getMethod != null)
			{
				return getMethod.GetParameters();
			}
			return new ParameterInfo[0];
		}

		public override MethodInfo GetSetMethod(bool nonPublic)
		{
			MethodInfo methodInfo = prop.GetSetMethod(nonPublic);
			if (methodInfo != null && prop.DeclaringType == instantiation.generic_type)
			{
				methodInfo = TypeBuilder.GetMethod(instantiation, methodInfo);
			}
			return methodInfo;
		}

		public override string ToString()
		{
			return string.Format("{0} {1}", PropertyType, Name);
		}

		public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotSupportedException();
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}
	}
}
