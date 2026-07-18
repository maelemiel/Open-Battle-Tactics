using System.Globalization;

namespace System.Reflection.Emit
{
	internal class ConstructorOnTypeBuilderInst : ConstructorInfo
	{
		private MonoGenericClass instantiation;

		private ConstructorBuilder cb;

		public override Type DeclaringType
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
				return cb.Name;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				return instantiation;
			}
		}

		public override int MetadataToken
		{
			get
			{
				if (!((ModuleBuilder)cb.Module).assemblyb.IsCompilerContext)
				{
					return base.MetadataToken;
				}
				return cb.MetadataToken;
			}
		}

		public override RuntimeMethodHandle MethodHandle
		{
			get
			{
				return cb.MethodHandle;
			}
		}

		public override MethodAttributes Attributes
		{
			get
			{
				return cb.Attributes;
			}
		}

		public override CallingConventions CallingConvention
		{
			get
			{
				return cb.CallingConvention;
			}
		}

		public override bool ContainsGenericParameters
		{
			get
			{
				return false;
			}
		}

		public override bool IsGenericMethodDefinition
		{
			get
			{
				return false;
			}
		}

		public override bool IsGenericMethod
		{
			get
			{
				return false;
			}
		}

		public ConstructorOnTypeBuilderInst(MonoGenericClass instantiation, ConstructorBuilder cb)
		{
			this.instantiation = instantiation;
			this.cb = cb;
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return cb.IsDefined(attributeType, inherit);
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return cb.GetCustomAttributes(inherit);
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return cb.GetCustomAttributes(attributeType, inherit);
		}

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return cb.GetMethodImplementationFlags();
		}

		public override ParameterInfo[] GetParameters()
		{
			if (!((ModuleBuilder)cb.Module).assemblyb.IsCompilerContext && !instantiation.generic_type.is_created)
			{
				throw new NotSupportedException();
			}
			ParameterInfo[] array = new ParameterInfo[cb.parameters.Length];
			for (int i = 0; i < cb.parameters.Length; i++)
			{
				Type type = instantiation.InflateType(cb.parameters[i]);
				array[i] = new ParameterInfo((cb.pinfo != null) ? cb.pinfo[i] : null, type, this, i + 1);
			}
			return array;
		}

		internal override int GetParameterCount()
		{
			return cb.GetParameterCount();
		}

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			return cb.Invoke(obj, invokeAttr, binder, parameters, culture);
		}

		public override Type[] GetGenericArguments()
		{
			return cb.GetGenericArguments();
		}

		public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			throw new InvalidOperationException();
		}
	}
}
