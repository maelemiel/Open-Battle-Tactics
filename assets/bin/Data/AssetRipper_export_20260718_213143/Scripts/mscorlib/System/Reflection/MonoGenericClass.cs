using System.Collections;
using System.Globalization;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Reflection
{
	internal class MonoGenericClass : MonoType
	{
		private const BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		internal TypeBuilder generic_type;

		private Type[] type_arguments;

		private bool initialized;

		private Hashtable fields;

		private Hashtable ctors;

		private Hashtable methods;

		private int event_count;

		public override Type BaseType
		{
			get
			{
				Type parentType = GetParentType();
				return (parentType == null) ? generic_type.BaseType : parentType;
			}
		}

		public override Type UnderlyingSystemType
		{
			get
			{
				return this;
			}
		}

		public override string Name
		{
			get
			{
				return generic_type.Name;
			}
		}

		public override string Namespace
		{
			get
			{
				return generic_type.Namespace;
			}
		}

		public override string FullName
		{
			get
			{
				return format_name(true, false);
			}
		}

		public override string AssemblyQualifiedName
		{
			get
			{
				return format_name(true, true);
			}
		}

		public override Guid GUID
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		internal MonoGenericClass()
			: base(null)
		{
			throw new InvalidOperationException();
		}

		internal MonoGenericClass(TypeBuilder tb, Type[] args)
			: base(null)
		{
			generic_type = tb;
			type_arguments = args;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void initialize(MethodInfo[] methods, ConstructorInfo[] ctors, FieldInfo[] fields, PropertyInfo[] properties, EventInfo[] events);

		private void initialize()
		{
			if (!initialized)
			{
				MonoGenericClass monoGenericClass = GetParentType() as MonoGenericClass;
				if (monoGenericClass != null)
				{
					monoGenericClass.initialize();
				}
				EventInfo[] events_internal = generic_type.GetEvents_internal(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				event_count = events_internal.Length;
				initialize(generic_type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic), generic_type.GetConstructorsInternal(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic), generic_type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic), generic_type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic), events_internal);
				initialized = true;
			}
		}

		private Type GetParentType()
		{
			return InflateType(generic_type.BaseType);
		}

		internal Type InflateType(Type type)
		{
			return InflateType(type, null);
		}

		internal Type InflateType(Type type, Type[] method_args)
		{
			if (type == null)
			{
				return null;
			}
			if (!type.IsGenericParameter && !type.ContainsGenericParameters)
			{
				return type;
			}
			if (type.IsGenericParameter)
			{
				if (type.DeclaringMethod == null)
				{
					return type_arguments[type.GenericParameterPosition];
				}
				if (method_args != null)
				{
					return method_args[type.GenericParameterPosition];
				}
				return type;
			}
			if (type.IsPointer)
			{
				return InflateType(type.GetElementType(), method_args).MakePointerType();
			}
			if (type.IsByRef)
			{
				return InflateType(type.GetElementType(), method_args).MakeByRefType();
			}
			if (type.IsArray)
			{
				if (type.GetArrayRank() > 1)
				{
					return InflateType(type.GetElementType(), method_args).MakeArrayType(type.GetArrayRank());
				}
				if (type.ToString().EndsWith("[*]", StringComparison.Ordinal))
				{
					return InflateType(type.GetElementType(), method_args).MakeArrayType(1);
				}
				return InflateType(type.GetElementType(), method_args).MakeArrayType();
			}
			Type[] genericArguments = type.GetGenericArguments();
			for (int i = 0; i < genericArguments.Length; i++)
			{
				genericArguments[i] = InflateType(genericArguments[i], method_args);
			}
			Type type2 = ((!type.IsGenericTypeDefinition) ? type.GetGenericTypeDefinition() : type);
			return type2.MakeGenericType(genericArguments);
		}

		private Type[] GetInterfacesInternal()
		{
			if (generic_type.interfaces == null)
			{
				return new Type[0];
			}
			Type[] array = new Type[generic_type.interfaces.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = InflateType(generic_type.interfaces[i]);
			}
			return array;
		}

		public override Type[] GetInterfaces()
		{
			if (!generic_type.IsCompilerContext)
			{
				throw new NotSupportedException();
			}
			return GetInterfacesInternal();
		}

		protected override bool IsValueTypeImpl()
		{
			return generic_type.IsValueType;
		}

		internal override MethodInfo GetMethod(MethodInfo fromNoninstanciated)
		{
			initialize();
			if (!(fromNoninstanciated is MethodBuilder))
			{
				throw new InvalidOperationException("Inflating non MethodBuilder objects is not supported: " + fromNoninstanciated.GetType());
			}
			MethodBuilder methodBuilder = (MethodBuilder)fromNoninstanciated;
			if (methods == null)
			{
				methods = new Hashtable();
			}
			if (!methods.ContainsKey(methodBuilder))
			{
				methods[methodBuilder] = new MethodOnTypeBuilderInst(this, methodBuilder);
			}
			return (MethodInfo)methods[methodBuilder];
		}

		internal override ConstructorInfo GetConstructor(ConstructorInfo fromNoninstanciated)
		{
			initialize();
			if (!(fromNoninstanciated is ConstructorBuilder))
			{
				throw new InvalidOperationException("Inflating non ConstructorBuilder objects is not supported: " + fromNoninstanciated.GetType());
			}
			ConstructorBuilder constructorBuilder = (ConstructorBuilder)fromNoninstanciated;
			if (ctors == null)
			{
				ctors = new Hashtable();
			}
			if (!ctors.ContainsKey(constructorBuilder))
			{
				ctors[constructorBuilder] = new ConstructorOnTypeBuilderInst(this, constructorBuilder);
			}
			return (ConstructorInfo)ctors[constructorBuilder];
		}

		internal override FieldInfo GetField(FieldInfo fromNoninstanciated)
		{
			initialize();
			if (!(fromNoninstanciated is FieldBuilder))
			{
				throw new InvalidOperationException("Inflating non FieldBuilder objects is not supported: " + fromNoninstanciated.GetType());
			}
			FieldBuilder fieldBuilder = (FieldBuilder)fromNoninstanciated;
			if (fields == null)
			{
				fields = new Hashtable();
			}
			if (!fields.ContainsKey(fieldBuilder))
			{
				fields[fieldBuilder] = new FieldOnTypeBuilderInst(this, fieldBuilder);
			}
			return (FieldInfo)fields[fieldBuilder];
		}

		public override MethodInfo[] GetMethods(BindingFlags bf)
		{
			if (!generic_type.IsCompilerContext)
			{
				throw new NotSupportedException();
			}
			ArrayList arrayList = new ArrayList();
			Type type = this;
			do
			{
				MonoGenericClass monoGenericClass = type as MonoGenericClass;
				if (monoGenericClass != null)
				{
					arrayList.AddRange(monoGenericClass.GetMethodsInternal(bf, this));
				}
				else
				{
					if (!(type is TypeBuilder))
					{
						MonoType monoType = (MonoType)type;
						arrayList.AddRange(monoType.GetMethodsByName(null, bf, false, this));
						break;
					}
					arrayList.AddRange(type.GetMethods(bf));
				}
				if ((bf & BindingFlags.DeclaredOnly) != BindingFlags.Default)
				{
					break;
				}
				type = type.BaseType;
			}
			while (type != null);
			MethodInfo[] array = new MethodInfo[arrayList.Count];
			arrayList.CopyTo(array);
			return array;
		}

		private MethodInfo[] GetMethodsInternal(BindingFlags bf, MonoGenericClass reftype)
		{
			if (generic_type.num_methods == 0)
			{
				return new MethodInfo[0];
			}
			ArrayList arrayList = new ArrayList();
			initialize();
			for (int i = 0; i < generic_type.num_methods; i++)
			{
				MethodInfo methodInfo = generic_type.methods[i];
				bool flag = false;
				MethodAttributes attributes = methodInfo.Attributes;
				if ((attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public)
				{
					if ((bf & BindingFlags.Public) != BindingFlags.Default)
					{
						flag = true;
					}
				}
				else if ((bf & BindingFlags.NonPublic) != BindingFlags.Default)
				{
					flag = true;
				}
				if (!flag)
				{
					continue;
				}
				flag = false;
				if ((attributes & MethodAttributes.Static) != MethodAttributes.PrivateScope)
				{
					if ((bf & BindingFlags.Static) != BindingFlags.Default)
					{
						flag = true;
					}
				}
				else if ((bf & BindingFlags.Instance) != BindingFlags.Default)
				{
					flag = true;
				}
				if (flag)
				{
					methodInfo = TypeBuilder.GetMethod(this, methodInfo);
					arrayList.Add(methodInfo);
				}
			}
			MethodInfo[] array = new MethodInfo[arrayList.Count];
			arrayList.CopyTo(array);
			return array;
		}

		public override ConstructorInfo[] GetConstructors(BindingFlags bf)
		{
			if (!generic_type.IsCompilerContext)
			{
				throw new NotSupportedException();
			}
			ArrayList arrayList = new ArrayList();
			Type type = this;
			do
			{
				MonoGenericClass monoGenericClass = type as MonoGenericClass;
				if (monoGenericClass != null)
				{
					arrayList.AddRange(monoGenericClass.GetConstructorsInternal(bf, this));
				}
				else
				{
					if (!(type is TypeBuilder))
					{
						MonoType monoType = (MonoType)type;
						arrayList.AddRange(monoType.GetConstructors_internal(bf, this));
						break;
					}
					arrayList.AddRange(type.GetConstructors(bf));
				}
				if ((bf & BindingFlags.DeclaredOnly) != BindingFlags.Default)
				{
					break;
				}
				type = type.BaseType;
			}
			while (type != null);
			ConstructorInfo[] array = new ConstructorInfo[arrayList.Count];
			arrayList.CopyTo(array);
			return array;
		}

		private ConstructorInfo[] GetConstructorsInternal(BindingFlags bf, MonoGenericClass reftype)
		{
			if (generic_type.ctors == null)
			{
				return new ConstructorInfo[0];
			}
			ArrayList arrayList = new ArrayList();
			initialize();
			for (int i = 0; i < generic_type.ctors.Length; i++)
			{
				ConstructorInfo constructorInfo = generic_type.ctors[i];
				bool flag = false;
				MethodAttributes attributes = constructorInfo.Attributes;
				if ((attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public)
				{
					if ((bf & BindingFlags.Public) != BindingFlags.Default)
					{
						flag = true;
					}
				}
				else if ((bf & BindingFlags.NonPublic) != BindingFlags.Default)
				{
					flag = true;
				}
				if (!flag)
				{
					continue;
				}
				flag = false;
				if ((attributes & MethodAttributes.Static) != MethodAttributes.PrivateScope)
				{
					if ((bf & BindingFlags.Static) != BindingFlags.Default)
					{
						flag = true;
					}
				}
				else if ((bf & BindingFlags.Instance) != BindingFlags.Default)
				{
					flag = true;
				}
				if (flag)
				{
					arrayList.Add(TypeBuilder.GetConstructor(this, constructorInfo));
				}
			}
			ConstructorInfo[] array = new ConstructorInfo[arrayList.Count];
			arrayList.CopyTo(array);
			return array;
		}

		public override FieldInfo[] GetFields(BindingFlags bf)
		{
			if (!generic_type.IsCompilerContext)
			{
				throw new NotSupportedException();
			}
			ArrayList arrayList = new ArrayList();
			Type type = this;
			do
			{
				MonoGenericClass monoGenericClass = type as MonoGenericClass;
				if (monoGenericClass != null)
				{
					arrayList.AddRange(monoGenericClass.GetFieldsInternal(bf, this));
				}
				else
				{
					if (!(type is TypeBuilder))
					{
						MonoType monoType = (MonoType)type;
						arrayList.AddRange(monoType.GetFields_internal(bf, this));
						break;
					}
					arrayList.AddRange(type.GetFields(bf));
				}
				if ((bf & BindingFlags.DeclaredOnly) != BindingFlags.Default)
				{
					break;
				}
				type = type.BaseType;
			}
			while (type != null);
			FieldInfo[] array = new FieldInfo[arrayList.Count];
			arrayList.CopyTo(array);
			return array;
		}

		private FieldInfo[] GetFieldsInternal(BindingFlags bf, MonoGenericClass reftype)
		{
			if (generic_type.num_fields == 0)
			{
				return new FieldInfo[0];
			}
			ArrayList arrayList = new ArrayList();
			initialize();
			for (int i = 0; i < generic_type.num_fields; i++)
			{
				FieldInfo fieldInfo = generic_type.fields[i];
				bool flag = false;
				FieldAttributes attributes = fieldInfo.Attributes;
				if ((attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Public)
				{
					if ((bf & BindingFlags.Public) != BindingFlags.Default)
					{
						flag = true;
					}
				}
				else if ((bf & BindingFlags.NonPublic) != BindingFlags.Default)
				{
					flag = true;
				}
				if (!flag)
				{
					continue;
				}
				flag = false;
				if ((attributes & FieldAttributes.Static) != FieldAttributes.PrivateScope)
				{
					if ((bf & BindingFlags.Static) != BindingFlags.Default)
					{
						flag = true;
					}
				}
				else if ((bf & BindingFlags.Instance) != BindingFlags.Default)
				{
					flag = true;
				}
				if (flag)
				{
					arrayList.Add(TypeBuilder.GetField(this, fieldInfo));
				}
			}
			FieldInfo[] array = new FieldInfo[arrayList.Count];
			arrayList.CopyTo(array);
			return array;
		}

		public override PropertyInfo[] GetProperties(BindingFlags bf)
		{
			if (!generic_type.IsCompilerContext)
			{
				throw new NotSupportedException();
			}
			ArrayList arrayList = new ArrayList();
			Type type = this;
			do
			{
				MonoGenericClass monoGenericClass = type as MonoGenericClass;
				if (monoGenericClass != null)
				{
					arrayList.AddRange(monoGenericClass.GetPropertiesInternal(bf, this));
				}
				else
				{
					if (!(type is TypeBuilder))
					{
						MonoType monoType = (MonoType)type;
						arrayList.AddRange(monoType.GetPropertiesByName(null, bf, false, this));
						break;
					}
					arrayList.AddRange(type.GetProperties(bf));
				}
				if ((bf & BindingFlags.DeclaredOnly) != BindingFlags.Default)
				{
					break;
				}
				type = type.BaseType;
			}
			while (type != null);
			PropertyInfo[] array = new PropertyInfo[arrayList.Count];
			arrayList.CopyTo(array);
			return array;
		}

		private PropertyInfo[] GetPropertiesInternal(BindingFlags bf, MonoGenericClass reftype)
		{
			if (generic_type.properties == null)
			{
				return new PropertyInfo[0];
			}
			ArrayList arrayList = new ArrayList();
			initialize();
			PropertyBuilder[] properties = generic_type.properties;
			foreach (PropertyInfo propertyInfo in properties)
			{
				bool flag = false;
				MethodInfo methodInfo = propertyInfo.GetGetMethod(true);
				if (methodInfo == null)
				{
					methodInfo = propertyInfo.GetSetMethod(true);
				}
				if (methodInfo == null)
				{
					continue;
				}
				MethodAttributes attributes = methodInfo.Attributes;
				if ((attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public)
				{
					if ((bf & BindingFlags.Public) != BindingFlags.Default)
					{
						flag = true;
					}
				}
				else if ((bf & BindingFlags.NonPublic) != BindingFlags.Default)
				{
					flag = true;
				}
				if (!flag)
				{
					continue;
				}
				flag = false;
				if ((attributes & MethodAttributes.Static) != MethodAttributes.PrivateScope)
				{
					if ((bf & BindingFlags.Static) != BindingFlags.Default)
					{
						flag = true;
					}
				}
				else if ((bf & BindingFlags.Instance) != BindingFlags.Default)
				{
					flag = true;
				}
				if (flag)
				{
					arrayList.Add(new PropertyOnTypeBuilderInst(reftype, propertyInfo));
				}
			}
			PropertyInfo[] array = new PropertyInfo[arrayList.Count];
			arrayList.CopyTo(array);
			return array;
		}

		public override EventInfo[] GetEvents(BindingFlags bf)
		{
			if (!generic_type.IsCompilerContext)
			{
				throw new NotSupportedException();
			}
			ArrayList arrayList = new ArrayList();
			Type type = this;
			do
			{
				MonoGenericClass monoGenericClass = type as MonoGenericClass;
				if (monoGenericClass != null)
				{
					arrayList.AddRange(monoGenericClass.GetEventsInternal(bf, this));
				}
				else
				{
					if (!(type is TypeBuilder))
					{
						MonoType monoType = (MonoType)type;
						arrayList.AddRange(monoType.GetEvents(bf));
						break;
					}
					arrayList.AddRange(type.GetEvents(bf));
				}
				if ((bf & BindingFlags.DeclaredOnly) != BindingFlags.Default)
				{
					break;
				}
				type = type.BaseType;
			}
			while (type != null);
			EventInfo[] array = new EventInfo[arrayList.Count];
			arrayList.CopyTo(array);
			return array;
		}

		private EventInfo[] GetEventsInternal(BindingFlags bf, MonoGenericClass reftype)
		{
			if (generic_type.events == null)
			{
				return new EventInfo[0];
			}
			initialize();
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < event_count; i++)
			{
				EventBuilder eventBuilder = generic_type.events[i];
				bool flag = false;
				MethodInfo methodInfo = eventBuilder.add_method;
				if (methodInfo == null)
				{
					methodInfo = eventBuilder.remove_method;
				}
				if (methodInfo == null)
				{
					continue;
				}
				MethodAttributes attributes = methodInfo.Attributes;
				if ((attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public)
				{
					if ((bf & BindingFlags.Public) != BindingFlags.Default)
					{
						flag = true;
					}
				}
				else if ((bf & BindingFlags.NonPublic) != BindingFlags.Default)
				{
					flag = true;
				}
				if (!flag)
				{
					continue;
				}
				flag = false;
				if ((attributes & MethodAttributes.Static) != MethodAttributes.PrivateScope)
				{
					if ((bf & BindingFlags.Static) != BindingFlags.Default)
					{
						flag = true;
					}
				}
				else if ((bf & BindingFlags.Instance) != BindingFlags.Default)
				{
					flag = true;
				}
				if (flag)
				{
					arrayList.Add(new EventOnTypeBuilderInst(this, eventBuilder));
				}
			}
			EventInfo[] array = new EventInfo[arrayList.Count];
			arrayList.CopyTo(array);
			return array;
		}

		public override Type[] GetNestedTypes(BindingFlags bf)
		{
			return generic_type.GetNestedTypes(bf);
		}

		public override bool IsAssignableFrom(Type c)
		{
			if (c == this)
			{
				return true;
			}
			Type[] interfacesInternal = GetInterfacesInternal();
			if (c.IsInterface)
			{
				if (interfacesInternal == null)
				{
					return false;
				}
				Type[] array = interfacesInternal;
				foreach (Type c2 in array)
				{
					if (c.IsAssignableFrom(c2))
					{
						return true;
					}
				}
				return false;
			}
			Type parentType = GetParentType();
			if (parentType == null)
			{
				return c == typeof(object);
			}
			return c.IsAssignableFrom(parentType);
		}

		private string format_name(bool full_name, bool assembly_qualified)
		{
			StringBuilder stringBuilder = new StringBuilder(generic_type.FullName);
			bool isCompilerContext = generic_type.IsCompilerContext;
			stringBuilder.Append("[");
			for (int i = 0; i < type_arguments.Length; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(",");
				}
				string text = ((!full_name) ? type_arguments[i].ToString() : type_arguments[i].AssemblyQualifiedName);
				if (text == null)
				{
					if (!isCompilerContext || !type_arguments[i].IsGenericParameter)
					{
						return null;
					}
					text = type_arguments[i].Name;
				}
				if (full_name)
				{
					stringBuilder.Append("[");
				}
				stringBuilder.Append(text);
				if (full_name)
				{
					stringBuilder.Append("]");
				}
			}
			stringBuilder.Append("]");
			if (assembly_qualified)
			{
				stringBuilder.Append(", ");
				stringBuilder.Append(generic_type.Assembly.FullName);
			}
			return stringBuilder.ToString();
		}

		public override string ToString()
		{
			return format_name(false, false);
		}

		public override Type MakeArrayType()
		{
			return new ArrayType(this, 0);
		}

		public override Type MakeArrayType(int rank)
		{
			if (rank < 1)
			{
				throw new IndexOutOfRangeException();
			}
			return new ArrayType(this, rank);
		}

		public override Type MakeByRefType()
		{
			return new ByRefType(this);
		}

		public override Type MakePointerType()
		{
			return new PointerType(this);
		}

		protected override bool IsCOMObjectImpl()
		{
			return false;
		}

		protected override bool IsPrimitiveImpl()
		{
			return false;
		}

		protected override TypeAttributes GetAttributeFlagsImpl()
		{
			return generic_type.Attributes;
		}

		public override Type GetInterface(string name, bool ignoreCase)
		{
			throw new NotSupportedException();
		}

		public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
		{
			if (!generic_type.IsCompilerContext)
			{
				throw new NotSupportedException();
			}
			EventInfo[] events = GetEvents(bindingAttr);
			foreach (EventInfo eventInfo in events)
			{
				if (eventInfo.Name == name)
				{
					return eventInfo;
				}
			}
			return null;
		}

		public override FieldInfo GetField(string name, BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}

		public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}

		public override Type GetNestedType(string name, BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}

		public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			throw new NotSupportedException();
		}

		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			throw new NotSupportedException();
		}

		protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			throw new NotSupportedException();
		}

		protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
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
