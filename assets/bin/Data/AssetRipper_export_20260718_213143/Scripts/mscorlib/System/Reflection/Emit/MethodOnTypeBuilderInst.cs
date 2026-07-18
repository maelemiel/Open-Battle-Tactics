using System.Globalization;
using System.Text;

namespace System.Reflection.Emit
{
	internal class MethodOnTypeBuilderInst : MethodInfo
	{
		private MonoGenericClass instantiation;

		internal MethodBuilder mb;

		private Type[] method_arguments;

		private MethodOnTypeBuilderInst generic_method_definition;

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
				return mb.Name;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				return instantiation;
			}
		}

		public override Type ReturnType
		{
			get
			{
				if (!((ModuleBuilder)mb.Module).assemblyb.IsCompilerContext)
				{
					return mb.ReturnType;
				}
				return instantiation.InflateType(mb.ReturnType, method_arguments);
			}
		}

		public override int MetadataToken
		{
			get
			{
				if (!((ModuleBuilder)mb.Module).assemblyb.IsCompilerContext)
				{
					return base.MetadataToken;
				}
				return mb.MetadataToken;
			}
		}

		public override RuntimeMethodHandle MethodHandle
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override MethodAttributes Attributes
		{
			get
			{
				return mb.Attributes;
			}
		}

		public override CallingConventions CallingConvention
		{
			get
			{
				return mb.CallingConvention;
			}
		}

		public override bool ContainsGenericParameters
		{
			get
			{
				if (mb.generic_params == null)
				{
					throw new NotSupportedException();
				}
				if (method_arguments == null)
				{
					return true;
				}
				Type[] array = method_arguments;
				foreach (Type type in array)
				{
					if (type.ContainsGenericParameters)
					{
						return true;
					}
				}
				return false;
			}
		}

		public override bool IsGenericMethodDefinition
		{
			get
			{
				return mb.generic_params != null && method_arguments == null;
			}
		}

		public override bool IsGenericMethod
		{
			get
			{
				return mb.generic_params != null;
			}
		}

		public override ICustomAttributeProvider ReturnTypeCustomAttributes
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public MethodOnTypeBuilderInst(MonoGenericClass instantiation, MethodBuilder mb)
		{
			this.instantiation = instantiation;
			this.mb = mb;
		}

		internal MethodOnTypeBuilderInst(MethodOnTypeBuilderInst gmd, Type[] typeArguments)
		{
			instantiation = gmd.instantiation;
			mb = gmd.mb;
			method_arguments = new Type[typeArguments.Length];
			typeArguments.CopyTo(method_arguments, 0);
			generic_method_definition = gmd;
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

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(ReturnType.ToString());
			stringBuilder.Append(" ");
			stringBuilder.Append(mb.Name);
			stringBuilder.Append("(");
			if (((ModuleBuilder)mb.Module).assemblyb.IsCompilerContext)
			{
				ParameterInfo[] parameters = GetParameters();
				for (int i = 0; i < parameters.Length; i++)
				{
					if (i > 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(parameters[i].ParameterType);
				}
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return mb.GetMethodImplementationFlags();
		}

		public override ParameterInfo[] GetParameters()
		{
			if (!((ModuleBuilder)mb.Module).assemblyb.IsCompilerContext)
			{
				throw new NotSupportedException();
			}
			ParameterInfo[] array = new ParameterInfo[mb.parameters.Length];
			for (int i = 0; i < mb.parameters.Length; i++)
			{
				Type type = instantiation.InflateType(mb.parameters[i], method_arguments);
				array[i] = new ParameterInfo((mb.pinfo != null) ? mb.pinfo[i + 1] : null, type, this, i + 1);
			}
			return array;
		}

		internal override int GetParameterCount()
		{
			return mb.GetParameterCount();
		}

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		public override MethodInfo MakeGenericMethod(params Type[] typeArguments)
		{
			if (mb.generic_params == null || method_arguments != null)
			{
				throw new NotSupportedException();
			}
			if (typeArguments == null)
			{
				throw new ArgumentNullException("typeArguments");
			}
			foreach (Type type in typeArguments)
			{
				if (type == null)
				{
					throw new ArgumentNullException("typeArguments");
				}
			}
			if (mb.generic_params.Length != typeArguments.Length)
			{
				throw new ArgumentException("Invalid argument array length");
			}
			return new MethodOnTypeBuilderInst(this, typeArguments);
		}

		public override Type[] GetGenericArguments()
		{
			if (mb.generic_params == null)
			{
				return null;
			}
			Type[] array = method_arguments ?? mb.generic_params;
			Type[] array2 = new Type[array.Length];
			array.CopyTo(array2, 0);
			return array2;
		}

		public override MethodInfo GetGenericMethodDefinition()
		{
			return (MethodInfo)(((object)generic_method_definition) ?? ((object)mb));
		}

		public override MethodInfo GetBaseDefinition()
		{
			throw new NotSupportedException();
		}
	}
}
