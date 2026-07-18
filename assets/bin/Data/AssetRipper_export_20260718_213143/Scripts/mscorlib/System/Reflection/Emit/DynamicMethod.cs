using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	public sealed class DynamicMethod : MethodInfo
	{
		private class AnonHostModuleHolder
		{
			public static Module anon_host_module;

			static AnonHostModuleHolder()
			{
				AssemblyName name = new AssemblyName
				{
					Name = "Anonymously Hosted DynamicMethods Assembly"
				};
				AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
				anon_host_module = assemblyBuilder.GetManifestModule();
			}
		}

		private RuntimeMethodHandle mhandle;

		private string name;

		private Type returnType;

		private Type[] parameters;

		private MethodAttributes attributes;

		private CallingConventions callingConvention;

		private Module module;

		private bool skipVisibility;

		private bool init_locals = true;

		private ILGenerator ilgen;

		private int nrefs;

		private object[] refs;

		private IntPtr referenced_by;

		private Type owner;

		private Delegate deleg;

		private MonoMethod method;

		private ParameterBuilder[] pinfo;

		internal bool creating;

		public override MethodAttributes Attributes
		{
			get
			{
				return attributes;
			}
		}

		public override CallingConventions CallingConvention
		{
			get
			{
				return callingConvention;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return null;
			}
		}

		public bool InitLocals
		{
			get
			{
				return init_locals;
			}
			set
			{
				init_locals = value;
			}
		}

		public override RuntimeMethodHandle MethodHandle
		{
			get
			{
				return mhandle;
			}
		}

		public override Module Module
		{
			get
			{
				return module;
			}
		}

		public override string Name
		{
			get
			{
				return name;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				return null;
			}
		}

		[MonoTODO("Not implemented")]
		public override ParameterInfo ReturnParameter
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override Type ReturnType
		{
			get
			{
				return returnType;
			}
		}

		[MonoTODO("Not implemented")]
		public override ICustomAttributeProvider ReturnTypeCustomAttributes
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public DynamicMethod(string name, Type returnType, Type[] parameterTypes, Module m)
			: this(name, returnType, parameterTypes, m, false)
		{
		}

		public DynamicMethod(string name, Type returnType, Type[] parameterTypes, Type owner)
			: this(name, returnType, parameterTypes, owner, false)
		{
		}

		public DynamicMethod(string name, Type returnType, Type[] parameterTypes, Module m, bool skipVisibility)
			: this(name, MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, returnType, parameterTypes, m, skipVisibility)
		{
		}

		public DynamicMethod(string name, Type returnType, Type[] parameterTypes, Type owner, bool skipVisibility)
			: this(name, MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, returnType, parameterTypes, owner, skipVisibility)
		{
		}

		public DynamicMethod(string name, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type owner, bool skipVisibility)
			: this(name, attributes, callingConvention, returnType, parameterTypes, owner, owner.Module, skipVisibility, false)
		{
		}

		public DynamicMethod(string name, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Module m, bool skipVisibility)
			: this(name, attributes, callingConvention, returnType, parameterTypes, null, m, skipVisibility, false)
		{
		}

		public DynamicMethod(string name, Type returnType, Type[] parameterTypes)
			: this(name, returnType, parameterTypes, false)
		{
		}

		[MonoTODO("Visibility is not restricted")]
		public DynamicMethod(string name, Type returnType, Type[] parameterTypes, bool restrictedSkipVisibility)
			: this(name, MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, returnType, parameterTypes, null, null, restrictedSkipVisibility, true)
		{
		}

		private DynamicMethod(string name, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type owner, Module m, bool skipVisibility, bool anonHosted)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (returnType == null)
			{
				returnType = typeof(void);
			}
			if (m == null && !anonHosted)
			{
				throw new ArgumentNullException("m");
			}
			if (returnType.IsByRef)
			{
				throw new ArgumentException("Return type can't be a byref type", "returnType");
			}
			if (parameterTypes != null)
			{
				for (int i = 0; i < parameterTypes.Length; i++)
				{
					if (parameterTypes[i] == null)
					{
						throw new ArgumentException("Parameter " + i + " is null", "parameterTypes");
					}
				}
			}
			if (m == null)
			{
				m = AnonHostModuleHolder.anon_host_module;
			}
			this.name = name;
			this.attributes = attributes | MethodAttributes.Static;
			this.callingConvention = callingConvention;
			this.returnType = returnType;
			parameters = parameterTypes;
			this.owner = owner;
			module = m;
			this.skipVisibility = skipVisibility;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void create_dynamic_method(DynamicMethod m);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void destroy_dynamic_method(DynamicMethod m);

		private void CreateDynMethod()
		{
			if (!(mhandle.Value == IntPtr.Zero))
			{
				return;
			}
			if (ilgen == null || ILGenerator.Mono_GetCurrentOffset(ilgen) == 0)
			{
				throw new InvalidOperationException("Method '" + name + "' does not have a method body.");
			}
			ilgen.label_fixup();
			try
			{
				creating = true;
				if (refs != null)
				{
					for (int i = 0; i < refs.Length; i++)
					{
						if (refs[i] is DynamicMethod)
						{
							DynamicMethod dynamicMethod = (DynamicMethod)refs[i];
							if (!dynamicMethod.creating)
							{
								dynamicMethod.CreateDynMethod();
							}
						}
					}
				}
			}
			finally
			{
				creating = false;
			}
			create_dynamic_method(this);
		}

		~DynamicMethod()
		{
			destroy_dynamic_method(this);
		}

		[ComVisible(true)]
		public Delegate CreateDelegate(Type delegateType)
		{
			if (delegateType == null)
			{
				throw new ArgumentNullException("delegateType");
			}
			if ((object)deleg != null)
			{
				return deleg;
			}
			CreateDynMethod();
			deleg = Delegate.CreateDelegate(delegateType, this);
			return deleg;
		}

		[ComVisible(true)]
		public Delegate CreateDelegate(Type delegateType, object target)
		{
			if (delegateType == null)
			{
				throw new ArgumentNullException("delegateType");
			}
			CreateDynMethod();
			return Delegate.CreateDelegate(delegateType, target, this);
		}

		public ParameterBuilder DefineParameter(int position, ParameterAttributes attributes, string parameterName)
		{
			if (position < 0 || position > parameters.Length)
			{
				throw new ArgumentOutOfRangeException("position");
			}
			RejectIfCreated();
			ParameterBuilder parameterBuilder = new ParameterBuilder(this, position, attributes, parameterName);
			if (pinfo == null)
			{
				pinfo = new ParameterBuilder[parameters.Length + 1];
			}
			pinfo[position] = parameterBuilder;
			return parameterBuilder;
		}

		public override MethodInfo GetBaseDefinition()
		{
			return this;
		}

		[MonoTODO("Not implemented")]
		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Not implemented")]
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Not implemented")]
		public DynamicILInfo GetDynamicILInfo()
		{
			throw new NotImplementedException();
		}

		public ILGenerator GetILGenerator()
		{
			return GetILGenerator(64);
		}

		public ILGenerator GetILGenerator(int streamSize)
		{
			if ((GetMethodImplementationFlags() & MethodImplAttributes.CodeTypeMask) != MethodImplAttributes.IL || (GetMethodImplementationFlags() & MethodImplAttributes.ManagedMask) != MethodImplAttributes.IL)
			{
				throw new InvalidOperationException("Method body should not exist.");
			}
			if (ilgen != null)
			{
				return ilgen;
			}
			ilgen = new ILGenerator(Module, new DynamicMethodTokenGenerator(this), streamSize);
			return ilgen;
		}

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return MethodImplAttributes.IL;
		}

		public override ParameterInfo[] GetParameters()
		{
			if (parameters == null)
			{
				return new ParameterInfo[0];
			}
			ParameterInfo[] array = new ParameterInfo[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				array[i] = new ParameterInfo((pinfo != null) ? pinfo[i + 1] : null, parameters[i], this, i + 1);
			}
			return array;
		}

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			try
			{
				CreateDynMethod();
				if (method == null)
				{
					method = new MonoMethod(mhandle);
				}
				return method.Invoke(obj, parameters);
			}
			catch (MethodAccessException inner)
			{
				throw new TargetInvocationException("Method cannot be invoked.", inner);
			}
		}

		[MonoTODO("Not implemented")]
		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotImplementedException();
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
			return ReturnType.Name + " " + Name + "(" + text + ")";
		}

		private void RejectIfCreated()
		{
			if (mhandle.Value != IntPtr.Zero)
			{
				throw new InvalidOperationException("Type definition of the method is complete.");
			}
		}

		internal int AddRef(object reference)
		{
			if (refs == null)
			{
				refs = new object[4];
			}
			if (nrefs >= refs.Length - 1)
			{
				object[] destinationArray = new object[refs.Length * 2];
				Array.Copy(refs, destinationArray, refs.Length);
				refs = destinationArray;
			}
			refs[nrefs] = reference;
			refs[nrefs + 1] = null;
			nrefs += 2;
			return nrefs - 1;
		}
	}
}
