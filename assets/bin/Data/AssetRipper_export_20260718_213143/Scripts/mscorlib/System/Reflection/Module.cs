using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Reflection
{
	[Serializable]
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	[ComDefaultInterface(typeof(_Module))]
	public class Module : ISerializable, ICustomAttributeProvider, _Module
	{
		private const BindingFlags defaultBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

		public static readonly TypeFilter FilterTypeName;

		public static readonly TypeFilter FilterTypeNameIgnoreCase;

		private IntPtr _impl;

		internal Assembly assembly;

		internal string fqname;

		internal string name;

		internal string scopename;

		internal bool is_resource;

		internal int token;

		public Assembly Assembly
		{
			get
			{
				return assembly;
			}
		}

		public virtual string FullyQualifiedName
		{
			get
			{
				return fqname;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public string ScopeName
		{
			get
			{
				return scopename;
			}
		}

		public ModuleHandle ModuleHandle
		{
			get
			{
				return new ModuleHandle(_impl);
			}
		}

		public extern int MetadataToken
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public int MDStreamVersion
		{
			get
			{
				if (_impl == IntPtr.Zero)
				{
					throw new NotSupportedException();
				}
				return GetMDStreamVersion(_impl);
			}
		}

		internal Guid MvId
		{
			get
			{
				return GetModuleVersionId();
			}
		}

		public Guid ModuleVersionId
		{
			get
			{
				return GetModuleVersionId();
			}
		}

		internal Module()
		{
		}

		static Module()
		{
			FilterTypeName = filter_by_type_name;
			FilterTypeNameIgnoreCase = filter_by_type_name_ignore_case;
		}

		void _Module.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _Module.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _Module.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _Module.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetMDStreamVersion(IntPtr module_handle);

		public virtual Type[] FindTypes(TypeFilter filter, object filterCriteria)
		{
			ArrayList arrayList = new ArrayList();
			Type[] types = GetTypes();
			Type[] array = types;
			foreach (Type type in array)
			{
				if (filter(type, filterCriteria))
				{
					arrayList.Add(type);
				}
			}
			return (Type[])arrayList.ToArray(typeof(Type));
		}

		public virtual object[] GetCustomAttributes(bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, inherit);
		}

		public virtual object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, attributeType, inherit);
		}

		public FieldInfo GetField(string name)
		{
			if (IsResource())
			{
				return null;
			}
			Type globalType = GetGlobalType();
			return (globalType == null) ? null : globalType.GetField(name, BindingFlags.Static | BindingFlags.Public);
		}

		public FieldInfo GetField(string name, BindingFlags bindingAttr)
		{
			if (IsResource())
			{
				return null;
			}
			Type globalType = GetGlobalType();
			return (globalType == null) ? null : globalType.GetField(name, bindingAttr);
		}

		public FieldInfo[] GetFields()
		{
			if (IsResource())
			{
				return new FieldInfo[0];
			}
			Type globalType = GetGlobalType();
			return (globalType == null) ? new FieldInfo[0] : globalType.GetFields(BindingFlags.Static | BindingFlags.Public);
		}

		public MethodInfo GetMethod(string name)
		{
			if (IsResource())
			{
				return null;
			}
			Type globalType = GetGlobalType();
			return (globalType == null) ? null : globalType.GetMethod(name);
		}

		public MethodInfo GetMethod(string name, Type[] types)
		{
			return GetMethodImpl(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Any, types, null);
		}

		public MethodInfo GetMethod(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			return GetMethodImpl(name, bindingAttr, binder, callConvention, types, modifiers);
		}

		protected virtual MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			if (IsResource())
			{
				return null;
			}
			Type globalType = GetGlobalType();
			return (globalType == null) ? null : globalType.GetMethod(name, bindingAttr, binder, callConvention, types, modifiers);
		}

		public MethodInfo[] GetMethods()
		{
			if (IsResource())
			{
				return new MethodInfo[0];
			}
			Type globalType = GetGlobalType();
			return (globalType == null) ? new MethodInfo[0] : globalType.GetMethods();
		}

		public MethodInfo[] GetMethods(BindingFlags bindingFlags)
		{
			if (IsResource())
			{
				return new MethodInfo[0];
			}
			Type globalType = GetGlobalType();
			return (globalType == null) ? new MethodInfo[0] : globalType.GetMethods(bindingFlags);
		}

		public FieldInfo[] GetFields(BindingFlags bindingFlags)
		{
			if (IsResource())
			{
				return new FieldInfo[0];
			}
			Type globalType = GetGlobalType();
			return (globalType == null) ? new FieldInfo[0] : globalType.GetFields(bindingFlags);
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			UnitySerializationHolder.GetModuleData(this, info, context);
		}

		[ComVisible(true)]
		public virtual Type GetType(string className)
		{
			return GetType(className, false, false);
		}

		[ComVisible(true)]
		public virtual Type GetType(string className, bool ignoreCase)
		{
			return GetType(className, false, ignoreCase);
		}

		[ComVisible(true)]
		public virtual Type GetType(string className, bool throwOnError, bool ignoreCase)
		{
			if (className == null)
			{
				throw new ArgumentNullException("className");
			}
			if (className == string.Empty)
			{
				throw new ArgumentException("Type name can't be empty");
			}
			return assembly.InternalGetType(this, className, throwOnError, ignoreCase);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Type[] InternalGetTypes();

		public virtual Type[] GetTypes()
		{
			return InternalGetTypes();
		}

		public virtual bool IsDefined(Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.IsDefined(this, attributeType, inherit);
		}

		public bool IsResource()
		{
			return is_resource;
		}

		public override string ToString()
		{
			return name;
		}

		public void GetPEKind(out PortableExecutableKinds peKind, out ImageFileMachine machine)
		{
			ModuleHandle.GetPEKind(out peKind, out machine);
		}

		private Exception resolve_token_exception(int metadataToken, ResolveTokenError error, string tokenType)
		{
			if (error == ResolveTokenError.OutOfRange)
			{
				return new ArgumentOutOfRangeException("metadataToken", string.Format("Token 0x{0:x} is not valid in the scope of module {1}", metadataToken, name));
			}
			return new ArgumentException(string.Format("Token 0x{0:x} is not a valid {1} token in the scope of module {2}", metadataToken, tokenType, name), "metadataToken");
		}

		private IntPtr[] ptrs_from_types(Type[] types)
		{
			if (types == null)
			{
				return null;
			}
			IntPtr[] array = new IntPtr[types.Length];
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i] == null)
				{
					throw new ArgumentException();
				}
				array[i] = types[i].TypeHandle.Value;
			}
			return array;
		}

		public FieldInfo ResolveField(int metadataToken)
		{
			return ResolveField(metadataToken, null, null);
		}

		public FieldInfo ResolveField(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			ResolveTokenError error;
			IntPtr intPtr = ResolveFieldToken(_impl, metadataToken, ptrs_from_types(genericTypeArguments), ptrs_from_types(genericMethodArguments), out error);
			if (intPtr == IntPtr.Zero)
			{
				throw resolve_token_exception(metadataToken, error, "Field");
			}
			return FieldInfo.GetFieldFromHandle(new RuntimeFieldHandle(intPtr));
		}

		public MemberInfo ResolveMember(int metadataToken)
		{
			return ResolveMember(metadataToken, null, null);
		}

		public MemberInfo ResolveMember(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			ResolveTokenError error;
			MemberInfo memberInfo = ResolveMemberToken(_impl, metadataToken, ptrs_from_types(genericTypeArguments), ptrs_from_types(genericMethodArguments), out error);
			if (memberInfo == null)
			{
				throw resolve_token_exception(metadataToken, error, "MemberInfo");
			}
			return memberInfo;
		}

		public MethodBase ResolveMethod(int metadataToken)
		{
			return ResolveMethod(metadataToken, null, null);
		}

		public MethodBase ResolveMethod(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			ResolveTokenError error;
			IntPtr intPtr = ResolveMethodToken(_impl, metadataToken, ptrs_from_types(genericTypeArguments), ptrs_from_types(genericMethodArguments), out error);
			if (intPtr == IntPtr.Zero)
			{
				throw resolve_token_exception(metadataToken, error, "MethodBase");
			}
			return MethodBase.GetMethodFromHandleNoGenericCheck(new RuntimeMethodHandle(intPtr));
		}

		public string ResolveString(int metadataToken)
		{
			ResolveTokenError error;
			string text = ResolveStringToken(_impl, metadataToken, out error);
			if (text == null)
			{
				throw resolve_token_exception(metadataToken, error, "string");
			}
			return text;
		}

		public Type ResolveType(int metadataToken)
		{
			return ResolveType(metadataToken, null, null);
		}

		public Type ResolveType(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			ResolveTokenError error;
			IntPtr intPtr = ResolveTypeToken(_impl, metadataToken, ptrs_from_types(genericTypeArguments), ptrs_from_types(genericMethodArguments), out error);
			if (intPtr == IntPtr.Zero)
			{
				throw resolve_token_exception(metadataToken, error, "Type");
			}
			return Type.GetTypeFromHandle(new RuntimeTypeHandle(intPtr));
		}

		public byte[] ResolveSignature(int metadataToken)
		{
			ResolveTokenError error;
			byte[] array = ResolveSignature(_impl, metadataToken, out error);
			if (array == null)
			{
				throw resolve_token_exception(metadataToken, error, "signature");
			}
			return array;
		}

		internal static Type MonoDebugger_ResolveType(Module module, int token)
		{
			ResolveTokenError error;
			IntPtr intPtr = ResolveTypeToken(module._impl, token, null, null, out error);
			if (intPtr == IntPtr.Zero)
			{
				return null;
			}
			return Type.GetTypeFromHandle(new RuntimeTypeHandle(intPtr));
		}

		internal static Guid Mono_GetGuid(Module module)
		{
			return module.GetModuleVersionId();
		}

		internal virtual Guid GetModuleVersionId()
		{
			return new Guid(GetGuidInternal());
		}

		private static bool filter_by_type_name(Type m, object filterCriteria)
		{
			string text = (string)filterCriteria;
			if (text.EndsWith("*"))
			{
				return m.Name.StartsWith(text.Substring(0, text.Length - 1));
			}
			return m.Name == text;
		}

		private static bool filter_by_type_name_ignore_case(Type m, object filterCriteria)
		{
			string text = (string)filterCriteria;
			if (text.EndsWith("*"))
			{
				return m.Name.ToLower().StartsWith(text.Substring(0, text.Length - 1).ToLower());
			}
			return string.Compare(m.Name, text, true) == 0;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern IntPtr GetHINSTANCE();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string GetGuidInternal();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Type GetGlobalType();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr ResolveTypeToken(IntPtr module, int token, IntPtr[] type_args, IntPtr[] method_args, out ResolveTokenError error);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr ResolveMethodToken(IntPtr module, int token, IntPtr[] type_args, IntPtr[] method_args, out ResolveTokenError error);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr ResolveFieldToken(IntPtr module, int token, IntPtr[] type_args, IntPtr[] method_args, out ResolveTokenError error);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string ResolveStringToken(IntPtr module, int token, out ResolveTokenError error);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern MemberInfo ResolveMemberToken(IntPtr module, int token, IntPtr[] type_args, IntPtr[] method_args, out ResolveTokenError error);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern byte[] ResolveSignature(IntPtr module, int metadataToken, out ResolveTokenError error);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void GetPEKind(IntPtr module, out PortableExecutableKinds peKind, out ImageFileMachine machine);
	}
}
