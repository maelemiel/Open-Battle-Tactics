using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[ComDefaultInterface(typeof(_Type))]
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	public abstract class Type : MemberInfo, IReflect, _Type
	{
		internal const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

		internal RuntimeTypeHandle _impl;

		public static readonly char Delimiter = '.';

		public static readonly Type[] EmptyTypes = new Type[0];

		public static readonly MemberFilter FilterAttribute = FilterAttribute_impl;

		public static readonly MemberFilter FilterName = FilterName_impl;

		public static readonly MemberFilter FilterNameIgnoreCase = FilterNameIgnoreCase_impl;

		public static readonly object Missing = System.Reflection.Missing.Value;

		public abstract Assembly Assembly { get; }

		public abstract string AssemblyQualifiedName { get; }

		public TypeAttributes Attributes
		{
			get
			{
				return GetAttributeFlagsImpl();
			}
		}

		public abstract Type BaseType { get; }

		public override Type DeclaringType
		{
			get
			{
				return null;
			}
		}

		public static Binder DefaultBinder
		{
			get
			{
				return Binder.DefaultBinder;
			}
		}

		public abstract string FullName { get; }

		public abstract Guid GUID { get; }

		public bool HasElementType
		{
			get
			{
				return HasElementTypeImpl();
			}
		}

		public bool IsAbstract
		{
			get
			{
				return (Attributes & TypeAttributes.Abstract) != 0;
			}
		}

		public bool IsAnsiClass
		{
			get
			{
				return (Attributes & TypeAttributes.StringFormatMask) == 0;
			}
		}

		public bool IsArray
		{
			get
			{
				return IsArrayImpl();
			}
		}

		public bool IsAutoClass
		{
			get
			{
				return (Attributes & TypeAttributes.StringFormatMask) == TypeAttributes.AutoClass;
			}
		}

		public bool IsAutoLayout
		{
			get
			{
				return (Attributes & TypeAttributes.LayoutMask) == 0;
			}
		}

		public bool IsByRef
		{
			get
			{
				return IsByRefImpl();
			}
		}

		public bool IsClass
		{
			get
			{
				if (IsInterface)
				{
					return false;
				}
				return !IsValueType;
			}
		}

		public bool IsCOMObject
		{
			get
			{
				return IsCOMObjectImpl();
			}
		}

		public bool IsContextful
		{
			get
			{
				return IsContextfulImpl();
			}
		}

		public bool IsEnum
		{
			get
			{
				return IsSubclassOf(typeof(Enum));
			}
		}

		public bool IsExplicitLayout
		{
			get
			{
				return (Attributes & TypeAttributes.LayoutMask) == TypeAttributes.ExplicitLayout;
			}
		}

		public bool IsImport
		{
			get
			{
				return (Attributes & TypeAttributes.Import) != 0;
			}
		}

		public bool IsInterface
		{
			get
			{
				return (Attributes & TypeAttributes.ClassSemanticsMask) == TypeAttributes.ClassSemanticsMask;
			}
		}

		public bool IsLayoutSequential
		{
			get
			{
				return (Attributes & TypeAttributes.LayoutMask) == TypeAttributes.SequentialLayout;
			}
		}

		public bool IsMarshalByRef
		{
			get
			{
				return IsMarshalByRefImpl();
			}
		}

		public bool IsNestedAssembly
		{
			get
			{
				return (Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedAssembly;
			}
		}

		public bool IsNestedFamANDAssem
		{
			get
			{
				return (Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedFamANDAssem;
			}
		}

		public bool IsNestedFamily
		{
			get
			{
				return (Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedFamily;
			}
		}

		public bool IsNestedFamORAssem
		{
			get
			{
				return (Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.VisibilityMask;
			}
		}

		public bool IsNestedPrivate
		{
			get
			{
				return (Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedPrivate;
			}
		}

		public bool IsNestedPublic
		{
			get
			{
				return (Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NestedPublic;
			}
		}

		public bool IsNotPublic
		{
			get
			{
				return (Attributes & TypeAttributes.VisibilityMask) == 0;
			}
		}

		public bool IsPointer
		{
			get
			{
				return IsPointerImpl();
			}
		}

		public bool IsPrimitive
		{
			get
			{
				return IsPrimitiveImpl();
			}
		}

		public bool IsPublic
		{
			get
			{
				return (Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.Public;
			}
		}

		public bool IsSealed
		{
			get
			{
				return (Attributes & TypeAttributes.Sealed) != 0;
			}
		}

		public bool IsSerializable
		{
			get
			{
				if ((Attributes & TypeAttributes.Serializable) != TypeAttributes.NotPublic)
				{
					return true;
				}
				Type type = UnderlyingSystemType;
				if (type == null)
				{
					return false;
				}
				if (type.IsSystemType)
				{
					return type_is_subtype_of(type, typeof(Enum), false) || type_is_subtype_of(type, typeof(Delegate), false);
				}
				do
				{
					if (type == typeof(Enum) || type == typeof(Delegate))
					{
						return true;
					}
					type = type.BaseType;
				}
				while (type != null);
				return false;
			}
		}

		public bool IsSpecialName
		{
			get
			{
				return (Attributes & TypeAttributes.SpecialName) != 0;
			}
		}

		public bool IsUnicodeClass
		{
			get
			{
				return (Attributes & TypeAttributes.StringFormatMask) == TypeAttributes.UnicodeClass;
			}
		}

		public bool IsValueType
		{
			get
			{
				return IsValueTypeImpl();
			}
		}

		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.TypeInfo;
			}
		}

		public abstract override Module Module { get; }

		public abstract string Namespace { get; }

		public override Type ReflectedType
		{
			get
			{
				return null;
			}
		}

		public virtual RuntimeTypeHandle TypeHandle
		{
			get
			{
				return default(RuntimeTypeHandle);
			}
		}

		[ComVisible(true)]
		public ConstructorInfo TypeInitializer
		{
			get
			{
				return GetConstructorImpl(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, CallingConventions.Any, EmptyTypes, null);
			}
		}

		public abstract Type UnderlyingSystemType { get; }

		internal bool IsSystemType
		{
			get
			{
				return _impl.Value != IntPtr.Zero;
			}
		}

		public virtual bool ContainsGenericParameters
		{
			get
			{
				return false;
			}
		}

		public virtual extern bool IsGenericTypeDefinition
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public virtual extern bool IsGenericType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public virtual bool IsGenericParameter
		{
			get
			{
				return false;
			}
		}

		public bool IsNested
		{
			get
			{
				return DeclaringType != null;
			}
		}

		public bool IsVisible
		{
			get
			{
				if (IsNestedPublic)
				{
					return DeclaringType.IsVisible;
				}
				return IsPublic;
			}
		}

		public virtual int GenericParameterPosition
		{
			get
			{
				int genericParameterPosition = GetGenericParameterPosition();
				if (genericParameterPosition < 0)
				{
					throw new InvalidOperationException();
				}
				return genericParameterPosition;
			}
		}

		public virtual GenericParameterAttributes GenericParameterAttributes
		{
			get
			{
				if (!IsGenericParameter)
				{
					throw new InvalidOperationException();
				}
				return GetGenericParameterAttributes();
			}
		}

		public virtual MethodBase DeclaringMethod
		{
			get
			{
				return null;
			}
		}

		public virtual StructLayoutAttribute StructLayoutAttribute
		{
			get
			{
				LayoutKind layoutKind = ((!IsLayoutSequential) ? ((!IsExplicitLayout) ? LayoutKind.Auto : LayoutKind.Explicit) : LayoutKind.Sequential);
				StructLayoutAttribute structLayoutAttribute = new StructLayoutAttribute(layoutKind);
				if (IsUnicodeClass)
				{
					structLayoutAttribute.CharSet = CharSet.Unicode;
				}
				else if (IsAnsiClass)
				{
					structLayoutAttribute.CharSet = CharSet.Ansi;
				}
				else
				{
					structLayoutAttribute.CharSet = CharSet.Auto;
				}
				if (layoutKind != LayoutKind.Auto)
				{
					GetPacking(out structLayoutAttribute.Pack, out structLayoutAttribute.Size);
				}
				return structLayoutAttribute;
			}
		}

		internal bool IsUserType
		{
			get
			{
				return _impl.Value == IntPtr.Zero && (GetType().Assembly != typeof(Type).Assembly || GetType() == typeof(TypeDelegator));
			}
		}

		void _Type.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _Type.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _Type.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _Type.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		private static bool FilterName_impl(MemberInfo m, object filterCriteria)
		{
			string text = (string)filterCriteria;
			if (text == null || text.Length == 0)
			{
				return false;
			}
			if (text[text.Length - 1] == '*')
			{
				return string.Compare(text, 0, m.Name, 0, text.Length - 1, false, CultureInfo.InvariantCulture) == 0;
			}
			return text.Equals(m.Name);
		}

		private static bool FilterNameIgnoreCase_impl(MemberInfo m, object filterCriteria)
		{
			string text = (string)filterCriteria;
			if (text == null || text.Length == 0)
			{
				return false;
			}
			if (text[text.Length - 1] == '*')
			{
				return string.Compare(text, 0, m.Name, 0, text.Length - 1, true, CultureInfo.InvariantCulture) == 0;
			}
			return string.Compare(text, m.Name, true, CultureInfo.InvariantCulture) == 0;
		}

		private static bool FilterAttribute_impl(MemberInfo m, object filterCriteria)
		{
			int num = ((IConvertible)filterCriteria).ToInt32(null);
			if (m is MethodInfo)
			{
				return ((uint)((MethodInfo)m).Attributes & (uint)num) != 0;
			}
			if (m is FieldInfo)
			{
				return ((uint)((FieldInfo)m).Attributes & (uint)num) != 0;
			}
			if (m is PropertyInfo)
			{
				return ((uint)((PropertyInfo)m).Attributes & (uint)num) != 0;
			}
			if (m is EventInfo)
			{
				return ((uint)((EventInfo)m).Attributes & (uint)num) != 0;
			}
			return false;
		}

		public override bool Equals(object o)
		{
			if (o == null)
			{
				return false;
			}
			Type type = o as Type;
			if (type == null)
			{
				return false;
			}
			return Equals(type);
		}

		public bool Equals(Type o)
		{
			if (o == null)
			{
				return false;
			}
			return UnderlyingSystemType.EqualsInternal(o.UnderlyingSystemType);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool EqualsInternal(Type type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Type internal_from_handle(IntPtr handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Type internal_from_name(string name, bool throwOnError, bool ignoreCase);

		public static Type GetType(string typeName)
		{
			if (typeName == null)
			{
				throw new ArgumentNullException("TypeName");
			}
			return internal_from_name(typeName, false, false);
		}

		public static Type GetType(string typeName, bool throwOnError)
		{
			if (typeName == null)
			{
				throw new ArgumentNullException("TypeName");
			}
			Type type = internal_from_name(typeName, throwOnError, false);
			if (throwOnError && type == null)
			{
				throw new TypeLoadException("Error loading '" + typeName + "'");
			}
			return type;
		}

		public static Type GetType(string typeName, bool throwOnError, bool ignoreCase)
		{
			if (typeName == null)
			{
				throw new ArgumentNullException("TypeName");
			}
			Type type = internal_from_name(typeName, throwOnError, ignoreCase);
			if (throwOnError && type == null)
			{
				throw new TypeLoadException("Error loading '" + typeName + "'");
			}
			return type;
		}

		public static Type[] GetTypeArray(object[] args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			Type[] array = new Type[args.Length];
			for (int i = 0; i < args.Length; i++)
			{
				array[i] = args[i].GetType();
			}
			return array;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern TypeCode GetTypeCodeInternal(Type type);

		public static TypeCode GetTypeCode(Type type)
		{
			if (type is MonoType)
			{
				return GetTypeCodeInternal(type);
			}
			if (type == null)
			{
				return TypeCode.Empty;
			}
			type = type.UnderlyingSystemType;
			if (!type.IsSystemType)
			{
				return TypeCode.Object;
			}
			return GetTypeCodeInternal(type);
		}

		[MonoTODO("This operation is currently not supported by Mono")]
		public static Type GetTypeFromCLSID(Guid clsid)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("This operation is currently not supported by Mono")]
		public static Type GetTypeFromCLSID(Guid clsid, bool throwOnError)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("This operation is currently not supported by Mono")]
		public static Type GetTypeFromCLSID(Guid clsid, string server)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("This operation is currently not supported by Mono")]
		public static Type GetTypeFromCLSID(Guid clsid, string server, bool throwOnError)
		{
			throw new NotImplementedException();
		}

		public static Type GetTypeFromHandle(RuntimeTypeHandle handle)
		{
			if (handle.Value == IntPtr.Zero)
			{
				return null;
			}
			return internal_from_handle(handle.Value);
		}

		[MonoTODO("Mono does not support COM")]
		public static Type GetTypeFromProgID(string progID)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Mono does not support COM")]
		public static Type GetTypeFromProgID(string progID, bool throwOnError)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Mono does not support COM")]
		public static Type GetTypeFromProgID(string progID, string server)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Mono does not support COM")]
		public static Type GetTypeFromProgID(string progID, string server, bool throwOnError)
		{
			throw new NotImplementedException();
		}

		public static RuntimeTypeHandle GetTypeHandle(object o)
		{
			if (o == null)
			{
				throw new ArgumentNullException();
			}
			return o.GetType().TypeHandle;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool type_is_subtype_of(Type a, Type b, bool check_interfaces);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool type_is_assignable_from(Type a, Type b);

		public new Type GetType()
		{
			return base.GetType();
		}

		[ComVisible(true)]
		public virtual bool IsSubclassOf(Type c)
		{
			if (c == null || c == this)
			{
				return false;
			}
			if (IsSystemType)
			{
				return c.IsSystemType && type_is_subtype_of(this, c, false);
			}
			for (Type baseType = BaseType; baseType != null; baseType = baseType.BaseType)
			{
				if (baseType == c)
				{
					return true;
				}
			}
			return false;
		}

		public virtual Type[] FindInterfaces(TypeFilter filter, object filterCriteria)
		{
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			ArrayList arrayList = new ArrayList();
			Type[] interfaces = GetInterfaces();
			foreach (Type type in interfaces)
			{
				if (filter(type, filterCriteria))
				{
					arrayList.Add(type);
				}
			}
			return (Type[])arrayList.ToArray(typeof(Type));
		}

		public Type GetInterface(string name)
		{
			return GetInterface(name, false);
		}

		public abstract Type GetInterface(string name, bool ignoreCase);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void GetInterfaceMapData(Type t, Type iface, out MethodInfo[] targets, out MethodInfo[] methods);

		[ComVisible(true)]
		public virtual InterfaceMapping GetInterfaceMap(Type interfaceType)
		{
			if (interfaceType == null)
			{
				throw new ArgumentNullException("interfaceType");
			}
			if (!interfaceType.IsInterface)
			{
				throw new ArgumentException(Locale.GetText("Argument must be an interface."), "interfaceType");
			}
			if (IsInterface)
			{
				throw new ArgumentException("'this' type cannot be an interface itself");
			}
			InterfaceMapping result = default(InterfaceMapping);
			result.TargetType = this;
			result.InterfaceType = interfaceType;
			GetInterfaceMapData(this, interfaceType, out result.TargetMethods, out result.InterfaceMethods);
			if (result.TargetMethods == null)
			{
				throw new ArgumentException(Locale.GetText("Interface not found"), "interfaceType");
			}
			return result;
		}

		public abstract Type[] GetInterfaces();

		public virtual bool IsAssignableFrom(Type c)
		{
			if (c == null)
			{
				return false;
			}
			if (Equals(c))
			{
				return true;
			}
			if (c is TypeBuilder)
			{
				return ((TypeBuilder)c).IsAssignableTo(this);
			}
			if (!IsSystemType)
			{
				Type underlyingSystemType = UnderlyingSystemType;
				if (!underlyingSystemType.IsSystemType)
				{
					return false;
				}
				return underlyingSystemType.IsAssignableFrom(c);
			}
			if (!c.IsSystemType)
			{
				Type underlyingSystemType2 = c.UnderlyingSystemType;
				if (!underlyingSystemType2.IsSystemType)
				{
					return false;
				}
				return IsAssignableFrom(underlyingSystemType2);
			}
			return type_is_assignable_from(this, c);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern bool IsInstanceOfType(object o);

		public virtual int GetArrayRank()
		{
			throw new NotSupportedException();
		}

		public abstract Type GetElementType();

		public EventInfo GetEvent(string name)
		{
			return GetEvent(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		public abstract EventInfo GetEvent(string name, BindingFlags bindingAttr);

		public virtual EventInfo[] GetEvents()
		{
			return GetEvents(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		public abstract EventInfo[] GetEvents(BindingFlags bindingAttr);

		public FieldInfo GetField(string name)
		{
			return GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		public abstract FieldInfo GetField(string name, BindingFlags bindingAttr);

		public FieldInfo[] GetFields()
		{
			return GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		public abstract FieldInfo[] GetFields(BindingFlags bindingAttr);

		public override int GetHashCode()
		{
			Type underlyingSystemType = UnderlyingSystemType;
			if (underlyingSystemType != null && underlyingSystemType != this)
			{
				return underlyingSystemType.GetHashCode();
			}
			return (int)_impl.Value;
		}

		public MemberInfo[] GetMember(string name)
		{
			return GetMember(name, MemberTypes.All, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		public virtual MemberInfo[] GetMember(string name, BindingFlags bindingAttr)
		{
			return GetMember(name, MemberTypes.All, bindingAttr);
		}

		public virtual MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if ((bindingAttr & BindingFlags.IgnoreCase) != BindingFlags.Default)
			{
				return FindMembers(type, bindingAttr, FilterNameIgnoreCase, name);
			}
			return FindMembers(type, bindingAttr, FilterName, name);
		}

		public MemberInfo[] GetMembers()
		{
			return GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		public abstract MemberInfo[] GetMembers(BindingFlags bindingAttr);

		public MethodInfo GetMethod(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return GetMethodImpl(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Any, null, null);
		}

		public MethodInfo GetMethod(string name, BindingFlags bindingAttr)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return GetMethodImpl(name, bindingAttr, null, CallingConventions.Any, null, null);
		}

		public MethodInfo GetMethod(string name, Type[] types)
		{
			return GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Any, types, null);
		}

		public MethodInfo GetMethod(string name, Type[] types, ParameterModifier[] modifiers)
		{
			return GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Any, types, modifiers);
		}

		public MethodInfo GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
		{
			return GetMethod(name, bindingAttr, binder, CallingConventions.Any, types, modifiers);
		}

		public MethodInfo GetMethod(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (types == null)
			{
				throw new ArgumentNullException("types");
			}
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i] == null)
				{
					throw new ArgumentNullException("types");
				}
			}
			return GetMethodImpl(name, bindingAttr, binder, callConvention, types, modifiers);
		}

		protected abstract MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers);

		internal MethodInfo GetMethodImplInternal(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			return GetMethodImpl(name, bindingAttr, binder, callConvention, types, modifiers);
		}

		internal virtual MethodInfo GetMethod(MethodInfo fromNoninstanciated)
		{
			throw new InvalidOperationException("can only be called in generic type");
		}

		internal virtual ConstructorInfo GetConstructor(ConstructorInfo fromNoninstanciated)
		{
			throw new InvalidOperationException("can only be called in generic type");
		}

		internal virtual FieldInfo GetField(FieldInfo fromNoninstanciated)
		{
			throw new InvalidOperationException("can only be called in generic type");
		}

		public MethodInfo[] GetMethods()
		{
			return GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		public abstract MethodInfo[] GetMethods(BindingFlags bindingAttr);

		public Type GetNestedType(string name)
		{
			return GetNestedType(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		public abstract Type GetNestedType(string name, BindingFlags bindingAttr);

		public Type[] GetNestedTypes()
		{
			return GetNestedTypes(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		public abstract Type[] GetNestedTypes(BindingFlags bindingAttr);

		public PropertyInfo[] GetProperties()
		{
			return GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		public abstract PropertyInfo[] GetProperties(BindingFlags bindingAttr);

		public PropertyInfo GetProperty(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return GetPropertyImpl(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, null, null, null);
		}

		public PropertyInfo GetProperty(string name, BindingFlags bindingAttr)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return GetPropertyImpl(name, bindingAttr, null, null, null, null);
		}

		public PropertyInfo GetProperty(string name, Type returnType)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return GetPropertyImpl(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, returnType, null, null);
		}

		public PropertyInfo GetProperty(string name, Type[] types)
		{
			return GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, null, types, null);
		}

		public PropertyInfo GetProperty(string name, Type returnType, Type[] types)
		{
			return GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, returnType, types, null);
		}

		public PropertyInfo GetProperty(string name, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			return GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, returnType, types, modifiers);
		}

		public PropertyInfo GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (types == null)
			{
				throw new ArgumentNullException("types");
			}
			foreach (Type type in types)
			{
				if (type == null)
				{
					throw new ArgumentNullException("types");
				}
			}
			return GetPropertyImpl(name, bindingAttr, binder, returnType, types, modifiers);
		}

		protected abstract PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers);

		internal PropertyInfo GetPropertyImplInternal(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			return GetPropertyImpl(name, bindingAttr, binder, returnType, types, modifiers);
		}

		protected abstract ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers);

		protected abstract TypeAttributes GetAttributeFlagsImpl();

		protected abstract bool HasElementTypeImpl();

		protected abstract bool IsArrayImpl();

		protected abstract bool IsByRefImpl();

		protected abstract bool IsCOMObjectImpl();

		protected abstract bool IsPointerImpl();

		protected abstract bool IsPrimitiveImpl();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsArrayImpl(Type type);

		protected virtual bool IsValueTypeImpl()
		{
			if (this == typeof(ValueType) || this == typeof(Enum))
			{
				return false;
			}
			return IsSubclassOf(typeof(ValueType));
		}

		protected virtual bool IsContextfulImpl()
		{
			return typeof(ContextBoundObject).IsAssignableFrom(this);
		}

		protected virtual bool IsMarshalByRefImpl()
		{
			return typeof(MarshalByRefObject).IsAssignableFrom(this);
		}

		[ComVisible(true)]
		public ConstructorInfo GetConstructor(Type[] types)
		{
			return GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.Any, types, null);
		}

		[ComVisible(true)]
		public ConstructorInfo GetConstructor(BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
		{
			return GetConstructor(bindingAttr, binder, CallingConventions.Any, types, modifiers);
		}

		[ComVisible(true)]
		public ConstructorInfo GetConstructor(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			if (types == null)
			{
				throw new ArgumentNullException("types");
			}
			foreach (Type type in types)
			{
				if (type == null)
				{
					throw new ArgumentNullException("types");
				}
			}
			return GetConstructorImpl(bindingAttr, binder, callConvention, types, modifiers);
		}

		[ComVisible(true)]
		public ConstructorInfo[] GetConstructors()
		{
			return GetConstructors(BindingFlags.Instance | BindingFlags.Public);
		}

		[ComVisible(true)]
		public abstract ConstructorInfo[] GetConstructors(BindingFlags bindingAttr);

		public virtual MemberInfo[] GetDefaultMembers()
		{
			object[] customAttributes = GetCustomAttributes(typeof(DefaultMemberAttribute), true);
			if (customAttributes.Length == 0)
			{
				return new MemberInfo[0];
			}
			MemberInfo[] member = GetMember(((DefaultMemberAttribute)customAttributes[0]).MemberName);
			return (member == null) ? new MemberInfo[0] : member;
		}

		public virtual MemberInfo[] FindMembers(MemberTypes memberType, BindingFlags bindingAttr, MemberFilter filter, object filterCriteria)
		{
			ArrayList arrayList = new ArrayList();
			if ((memberType & MemberTypes.Method) != 0)
			{
				MethodInfo[] methods = GetMethods(bindingAttr);
				if (filter != null)
				{
					MethodInfo[] array = methods;
					foreach (MemberInfo memberInfo in array)
					{
						if (filter(memberInfo, filterCriteria))
						{
							arrayList.Add(memberInfo);
						}
					}
				}
				else
				{
					arrayList.AddRange(methods);
				}
			}
			if ((memberType & MemberTypes.Constructor) != 0)
			{
				ConstructorInfo[] constructors = GetConstructors(bindingAttr);
				if (filter != null)
				{
					ConstructorInfo[] array2 = constructors;
					foreach (MemberInfo memberInfo2 in array2)
					{
						if (filter(memberInfo2, filterCriteria))
						{
							arrayList.Add(memberInfo2);
						}
					}
				}
				else
				{
					arrayList.AddRange(constructors);
				}
			}
			if ((memberType & MemberTypes.Property) != 0)
			{
				int count = arrayList.Count;
				if (filter != null)
				{
					Type type = this;
					while (arrayList.Count == count && type != null)
					{
						PropertyInfo[] properties = type.GetProperties(bindingAttr);
						PropertyInfo[] array3 = properties;
						foreach (MemberInfo memberInfo3 in array3)
						{
							if (filter(memberInfo3, filterCriteria))
							{
								arrayList.Add(memberInfo3);
							}
						}
						type = type.BaseType;
					}
				}
				else
				{
					PropertyInfo[] properties = GetProperties(bindingAttr);
					arrayList.AddRange(properties);
				}
			}
			if ((memberType & MemberTypes.Event) != 0)
			{
				EventInfo[] events = GetEvents(bindingAttr);
				if (filter != null)
				{
					EventInfo[] array4 = events;
					foreach (MemberInfo memberInfo4 in array4)
					{
						if (filter(memberInfo4, filterCriteria))
						{
							arrayList.Add(memberInfo4);
						}
					}
				}
				else
				{
					arrayList.AddRange(events);
				}
			}
			if ((memberType & MemberTypes.Field) != 0)
			{
				FieldInfo[] fields = GetFields(bindingAttr);
				if (filter != null)
				{
					FieldInfo[] array5 = fields;
					foreach (MemberInfo memberInfo5 in array5)
					{
						if (filter(memberInfo5, filterCriteria))
						{
							arrayList.Add(memberInfo5);
						}
					}
				}
				else
				{
					arrayList.AddRange(fields);
				}
			}
			if ((memberType & MemberTypes.NestedType) != 0)
			{
				Type[] nestedTypes = GetNestedTypes(bindingAttr);
				if (filter != null)
				{
					Type[] array6 = nestedTypes;
					foreach (MemberInfo memberInfo6 in array6)
					{
						if (filter(memberInfo6, filterCriteria))
						{
							arrayList.Add(memberInfo6);
						}
					}
				}
				else
				{
					arrayList.AddRange(nestedTypes);
				}
			}
			MemberInfo[] array7;
			switch (memberType)
			{
			case MemberTypes.Constructor:
				array7 = new ConstructorInfo[arrayList.Count];
				break;
			case MemberTypes.Event:
				array7 = new EventInfo[arrayList.Count];
				break;
			case MemberTypes.Field:
				array7 = new FieldInfo[arrayList.Count];
				break;
			case MemberTypes.Method:
				array7 = new MethodInfo[arrayList.Count];
				break;
			case MemberTypes.TypeInfo:
			case MemberTypes.NestedType:
				array7 = new Type[arrayList.Count];
				break;
			case MemberTypes.Property:
				array7 = new PropertyInfo[arrayList.Count];
				break;
			default:
				array7 = new MemberInfo[arrayList.Count];
				break;
			}
			arrayList.CopyTo(array7);
			return array7;
		}

		[DebuggerHidden]
		[DebuggerStepThrough]
		public object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args)
		{
			return InvokeMember(name, invokeAttr, binder, target, args, null, null, null);
		}

		[DebuggerHidden]
		[DebuggerStepThrough]
		public object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, CultureInfo culture)
		{
			return InvokeMember(name, invokeAttr, binder, target, args, null, culture, null);
		}

		public abstract object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters);

		public override string ToString()
		{
			return FullName;
		}

		public virtual Type[] GetGenericArguments()
		{
			throw new NotSupportedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Type GetGenericTypeDefinition_impl();

		public virtual Type GetGenericTypeDefinition()
		{
			throw new NotSupportedException("Derived classes must provide an implementation.");
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Type MakeGenericType(Type gt, Type[] types);

		public virtual Type MakeGenericType(params Type[] typeArguments)
		{
			if (!IsGenericTypeDefinition)
			{
				throw new InvalidOperationException("not a generic type definition");
			}
			if (typeArguments == null)
			{
				throw new ArgumentNullException("typeArguments");
			}
			if (GetGenericArguments().Length != typeArguments.Length)
			{
				throw new ArgumentException(string.Format("The type or method has {0} generic parameter(s) but {1} generic argument(s) where provided. A generic argument must be provided for each generic parameter.", GetGenericArguments().Length, typeArguments.Length), "typeArguments");
			}
			Type[] array = new Type[typeArguments.Length];
			for (int i = 0; i < typeArguments.Length; i++)
			{
				Type type = typeArguments[i];
				if (type == null)
				{
					throw new ArgumentNullException("typeArguments");
				}
				if (!(type is EnumBuilder) && !(type is TypeBuilder))
				{
					type = type.UnderlyingSystemType;
				}
				if (type == null || !type.IsSystemType)
				{
					throw new ArgumentNullException("typeArguments");
				}
				array[i] = type;
			}
			Type type2 = MakeGenericType(this, array);
			if (type2 == null)
			{
				throw new TypeLoadException();
			}
			return type2;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetGenericParameterPosition();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern GenericParameterAttributes GetGenericParameterAttributes();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Type[] GetGenericParameterConstraints_impl();

		public virtual Type[] GetGenericParameterConstraints()
		{
			if (!IsGenericParameter)
			{
				throw new InvalidOperationException();
			}
			return GetGenericParameterConstraints_impl();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Type make_array_type(int rank);

		public virtual Type MakeArrayType()
		{
			return make_array_type(0);
		}

		public virtual Type MakeArrayType(int rank)
		{
			if (rank < 1)
			{
				throw new IndexOutOfRangeException();
			}
			return make_array_type(rank);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Type make_byref_type();

		public virtual Type MakeByRefType()
		{
			return make_byref_type();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern Type MakePointerType();

		public static Type ReflectionOnlyGetType(string typeName, bool throwIfNotFound, bool ignoreCase)
		{
			if (typeName == null)
			{
				throw new ArgumentNullException("typeName");
			}
			int num = typeName.IndexOf(',');
			if (num < 0 || num == 0 || num == typeName.Length - 1)
			{
				throw new ArgumentException("Assembly qualifed type name is required", "typeName");
			}
			string assemblyString = typeName.Substring(num + 1);
			Assembly assembly;
			try
			{
				assembly = Assembly.ReflectionOnlyLoad(assemblyString);
			}
			catch
			{
				if (throwIfNotFound)
				{
					throw;
				}
				return null;
			}
			return assembly.GetType(typeName.Substring(0, num), throwIfNotFound, ignoreCase);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetPacking(out int packing, out int size);

		internal object[] GetPseudoCustomAttributes()
		{
			int num = 0;
			if ((Attributes & TypeAttributes.Serializable) != TypeAttributes.NotPublic)
			{
				num++;
			}
			if ((Attributes & TypeAttributes.Import) != TypeAttributes.NotPublic)
			{
				num++;
			}
			if (num == 0)
			{
				return null;
			}
			object[] array = new object[num];
			num = 0;
			if ((Attributes & TypeAttributes.Serializable) != TypeAttributes.NotPublic)
			{
				array[num++] = new SerializableAttribute();
			}
			if ((Attributes & TypeAttributes.Import) != TypeAttributes.NotPublic)
			{
				array[num++] = new ComImportAttribute();
			}
			return array;
		}
	}
}
