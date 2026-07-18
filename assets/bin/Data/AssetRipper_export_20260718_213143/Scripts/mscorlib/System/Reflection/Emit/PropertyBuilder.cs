using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComDefaultInterface(typeof(_PropertyBuilder))]
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	public sealed class PropertyBuilder : PropertyInfo, _PropertyBuilder
	{
		private PropertyAttributes attrs;

		private string name;

		private Type type;

		private Type[] parameters;

		private CustomAttributeBuilder[] cattrs;

		private object def_value;

		private MethodBuilder set_method;

		private MethodBuilder get_method;

		private int table_idx;

		internal TypeBuilder typeb;

		private Type[] returnModReq;

		private Type[] returnModOpt;

		private Type[][] paramModReq;

		private Type[][] paramModOpt;

		public override PropertyAttributes Attributes
		{
			get
			{
				return attrs;
			}
		}

		public override bool CanRead
		{
			get
			{
				return get_method != null;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return set_method != null;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return typeb;
			}
		}

		public override string Name
		{
			get
			{
				return name;
			}
		}

		public PropertyToken PropertyToken
		{
			get
			{
				return default(PropertyToken);
			}
		}

		public override Type PropertyType
		{
			get
			{
				return type;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				return typeb;
			}
		}

		public override Module Module
		{
			get
			{
				return base.Module;
			}
		}

		internal PropertyBuilder(TypeBuilder tb, string name, PropertyAttributes attributes, Type returnType, Type[] returnModReq, Type[] returnModOpt, Type[] parameterTypes, Type[][] paramModReq, Type[][] paramModOpt)
		{
			this.name = name;
			attrs = attributes;
			type = returnType;
			this.returnModReq = returnModReq;
			this.returnModOpt = returnModOpt;
			this.paramModReq = paramModReq;
			this.paramModOpt = paramModOpt;
			if (parameterTypes != null)
			{
				parameters = new Type[parameterTypes.Length];
				Array.Copy(parameterTypes, parameters, parameters.Length);
			}
			typeb = tb;
			table_idx = tb.get_next_table_index(this, 23, true);
		}

		void _PropertyBuilder.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _PropertyBuilder.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _PropertyBuilder.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _PropertyBuilder.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		public void AddOtherMethod(MethodBuilder mdBuilder)
		{
		}

		public override MethodInfo[] GetAccessors(bool nonPublic)
		{
			return null;
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			throw not_supported();
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw not_supported();
		}

		public override MethodInfo GetGetMethod(bool nonPublic)
		{
			return get_method;
		}

		public override ParameterInfo[] GetIndexParameters()
		{
			throw not_supported();
		}

		public override MethodInfo GetSetMethod(bool nonPublic)
		{
			return set_method;
		}

		public override object GetValue(object obj, object[] index)
		{
			return null;
		}

		public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
		{
			throw not_supported();
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw not_supported();
		}

		public void SetConstant(object defaultValue)
		{
			def_value = defaultValue;
		}

		public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
		{
			string fullName = customBuilder.Ctor.ReflectedType.FullName;
			if (fullName == "System.Runtime.CompilerServices.SpecialNameAttribute")
			{
				attrs |= PropertyAttributes.SpecialName;
			}
			else if (cattrs != null)
			{
				CustomAttributeBuilder[] array = new CustomAttributeBuilder[cattrs.Length + 1];
				cattrs.CopyTo(array, 0);
				array[cattrs.Length] = customBuilder;
				cattrs = array;
			}
			else
			{
				cattrs = new CustomAttributeBuilder[1];
				cattrs[0] = customBuilder;
			}
		}

		[ComVisible(true)]
		public void SetCustomAttribute(ConstructorInfo con, byte[] binaryAttribute)
		{
			SetCustomAttribute(new CustomAttributeBuilder(con, binaryAttribute));
		}

		public void SetGetMethod(MethodBuilder mdBuilder)
		{
			get_method = mdBuilder;
		}

		public void SetSetMethod(MethodBuilder mdBuilder)
		{
			set_method = mdBuilder;
		}

		public override void SetValue(object obj, object value, object[] index)
		{
		}

		public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
		{
		}

		private Exception not_supported()
		{
			return new NotSupportedException("The invoked member is not supported in a dynamic module.");
		}
	}
}
