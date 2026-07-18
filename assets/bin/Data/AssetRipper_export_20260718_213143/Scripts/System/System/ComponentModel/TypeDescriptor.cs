using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.ComponentModel
{
	public sealed class TypeDescriptor
	{
		private sealed class AttributeProvider : TypeDescriptionProvider
		{
			private sealed class AttributeTypeDescriptor : CustomTypeDescriptor
			{
				private Attribute[] attributes;

				public AttributeTypeDescriptor(ICustomTypeDescriptor parent, Attribute[] attributes)
					: base(parent)
				{
					this.attributes = attributes;
				}

				public override AttributeCollection GetAttributes()
				{
					AttributeCollection attributeCollection = base.GetAttributes();
					if (attributeCollection != null && attributeCollection.Count > 0)
					{
						return AttributeCollection.FromExisting(attributeCollection, attributes);
					}
					return new AttributeCollection(attributes);
				}
			}

			private Attribute[] attributes;

			public AttributeProvider(Attribute[] attributes, TypeDescriptionProvider parent)
				: base(parent)
			{
				this.attributes = attributes;
			}

			public override ICustomTypeDescriptor GetTypeDescriptor(Type type, object instance)
			{
				return new AttributeTypeDescriptor(base.GetTypeDescriptor(type, instance), attributes);
			}
		}

		private sealed class WrappedTypeDescriptionProvider : TypeDescriptionProvider
		{
			public TypeDescriptionProvider Wrapped { get; private set; }

			public WrappedTypeDescriptionProvider(TypeDescriptionProvider wrapped)
			{
				Wrapped = wrapped;
			}

			public override object CreateInstance(IServiceProvider provider, Type objectType, Type[] argTypes, object[] args)
			{
				TypeDescriptionProvider wrapped = Wrapped;
				if (wrapped == null)
				{
					return base.CreateInstance(provider, objectType, argTypes, args);
				}
				return wrapped.CreateInstance(provider, objectType, argTypes, args);
			}

			public override IDictionary GetCache(object instance)
			{
				TypeDescriptionProvider wrapped = Wrapped;
				if (wrapped == null)
				{
					return base.GetCache(instance);
				}
				return wrapped.GetCache(instance);
			}

			public override ICustomTypeDescriptor GetExtendedTypeDescriptor(object instance)
			{
				return new DefaultTypeDescriptor(this, null, instance);
			}

			public override string GetFullComponentName(object component)
			{
				TypeDescriptionProvider wrapped = Wrapped;
				if (wrapped == null)
				{
					return base.GetFullComponentName(component);
				}
				return wrapped.GetFullComponentName(component);
			}

			public override Type GetReflectionType(Type type, object instance)
			{
				TypeDescriptionProvider wrapped = Wrapped;
				if (wrapped == null)
				{
					return base.GetReflectionType(type, instance);
				}
				return wrapped.GetReflectionType(type, instance);
			}

			public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
			{
				TypeDescriptionProvider wrapped = Wrapped;
				if (wrapped == null)
				{
					return new DefaultTypeDescriptor(this, objectType, instance);
				}
				return wrapped.GetTypeDescriptor(objectType, instance);
			}
		}

		private sealed class DefaultTypeDescriptor : CustomTypeDescriptor
		{
			private TypeDescriptionProvider owner;

			private Type objectType;

			private object instance;

			public DefaultTypeDescriptor(TypeDescriptionProvider owner, Type objectType, object instance)
			{
				this.owner = owner;
				this.objectType = objectType;
				this.instance = instance;
			}

			public override AttributeCollection GetAttributes()
			{
				WrappedTypeDescriptionProvider wrappedTypeDescriptionProvider = owner as WrappedTypeDescriptionProvider;
				if (wrappedTypeDescriptionProvider != null)
				{
					return wrappedTypeDescriptionProvider.Wrapped.GetTypeDescriptor(objectType, instance).GetAttributes();
				}
				if (instance != null)
				{
					return TypeDescriptor.GetAttributes(instance, false);
				}
				if (objectType != null)
				{
					return GetTypeInfo(objectType).GetAttributes();
				}
				return base.GetAttributes();
			}

			public override string GetClassName()
			{
				WrappedTypeDescriptionProvider wrappedTypeDescriptionProvider = owner as WrappedTypeDescriptionProvider;
				if (wrappedTypeDescriptionProvider != null)
				{
					return wrappedTypeDescriptionProvider.Wrapped.GetTypeDescriptor(objectType, instance).GetClassName();
				}
				return base.GetClassName();
			}

			public override PropertyDescriptor GetDefaultProperty()
			{
				WrappedTypeDescriptionProvider wrappedTypeDescriptionProvider = owner as WrappedTypeDescriptionProvider;
				if (wrappedTypeDescriptionProvider != null)
				{
					return wrappedTypeDescriptionProvider.Wrapped.GetTypeDescriptor(objectType, instance).GetDefaultProperty();
				}
				if (objectType != null)
				{
					return GetTypeInfo(objectType).GetDefaultProperty();
				}
				if (instance != null)
				{
					return GetTypeInfo(instance.GetType()).GetDefaultProperty();
				}
				return base.GetDefaultProperty();
			}

			public override PropertyDescriptorCollection GetProperties()
			{
				WrappedTypeDescriptionProvider wrappedTypeDescriptionProvider = owner as WrappedTypeDescriptionProvider;
				if (wrappedTypeDescriptionProvider != null)
				{
					return wrappedTypeDescriptionProvider.Wrapped.GetTypeDescriptor(objectType, instance).GetProperties();
				}
				if (instance != null)
				{
					return TypeDescriptor.GetProperties(instance, (Attribute[])null, false);
				}
				if (objectType != null)
				{
					return GetTypeInfo(objectType).GetProperties(null);
				}
				return base.GetProperties();
			}
		}

		private sealed class DefaultTypeDescriptionProvider : TypeDescriptionProvider
		{
			public override ICustomTypeDescriptor GetExtendedTypeDescriptor(object instance)
			{
				return new DefaultTypeDescriptor(this, null, instance);
			}

			public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
			{
				return new DefaultTypeDescriptor(this, objectType, instance);
			}
		}

		private static readonly object creatingDefaultConverters;

		private static ArrayList defaultConverters;

		private static IComNativeDescriptorHandler descriptorHandler;

		private static Hashtable componentTable;

		private static Hashtable typeTable;

		private static Hashtable editors;

		private static object typeDescriptionProvidersLock;

		private static Dictionary<Type, LinkedList<TypeDescriptionProvider>> typeDescriptionProviders;

		private static object componentDescriptionProvidersLock;

		private static Dictionary<System.ComponentModel.WeakObjectWrapper, LinkedList<TypeDescriptionProvider>> componentDescriptionProviders;

		private static EventHandler onDispose;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[System.MonoNotSupported("Mono does not support COM")]
		public static Type ComObjectType
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		private static ArrayList DefaultConverters
		{
			get
			{
				lock (creatingDefaultConverters)
				{
					if (defaultConverters != null)
					{
						return defaultConverters;
					}
					defaultConverters = new ArrayList();
					defaultConverters.Add(new DictionaryEntry(typeof(bool), typeof(BooleanConverter)));
					defaultConverters.Add(new DictionaryEntry(typeof(byte), typeof(ByteConverter)));
					defaultConverters.Add(new DictionaryEntry(typeof(sbyte), typeof(SByteConverter)));
					defaultConverters.Add(new DictionaryEntry(typeof(string), typeof(StringConverter)));
					defaultConverters.Add(new DictionaryEntry(typeof(char), typeof(CharConverter)));
					defaultConverters.Add(new DictionaryEntry(typeof(short), typeof(Int16Converter)));
					defaultConverters.Add(new DictionaryEntry(typeof(int), typeof(Int32Converter)));
					defaultConverters.Add(new DictionaryEntry(typeof(long), typeof(Int64Converter)));
					defaultConverters.Add(new DictionaryEntry(typeof(ushort), typeof(UInt16Converter)));
					defaultConverters.Add(new DictionaryEntry(typeof(uint), typeof(UInt32Converter)));
					defaultConverters.Add(new DictionaryEntry(typeof(ulong), typeof(UInt64Converter)));
					defaultConverters.Add(new DictionaryEntry(typeof(float), typeof(SingleConverter)));
					defaultConverters.Add(new DictionaryEntry(typeof(double), typeof(DoubleConverter)));
					defaultConverters.Add(new DictionaryEntry(typeof(decimal), typeof(DecimalConverter)));
					defaultConverters.Add(new DictionaryEntry(typeof(void), typeof(TypeConverter)));
					defaultConverters.Add(new DictionaryEntry(typeof(Array), typeof(ArrayConverter)));
					defaultConverters.Add(new DictionaryEntry(typeof(CultureInfo), typeof(CultureInfoConverter)));
					defaultConverters.Add(new DictionaryEntry(typeof(DateTime), typeof(DateTimeConverter)));
					defaultConverters.Add(new DictionaryEntry(typeof(Guid), typeof(GuidConverter)));
					defaultConverters.Add(new DictionaryEntry(typeof(TimeSpan), typeof(TimeSpanConverter)));
					defaultConverters.Add(new DictionaryEntry(typeof(ICollection), typeof(CollectionConverter)));
					defaultConverters.Add(new DictionaryEntry(typeof(Enum), typeof(EnumConverter)));
				}
				return defaultConverters;
			}
		}

		[Obsolete("Use ComObjectType")]
		public static IComNativeDescriptorHandler ComNativeDescriptorHandler
		{
			get
			{
				return descriptorHandler;
			}
			set
			{
				descriptorHandler = value;
			}
		}

		public static event RefreshEventHandler Refreshed;

		private TypeDescriptor()
		{
		}

		static TypeDescriptor()
		{
			creatingDefaultConverters = new object();
			componentTable = new Hashtable();
			typeTable = new Hashtable();
			typeDescriptionProvidersLock = new object();
			componentDescriptionProvidersLock = new object();
			typeDescriptionProviders = new Dictionary<Type, LinkedList<TypeDescriptionProvider>>();
			componentDescriptionProviders = new Dictionary<System.ComponentModel.WeakObjectWrapper, LinkedList<TypeDescriptionProvider>>(new System.ComponentModel.WeakObjectWrapperComparer());
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static TypeDescriptionProvider AddAttributes(object instance, params Attribute[] attributes)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (attributes == null)
			{
				throw new ArgumentNullException("attributes");
			}
			AttributeProvider attributeProvider = new AttributeProvider(attributes, GetProvider(instance));
			AddProvider(attributeProvider, instance);
			return attributeProvider;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static TypeDescriptionProvider AddAttributes(Type type, params Attribute[] attributes)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (attributes == null)
			{
				throw new ArgumentNullException("attributes");
			}
			AttributeProvider attributeProvider = new AttributeProvider(attributes, GetProvider(type));
			AddProvider(attributeProvider, type);
			return attributeProvider;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static void AddProvider(TypeDescriptionProvider provider, object instance)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			lock (componentDescriptionProvidersLock)
			{
				System.ComponentModel.WeakObjectWrapper key = new System.ComponentModel.WeakObjectWrapper(instance);
				LinkedList<TypeDescriptionProvider> value;
				if (!componentDescriptionProviders.TryGetValue(key, out value))
				{
					value = new LinkedList<TypeDescriptionProvider>();
					componentDescriptionProviders.Add(new System.ComponentModel.WeakObjectWrapper(instance), value);
				}
				value.AddLast(provider);
				key = null;
				Refresh(instance);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static void AddProvider(TypeDescriptionProvider provider, Type type)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			lock (typeDescriptionProvidersLock)
			{
				LinkedList<TypeDescriptionProvider> value;
				if (!typeDescriptionProviders.TryGetValue(type, out value))
				{
					value = new LinkedList<TypeDescriptionProvider>();
					typeDescriptionProviders.Add(type, value);
				}
				value.AddLast(provider);
				Refresh(type);
			}
		}

		[System.MonoTODO]
		public static object CreateInstance(IServiceProvider provider, Type objectType, Type[] argTypes, object[] args)
		{
			if (objectType == null)
			{
				throw new ArgumentNullException("objectType");
			}
			object obj = null;
			if (provider != null)
			{
				TypeDescriptionProvider typeDescriptionProvider = provider.GetService(typeof(TypeDescriptionProvider)) as TypeDescriptionProvider;
				if (typeDescriptionProvider != null)
				{
					obj = typeDescriptionProvider.CreateInstance(provider, objectType, argTypes, args);
				}
			}
			if (obj == null)
			{
				obj = Activator.CreateInstance(objectType, args);
			}
			return obj;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static void AddEditorTable(Type editorBaseType, Hashtable table)
		{
			if (editorBaseType == null)
			{
				throw new ArgumentNullException("editorBaseType");
			}
			if (editors == null)
			{
				editors = new Hashtable();
			}
			if (!editors.ContainsKey(editorBaseType))
			{
				editors[editorBaseType] = table;
			}
		}

		public static IDesigner CreateDesigner(IComponent component, Type designerBaseType)
		{
			string assemblyQualifiedName = designerBaseType.AssemblyQualifiedName;
			AttributeCollection attributes = GetAttributes(component);
			foreach (Attribute item in attributes)
			{
				DesignerAttribute designerAttribute = item as DesignerAttribute;
				if (designerAttribute != null && assemblyQualifiedName == designerAttribute.DesignerBaseTypeName)
				{
					Type typeFromName = GetTypeFromName(component, designerAttribute.DesignerTypeName);
					if (typeFromName != null)
					{
						return (IDesigner)Activator.CreateInstance(typeFromName);
					}
				}
			}
			return null;
		}

		public static EventDescriptor CreateEvent(Type componentType, string name, Type type, params Attribute[] attributes)
		{
			return new System.ComponentModel.ReflectionEventDescriptor(componentType, name, type, attributes);
		}

		public static EventDescriptor CreateEvent(Type componentType, EventDescriptor oldEventDescriptor, params Attribute[] attributes)
		{
			return new System.ComponentModel.ReflectionEventDescriptor(componentType, oldEventDescriptor, attributes);
		}

		public static PropertyDescriptor CreateProperty(Type componentType, string name, Type type, params Attribute[] attributes)
		{
			return new System.ComponentModel.ReflectionPropertyDescriptor(componentType, name, type, attributes);
		}

		public static PropertyDescriptor CreateProperty(Type componentType, PropertyDescriptor oldPropertyDescriptor, params Attribute[] attributes)
		{
			return new System.ComponentModel.ReflectionPropertyDescriptor(componentType, oldPropertyDescriptor, attributes);
		}

		public static AttributeCollection GetAttributes(Type componentType)
		{
			if (componentType == null)
			{
				return AttributeCollection.Empty;
			}
			return GetTypeInfo(componentType).GetAttributes();
		}

		public static AttributeCollection GetAttributes(object component)
		{
			return GetAttributes(component, false);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static AttributeCollection GetAttributes(object component, bool noCustomTypeDesc)
		{
			if (component == null)
			{
				return AttributeCollection.Empty;
			}
			if (!noCustomTypeDesc && component is ICustomTypeDescriptor)
			{
				return ((ICustomTypeDescriptor)component).GetAttributes();
			}
			IComponent component2 = component as IComponent;
			if (component2 != null && component2.Site != null)
			{
				return GetComponentInfo(component2).GetAttributes();
			}
			return GetTypeInfo(component.GetType()).GetAttributes();
		}

		public static string GetClassName(object component)
		{
			return GetClassName(component, false);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static string GetClassName(object component, bool noCustomTypeDesc)
		{
			if (component == null)
			{
				throw new ArgumentNullException("component", "component cannot be null");
			}
			if (!noCustomTypeDesc && component is ICustomTypeDescriptor)
			{
				string text = ((ICustomTypeDescriptor)component).GetClassName();
				if (text == null)
				{
					text = ((ICustomTypeDescriptor)component).GetComponentName();
				}
				if (text == null)
				{
					text = component.GetType().FullName;
				}
				return text;
			}
			return component.GetType().FullName;
		}

		public static string GetComponentName(object component)
		{
			return GetComponentName(component, false);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static string GetComponentName(object component, bool noCustomTypeDesc)
		{
			if (component == null)
			{
				throw new ArgumentNullException("component", "component cannot be null");
			}
			if (!noCustomTypeDesc && component is ICustomTypeDescriptor)
			{
				return ((ICustomTypeDescriptor)component).GetComponentName();
			}
			IComponent component2 = component as IComponent;
			if (component2 != null && component2.Site != null)
			{
				return component2.Site.Name;
			}
			return null;
		}

		[System.MonoNotSupported("")]
		public static string GetFullComponentName(object component)
		{
			throw new NotImplementedException();
		}

		[System.MonoNotSupported("")]
		public static string GetClassName(Type componentType)
		{
			throw new NotImplementedException();
		}

		public static TypeConverter GetConverter(object component)
		{
			return GetConverter(component, false);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static TypeConverter GetConverter(object component, bool noCustomTypeDesc)
		{
			if (component == null)
			{
				throw new ArgumentNullException("component", "component cannot be null");
			}
			if (!noCustomTypeDesc && component is ICustomTypeDescriptor)
			{
				return ((ICustomTypeDescriptor)component).GetConverter();
			}
			Type type = null;
			AttributeCollection attributes = GetAttributes(component, false);
			TypeConverterAttribute typeConverterAttribute = (TypeConverterAttribute)attributes[typeof(TypeConverterAttribute)];
			if (typeConverterAttribute != null && typeConverterAttribute.ConverterTypeName.Length > 0)
			{
				type = GetTypeFromName(component as IComponent, typeConverterAttribute.ConverterTypeName);
			}
			if (type == null)
			{
				type = FindDefaultConverterType(component.GetType());
			}
			if (type != null)
			{
				ConstructorInfo constructor = type.GetConstructor(new Type[1] { typeof(Type) });
				if (constructor != null)
				{
					return (TypeConverter)constructor.Invoke(new object[1] { component.GetType() });
				}
				return (TypeConverter)Activator.CreateInstance(type);
			}
			return null;
		}

		public static TypeConverter GetConverter(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			Type type2 = null;
			AttributeCollection attributes = GetAttributes(type);
			TypeConverterAttribute typeConverterAttribute = (TypeConverterAttribute)attributes[typeof(TypeConverterAttribute)];
			if (typeConverterAttribute != null && typeConverterAttribute.ConverterTypeName.Length > 0)
			{
				type2 = GetTypeFromName(null, typeConverterAttribute.ConverterTypeName);
			}
			if (type2 == null)
			{
				type2 = FindDefaultConverterType(type);
			}
			if (type2 != null)
			{
				ConstructorInfo constructor = type2.GetConstructor(new Type[1] { typeof(Type) });
				if (constructor != null)
				{
					return (TypeConverter)constructor.Invoke(new object[1] { type });
				}
				return (TypeConverter)Activator.CreateInstance(type2);
			}
			return null;
		}

		private static Type FindDefaultConverterType(Type type)
		{
			Type type2 = null;
			if (type != null)
			{
				if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					return typeof(NullableConverter);
				}
				foreach (DictionaryEntry defaultConverter in DefaultConverters)
				{
					if ((Type)defaultConverter.Key == type)
					{
						return (Type)defaultConverter.Value;
					}
				}
			}
			Type type3 = type;
			while (type3 != null && type3 != typeof(object))
			{
				foreach (DictionaryEntry defaultConverter2 in DefaultConverters)
				{
					Type type4 = (Type)defaultConverter2.Key;
					if (type4.IsAssignableFrom(type3))
					{
						type2 = (Type)defaultConverter2.Value;
						break;
					}
				}
				type3 = type3.BaseType;
			}
			if (type2 == null)
			{
				type2 = ((type == null || !type.IsInterface) ? typeof(TypeConverter) : typeof(ReferenceConverter));
			}
			return type2;
		}

		public static EventDescriptor GetDefaultEvent(Type componentType)
		{
			return GetTypeInfo(componentType).GetDefaultEvent();
		}

		public static EventDescriptor GetDefaultEvent(object component)
		{
			return GetDefaultEvent(component, false);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static EventDescriptor GetDefaultEvent(object component, bool noCustomTypeDesc)
		{
			if (!noCustomTypeDesc && component is ICustomTypeDescriptor)
			{
				return ((ICustomTypeDescriptor)component).GetDefaultEvent();
			}
			IComponent component2 = component as IComponent;
			if (component2 != null && component2.Site != null)
			{
				return GetComponentInfo(component2).GetDefaultEvent();
			}
			return GetTypeInfo(component.GetType()).GetDefaultEvent();
		}

		public static PropertyDescriptor GetDefaultProperty(Type componentType)
		{
			return GetTypeInfo(componentType).GetDefaultProperty();
		}

		public static PropertyDescriptor GetDefaultProperty(object component)
		{
			return GetDefaultProperty(component, false);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static PropertyDescriptor GetDefaultProperty(object component, bool noCustomTypeDesc)
		{
			if (!noCustomTypeDesc && component is ICustomTypeDescriptor)
			{
				return ((ICustomTypeDescriptor)component).GetDefaultProperty();
			}
			IComponent component2 = component as IComponent;
			if (component2 != null && component2.Site != null)
			{
				return GetComponentInfo(component2).GetDefaultProperty();
			}
			return GetTypeInfo(component.GetType()).GetDefaultProperty();
		}

		internal static object CreateEditor(Type t, Type componentType)
		{
			if (t == null)
			{
				return null;
			}
			try
			{
				return Activator.CreateInstance(t);
			}
			catch
			{
			}
			try
			{
				return Activator.CreateInstance(t, componentType);
			}
			catch
			{
			}
			return null;
		}

		private static object FindEditorInTable(Type componentType, Type editorBaseType, Hashtable table)
		{
			object obj = null;
			object obj2 = null;
			if (componentType == null || editorBaseType == null || table == null)
			{
				return null;
			}
			for (Type type = componentType; type != null; type = type.BaseType)
			{
				obj = table[type];
				if (obj != null)
				{
					break;
				}
			}
			if (obj == null)
			{
				Type[] interfaces = componentType.GetInterfaces();
				foreach (Type key in interfaces)
				{
					obj = table[key];
					if (obj != null)
					{
						break;
					}
				}
			}
			if (obj == null)
			{
				return null;
			}
			if (obj is string)
			{
				obj2 = CreateEditor(Type.GetType((string)obj), componentType);
			}
			else if (obj is Type)
			{
				obj2 = CreateEditor((Type)obj, componentType);
			}
			else if (obj.GetType().IsSubclassOf(editorBaseType))
			{
				obj2 = obj;
			}
			if (obj2 != null)
			{
				table[componentType] = obj2;
			}
			return obj2;
		}

		public static object GetEditor(Type componentType, Type editorBaseType)
		{
			Type type = null;
			object obj = null;
			object[] customAttributes = componentType.GetCustomAttributes(typeof(EditorAttribute), true);
			if (customAttributes != null && customAttributes.Length != 0)
			{
				object[] array = customAttributes;
				for (int i = 0; i < array.Length; i++)
				{
					EditorAttribute editorAttribute = (EditorAttribute)array[i];
					type = GetTypeFromName(null, editorAttribute.EditorTypeName);
					if (type != null && type.IsSubclassOf(editorBaseType))
					{
						break;
					}
				}
			}
			if (type != null)
			{
				obj = CreateEditor(type, componentType);
			}
			if (type == null || obj == null)
			{
				RuntimeHelpers.RunClassConstructor(editorBaseType.TypeHandle);
				if (editors != null)
				{
					obj = FindEditorInTable(componentType, editorBaseType, editors[editorBaseType] as Hashtable);
				}
			}
			return obj;
		}

		public static object GetEditor(object component, Type editorBaseType)
		{
			return GetEditor(component, editorBaseType, false);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static object GetEditor(object component, Type editorBaseType, bool noCustomTypeDesc)
		{
			if (component == null)
			{
				throw new ArgumentNullException("component");
			}
			if (editorBaseType == null)
			{
				throw new ArgumentNullException("editorBaseType");
			}
			if (!noCustomTypeDesc && component is ICustomTypeDescriptor)
			{
				return ((ICustomTypeDescriptor)component).GetEditor(editorBaseType);
			}
			object[] customAttributes = component.GetType().GetCustomAttributes(typeof(EditorAttribute), true);
			if (customAttributes.Length == 0)
			{
				return null;
			}
			string assemblyQualifiedName = editorBaseType.AssemblyQualifiedName;
			object[] array = customAttributes;
			for (int i = 0; i < array.Length; i++)
			{
				EditorAttribute editorAttribute = (EditorAttribute)array[i];
				if (editorAttribute.EditorBaseTypeName == assemblyQualifiedName)
				{
					Type type = Type.GetType(editorAttribute.EditorTypeName, true);
					return Activator.CreateInstance(type);
				}
			}
			return null;
		}

		public static EventDescriptorCollection GetEvents(object component)
		{
			return GetEvents(component, false);
		}

		public static EventDescriptorCollection GetEvents(Type componentType)
		{
			return GetEvents(componentType, null);
		}

		public static EventDescriptorCollection GetEvents(object component, Attribute[] attributes)
		{
			return GetEvents(component, attributes, false);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static EventDescriptorCollection GetEvents(object component, bool noCustomTypeDesc)
		{
			if (!noCustomTypeDesc && component is ICustomTypeDescriptor)
			{
				return ((ICustomTypeDescriptor)component).GetEvents();
			}
			IComponent component2 = component as IComponent;
			if (component2 != null && component2.Site != null)
			{
				return GetComponentInfo(component2).GetEvents();
			}
			return GetTypeInfo(component.GetType()).GetEvents();
		}

		public static EventDescriptorCollection GetEvents(Type componentType, Attribute[] attributes)
		{
			return GetTypeInfo(componentType).GetEvents(attributes);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static EventDescriptorCollection GetEvents(object component, Attribute[] attributes, bool noCustomTypeDesc)
		{
			if (!noCustomTypeDesc && component is ICustomTypeDescriptor)
			{
				return ((ICustomTypeDescriptor)component).GetEvents(attributes);
			}
			IComponent component2 = component as IComponent;
			if (component2 != null && component2.Site != null)
			{
				return GetComponentInfo(component2).GetEvents(attributes);
			}
			return GetTypeInfo(component.GetType()).GetEvents(attributes);
		}

		public static PropertyDescriptorCollection GetProperties(object component)
		{
			return GetProperties(component, false);
		}

		public static PropertyDescriptorCollection GetProperties(Type componentType)
		{
			return GetProperties(componentType, null);
		}

		public static PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes)
		{
			return GetProperties(component, attributes, false);
		}

		public static PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes, bool noCustomTypeDesc)
		{
			if (component == null)
			{
				return PropertyDescriptorCollection.Empty;
			}
			if (!noCustomTypeDesc && component is ICustomTypeDescriptor)
			{
				return ((ICustomTypeDescriptor)component).GetProperties(attributes);
			}
			IComponent component2 = component as IComponent;
			if (component2 != null && component2.Site != null)
			{
				return GetComponentInfo(component2).GetProperties(attributes);
			}
			return GetTypeInfo(component.GetType()).GetProperties(attributes);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static PropertyDescriptorCollection GetProperties(object component, bool noCustomTypeDesc)
		{
			if (component == null)
			{
				return PropertyDescriptorCollection.Empty;
			}
			if (!noCustomTypeDesc && component is ICustomTypeDescriptor)
			{
				return ((ICustomTypeDescriptor)component).GetProperties();
			}
			IComponent component2 = component as IComponent;
			if (component2 != null && component2.Site != null)
			{
				return GetComponentInfo(component2).GetProperties();
			}
			return GetTypeInfo(component.GetType()).GetProperties();
		}

		public static PropertyDescriptorCollection GetProperties(Type componentType, Attribute[] attributes)
		{
			return GetTypeInfo(componentType).GetProperties(attributes);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static TypeDescriptionProvider GetProvider(object instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			TypeDescriptionProvider typeDescriptionProvider = null;
			lock (componentDescriptionProvidersLock)
			{
				System.ComponentModel.WeakObjectWrapper key = new System.ComponentModel.WeakObjectWrapper(instance);
				LinkedList<TypeDescriptionProvider> value;
				if (componentDescriptionProviders.TryGetValue(key, out value) && value.Count > 0)
				{
					typeDescriptionProvider = value.Last.Value;
				}
				key = null;
			}
			if (typeDescriptionProvider == null)
			{
				typeDescriptionProvider = GetProvider(instance.GetType());
			}
			if (typeDescriptionProvider == null)
			{
				return new DefaultTypeDescriptionProvider();
			}
			return new WrappedTypeDescriptionProvider(typeDescriptionProvider);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static TypeDescriptionProvider GetProvider(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			TypeDescriptionProvider typeDescriptionProvider = null;
			lock (typeDescriptionProvidersLock)
			{
				LinkedList<TypeDescriptionProvider> value;
				while (!typeDescriptionProviders.TryGetValue(type, out value))
				{
					value = null;
					type = type.BaseType;
					if (type == null)
					{
						break;
					}
				}
				if (value != null && value.Count > 0)
				{
					typeDescriptionProvider = value.Last.Value;
				}
			}
			if (typeDescriptionProvider == null)
			{
				return new DefaultTypeDescriptionProvider();
			}
			return new WrappedTypeDescriptionProvider(typeDescriptionProvider);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static Type GetReflectionType(object instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			return instance.GetType();
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static Type GetReflectionType(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			return type;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[System.MonoNotSupported("Associations not supported")]
		public static void CreateAssociation(object primary, object secondary)
		{
			throw new NotImplementedException();
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[System.MonoNotSupported("Associations not supported")]
		public static object GetAssociation(Type type, object primary)
		{
			throw new NotImplementedException();
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[System.MonoNotSupported("Associations not supported")]
		public static void RemoveAssociation(object primary, object secondary)
		{
			throw new NotImplementedException();
		}

		[System.MonoNotSupported("Associations not supported")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static void RemoveAssociations(object primary)
		{
			throw new NotImplementedException();
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static void RemoveProvider(TypeDescriptionProvider provider, object instance)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			lock (componentDescriptionProvidersLock)
			{
				System.ComponentModel.WeakObjectWrapper key = new System.ComponentModel.WeakObjectWrapper(instance);
				LinkedList<TypeDescriptionProvider> value;
				if (componentDescriptionProviders.TryGetValue(key, out value) && value.Count > 0)
				{
					RemoveProvider(provider, value);
				}
				key = null;
			}
			RefreshEventHandler refreshed = TypeDescriptor.Refreshed;
			if (refreshed != null)
			{
				refreshed(new RefreshEventArgs(instance));
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static void RemoveProvider(TypeDescriptionProvider provider, Type type)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			lock (typeDescriptionProvidersLock)
			{
				LinkedList<TypeDescriptionProvider> value;
				if (typeDescriptionProviders.TryGetValue(type, out value) && value.Count > 0)
				{
					RemoveProvider(provider, value);
				}
			}
			RefreshEventHandler refreshed = TypeDescriptor.Refreshed;
			if (refreshed != null)
			{
				refreshed(new RefreshEventArgs(type));
			}
		}

		private static void RemoveProvider(TypeDescriptionProvider provider, LinkedList<TypeDescriptionProvider> plist)
		{
			LinkedListNode<TypeDescriptionProvider> linkedListNode = plist.Last;
			LinkedListNode<TypeDescriptionProvider> first = plist.First;
			while (true)
			{
				TypeDescriptionProvider value = linkedListNode.Value;
				if (value == provider)
				{
					plist.Remove(linkedListNode);
					break;
				}
				if (linkedListNode == first)
				{
					break;
				}
				linkedListNode = linkedListNode.Previous;
			}
		}

		public static void SortDescriptorArray(IList infos)
		{
			string[] array = new string[infos.Count];
			object[] array2 = new object[infos.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = ((MemberDescriptor)infos[i]).Name;
				array2[i] = infos[i];
			}
			Array.Sort(array, array2);
			infos.Clear();
			object[] array3 = array2;
			foreach (object value in array3)
			{
				infos.Add(value);
			}
		}

		public static void Refresh(Assembly assembly)
		{
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
			{
				Refresh(type);
			}
		}

		public static void Refresh(Module module)
		{
			Type[] types = module.GetTypes();
			foreach (Type type in types)
			{
				Refresh(type);
			}
		}

		public static void Refresh(object component)
		{
			lock (componentTable)
			{
				componentTable.Remove(component);
			}
			if (TypeDescriptor.Refreshed != null)
			{
				TypeDescriptor.Refreshed(new RefreshEventArgs(component));
			}
		}

		public static void Refresh(Type type)
		{
			lock (typeTable)
			{
				typeTable.Remove(type);
			}
			if (TypeDescriptor.Refreshed != null)
			{
				TypeDescriptor.Refreshed(new RefreshEventArgs(type));
			}
		}

		private static void OnComponentDisposed(object sender, EventArgs args)
		{
			lock (componentTable)
			{
				componentTable.Remove(sender);
			}
		}

		internal static System.ComponentModel.ComponentInfo GetComponentInfo(IComponent com)
		{
			lock (componentTable)
			{
				System.ComponentModel.ComponentInfo componentInfo = (System.ComponentModel.ComponentInfo)componentTable[com];
				if (componentInfo == null)
				{
					if (onDispose == null)
					{
						onDispose = OnComponentDisposed;
					}
					com.Disposed += onDispose;
					componentInfo = new System.ComponentModel.ComponentInfo(com);
					componentTable[com] = componentInfo;
				}
				return componentInfo;
			}
		}

		internal static System.ComponentModel.TypeInfo GetTypeInfo(Type type)
		{
			lock (typeTable)
			{
				System.ComponentModel.TypeInfo typeInfo = (System.ComponentModel.TypeInfo)typeTable[type];
				if (typeInfo == null)
				{
					typeInfo = new System.ComponentModel.TypeInfo(type);
					typeTable[type] = typeInfo;
				}
				return typeInfo;
			}
		}

		private static Type GetTypeFromName(IComponent component, string typeName)
		{
			Type type = null;
			if (component != null && component.Site != null)
			{
				ITypeResolutionService typeResolutionService = (ITypeResolutionService)component.Site.GetService(typeof(ITypeResolutionService));
				if (typeResolutionService != null)
				{
					type = typeResolutionService.GetType(typeName);
				}
			}
			if (type == null)
			{
				type = Type.GetType(typeName);
			}
			return type;
		}
	}
}
