using System.Globalization;

namespace System.Reflection
{
	internal class MonoArrayMethod : MethodInfo
	{
		internal RuntimeMethodHandle mhandle;

		internal Type parent;

		internal Type ret;

		internal Type[] parameters;

		internal string name;

		internal int table_idx;

		internal CallingConventions call_conv;

		public override Type ReturnType
		{
			get
			{
				return ret;
			}
		}

		[MonoTODO("Not implemented.  Always returns null")]
		public override ICustomAttributeProvider ReturnTypeCustomAttributes
		{
			get
			{
				return null;
			}
		}

		public override RuntimeMethodHandle MethodHandle
		{
			get
			{
				return mhandle;
			}
		}

		[MonoTODO("Not implemented.  Always returns zero")]
		public override MethodAttributes Attributes
		{
			get
			{
				return MethodAttributes.PrivateScope;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				return parent;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return parent;
			}
		}

		public override string Name
		{
			get
			{
				return name;
			}
		}

		internal MonoArrayMethod(Type arrayClass, string methodName, CallingConventions callingConvention, Type returnType, Type[] parameterTypes)
		{
			name = methodName;
			parent = arrayClass;
			ret = returnType;
			parameters = (Type[])parameterTypes.Clone();
			call_conv = callingConvention;
		}

		[MonoTODO("Always returns this")]
		public override MethodInfo GetBaseDefinition()
		{
			return this;
		}

		[MonoTODO("Not implemented.  Always returns zero")]
		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return MethodImplAttributes.IL;
		}

		[MonoTODO("Not implemented.  Always returns an empty array")]
		public override ParameterInfo[] GetParameters()
		{
			return new ParameterInfo[0];
		}

		[MonoTODO("Not implemented")]
		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.IsDefined(this, attributeType, inherit);
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, inherit);
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, attributeType, inherit);
		}

		public override string ToString()
		{
			string text = string.Empty;
			ParameterInfo[] array = GetParameters();
			for (int i = 0; i < array.Length; i++)
			{
				if (i > 0)
				{
					text += ", ";
				}
				text += array[i].ParameterType.Name;
			}
			if (ReturnType != null)
			{
				return ReturnType.Name + " " + Name + "(" + text + ")";
			}
			return "void " + Name + "(" + text + ")";
		}
	}
}
