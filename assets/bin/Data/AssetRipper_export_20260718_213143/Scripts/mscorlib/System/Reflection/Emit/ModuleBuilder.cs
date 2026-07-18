using System.Collections;
using System.Diagnostics.SymbolStore;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[ComDefaultInterface(typeof(_ModuleBuilder))]
	[ClassInterface(ClassInterfaceType.None)]
	public class ModuleBuilder : Module, _ModuleBuilder
	{
		private UIntPtr dynamic_image;

		private int num_types;

		private TypeBuilder[] types;

		private CustomAttributeBuilder[] cattrs;

		private byte[] guid;

		private int table_idx;

		internal AssemblyBuilder assemblyb;

		private MethodBuilder[] global_methods;

		private FieldBuilder[] global_fields;

		private bool is_main;

		private MonoResource[] resources;

		private TypeBuilder global_type;

		private Type global_type_created;

		private Hashtable name_cache;

		private Hashtable us_string_cache = new Hashtable();

		private int[] table_indexes;

		private bool transient;

		private ModuleBuilderTokenGenerator token_gen;

		private Hashtable resource_writers;

		private ISymbolWriter symbolWriter;

		private static readonly char[] type_modifiers = new char[3] { '&', '[', '*' };

		public override string FullyQualifiedName
		{
			get
			{
				return fqname;
			}
		}

		internal string FileName
		{
			get
			{
				return fqname;
			}
		}

		internal bool IsMain
		{
			set
			{
				is_main = value;
			}
		}

		internal ModuleBuilder(AssemblyBuilder assb, string name, string fullyqname, bool emitSymbolInfo, bool transient)
		{
			base.name = (scopename = name);
			fqname = fullyqname;
			base.assembly = (assemblyb = assb);
			this.transient = transient;
			guid = Guid.FastNewGuidArray();
			table_idx = get_next_table_index(this, 0, true);
			name_cache = new Hashtable();
			basic_init(this);
			CreateGlobalType();
			if (assb.IsRun)
			{
				TypeBuilder typeBuilder = new TypeBuilder(this, TypeAttributes.Abstract, 16777215);
				Type ab = typeBuilder.CreateType();
				set_wrappers_type(this, ab);
			}
			if (emitSymbolInfo)
			{
				Assembly assembly = Assembly.LoadWithPartialName("Mono.CompilerServices.SymbolWriter");
				if (assembly == null)
				{
					throw new ExecutionEngineException("The assembly for default symbol writer cannot be loaded");
				}
				Type type = assembly.GetType("Mono.CompilerServices.SymbolWriter.SymbolWriterImpl");
				if (type == null)
				{
					throw new ExecutionEngineException("The type that implements the default symbol writer interface cannot be found");
				}
				symbolWriter = (ISymbolWriter)Activator.CreateInstance(type, this);
				string text = fqname;
				if (assemblyb.AssemblyDir != null)
				{
					text = Path.Combine(assemblyb.AssemblyDir, text);
				}
				symbolWriter.Initialize(IntPtr.Zero, text, true);
			}
		}

		void _ModuleBuilder.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _ModuleBuilder.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _ModuleBuilder.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _ModuleBuilder.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void basic_init(ModuleBuilder ab);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_wrappers_type(ModuleBuilder mb, Type ab);

		public bool IsTransient()
		{
			return transient;
		}

		public void CreateGlobalFunctions()
		{
			if (global_type_created != null)
			{
				throw new InvalidOperationException("global methods already created");
			}
			if (global_type != null)
			{
				global_type_created = global_type.CreateType();
			}
		}

		public FieldBuilder DefineInitializedData(string name, byte[] data, FieldAttributes attributes)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			FieldBuilder fieldBuilder = DefineUninitializedData(name, data.Length, attributes | FieldAttributes.HasFieldRVA);
			fieldBuilder.SetRVAData(data);
			return fieldBuilder;
		}

		public FieldBuilder DefineUninitializedData(string name, int size, FieldAttributes attributes)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (global_type_created != null)
			{
				throw new InvalidOperationException("global fields already created");
			}
			if (size <= 0 || size > 4128768)
			{
				throw new ArgumentException("size", "Data size must be > 0 and < 0x3f0000");
			}
			CreateGlobalType();
			string className = "$ArrayType$" + size;
			Type type = GetType(className, false, false);
			if (type == null)
			{
				TypeBuilder typeBuilder = DefineType(className, TypeAttributes.Public | TypeAttributes.ExplicitLayout | TypeAttributes.Sealed, assemblyb.corlib_value_type, null, PackingSize.Size1, size);
				typeBuilder.CreateType();
				type = typeBuilder;
			}
			FieldBuilder fieldBuilder = global_type.DefineField(name, type, attributes | FieldAttributes.Static);
			if (global_fields != null)
			{
				FieldBuilder[] array = new FieldBuilder[global_fields.Length + 1];
				Array.Copy(global_fields, array, global_fields.Length);
				array[global_fields.Length] = fieldBuilder;
				global_fields = array;
			}
			else
			{
				global_fields = new FieldBuilder[1];
				global_fields[0] = fieldBuilder;
			}
			return fieldBuilder;
		}

		private void addGlobalMethod(MethodBuilder mb)
		{
			if (global_methods != null)
			{
				MethodBuilder[] array = new MethodBuilder[global_methods.Length + 1];
				Array.Copy(global_methods, array, global_methods.Length);
				array[global_methods.Length] = mb;
				global_methods = array;
			}
			else
			{
				global_methods = new MethodBuilder[1];
				global_methods[0] = mb;
			}
		}

		public MethodBuilder DefineGlobalMethod(string name, MethodAttributes attributes, Type returnType, Type[] parameterTypes)
		{
			return DefineGlobalMethod(name, attributes, CallingConventions.Standard, returnType, parameterTypes);
		}

		public MethodBuilder DefineGlobalMethod(string name, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes)
		{
			return DefineGlobalMethod(name, attributes, callingConvention, returnType, null, null, parameterTypes, null, null);
		}

		public MethodBuilder DefineGlobalMethod(string name, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] requiredReturnTypeCustomModifiers, Type[] optionalReturnTypeCustomModifiers, Type[] parameterTypes, Type[][] requiredParameterTypeCustomModifiers, Type[][] optionalParameterTypeCustomModifiers)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if ((attributes & MethodAttributes.Static) == 0)
			{
				throw new ArgumentException("global methods must be static");
			}
			if (global_type_created != null)
			{
				throw new InvalidOperationException("global methods already created");
			}
			CreateGlobalType();
			MethodBuilder methodBuilder = global_type.DefineMethod(name, attributes, callingConvention, returnType, requiredReturnTypeCustomModifiers, optionalReturnTypeCustomModifiers, parameterTypes, requiredParameterTypeCustomModifiers, optionalParameterTypeCustomModifiers);
			addGlobalMethod(methodBuilder);
			return methodBuilder;
		}

		public MethodBuilder DefinePInvokeMethod(string name, string dllName, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, CallingConvention nativeCallConv, CharSet nativeCharSet)
		{
			return DefinePInvokeMethod(name, dllName, name, attributes, callingConvention, returnType, parameterTypes, nativeCallConv, nativeCharSet);
		}

		public MethodBuilder DefinePInvokeMethod(string name, string dllName, string entryName, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, CallingConvention nativeCallConv, CharSet nativeCharSet)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if ((attributes & MethodAttributes.Static) == 0)
			{
				throw new ArgumentException("global methods must be static");
			}
			if (global_type_created != null)
			{
				throw new InvalidOperationException("global methods already created");
			}
			CreateGlobalType();
			MethodBuilder methodBuilder = global_type.DefinePInvokeMethod(name, dllName, entryName, attributes, callingConvention, returnType, parameterTypes, nativeCallConv, nativeCharSet);
			addGlobalMethod(methodBuilder);
			return methodBuilder;
		}

		public TypeBuilder DefineType(string name)
		{
			return DefineType(name, TypeAttributes.NotPublic);
		}

		public TypeBuilder DefineType(string name, TypeAttributes attr)
		{
			if ((attr & TypeAttributes.ClassSemanticsMask) != TypeAttributes.NotPublic)
			{
				return DefineType(name, attr, null, null);
			}
			return DefineType(name, attr, typeof(object), null);
		}

		public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent)
		{
			return DefineType(name, attr, parent, null);
		}

		private void AddType(TypeBuilder tb)
		{
			if (types != null)
			{
				if (types.Length == num_types)
				{
					TypeBuilder[] destinationArray = new TypeBuilder[types.Length * 2];
					Array.Copy(types, destinationArray, num_types);
					types = destinationArray;
				}
			}
			else
			{
				types = new TypeBuilder[1];
			}
			types[num_types] = tb;
			num_types++;
		}

		private TypeBuilder DefineType(string name, TypeAttributes attr, Type parent, Type[] interfaces, PackingSize packingSize, int typesize)
		{
			if (name_cache.ContainsKey(name))
			{
				throw new ArgumentException("Duplicate type name within an assembly.");
			}
			TypeBuilder typeBuilder = new TypeBuilder(this, name, attr, parent, interfaces, packingSize, typesize, null);
			AddType(typeBuilder);
			name_cache.Add(name, typeBuilder);
			return typeBuilder;
		}

		internal void RegisterTypeName(TypeBuilder tb, string name)
		{
			name_cache.Add(name, tb);
		}

		internal TypeBuilder GetRegisteredType(string name)
		{
			return (TypeBuilder)name_cache[name];
		}

		[ComVisible(true)]
		public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent, Type[] interfaces)
		{
			return DefineType(name, attr, parent, interfaces, PackingSize.Unspecified, 0);
		}

		public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent, int typesize)
		{
			return DefineType(name, attr, parent, null, PackingSize.Unspecified, 0);
		}

		public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent, PackingSize packsize)
		{
			return DefineType(name, attr, parent, null, packsize, 0);
		}

		public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent, PackingSize packingSize, int typesize)
		{
			return DefineType(name, attr, parent, null, packingSize, typesize);
		}

		public MethodInfo GetArrayMethod(Type arrayClass, string methodName, CallingConventions callingConvention, Type returnType, Type[] parameterTypes)
		{
			return new MonoArrayMethod(arrayClass, methodName, callingConvention, returnType, parameterTypes);
		}

		public EnumBuilder DefineEnum(string name, TypeAttributes visibility, Type underlyingType)
		{
			if (name_cache.Contains(name))
			{
				throw new ArgumentException("Duplicate type name within an assembly.");
			}
			EnumBuilder enumBuilder = new EnumBuilder(this, name, visibility, underlyingType);
			TypeBuilder typeBuilder = enumBuilder.GetTypeBuilder();
			AddType(typeBuilder);
			name_cache.Add(name, typeBuilder);
			return enumBuilder;
		}

		[ComVisible(true)]
		public override Type GetType(string className)
		{
			return GetType(className, false, false);
		}

		[ComVisible(true)]
		public override Type GetType(string className, bool ignoreCase)
		{
			return GetType(className, false, ignoreCase);
		}

		private TypeBuilder search_in_array(TypeBuilder[] arr, int validElementsInArray, string className)
		{
			for (int i = 0; i < validElementsInArray; i++)
			{
				if (string.Compare(className, arr[i].FullName, true, CultureInfo.InvariantCulture) == 0)
				{
					return arr[i];
				}
			}
			return null;
		}

		private TypeBuilder search_nested_in_array(TypeBuilder[] arr, int validElementsInArray, string className)
		{
			for (int i = 0; i < validElementsInArray; i++)
			{
				if (string.Compare(className, arr[i].Name, true, CultureInfo.InvariantCulture) == 0)
				{
					return arr[i];
				}
			}
			return null;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Type create_modified_type(TypeBuilder tb, string modifiers);

		private TypeBuilder GetMaybeNested(TypeBuilder t, string className)
		{
			int num = className.IndexOf('+');
			if (num < 0)
			{
				if (t.subtypes != null)
				{
					return search_nested_in_array(t.subtypes, t.subtypes.Length, className);
				}
				return null;
			}
			if (t.subtypes != null)
			{
				string className2 = className.Substring(0, num);
				string className3 = className.Substring(num + 1);
				TypeBuilder typeBuilder = search_nested_in_array(t.subtypes, t.subtypes.Length, className2);
				if (typeBuilder != null)
				{
					return GetMaybeNested(typeBuilder, className3);
				}
			}
			return null;
		}

		[ComVisible(true)]
		public override Type GetType(string className, bool throwOnError, bool ignoreCase)
		{
			if (className == null)
			{
				throw new ArgumentNullException("className");
			}
			if (className.Length == 0)
			{
				throw new ArgumentException("className");
			}
			string message = className;
			TypeBuilder typeBuilder = null;
			if (types == null && throwOnError)
			{
				throw new TypeLoadException(className);
			}
			int num = className.IndexOfAny(type_modifiers);
			string text;
			if (num >= 0)
			{
				text = className.Substring(num);
				className = className.Substring(0, num);
			}
			else
			{
				text = null;
			}
			if (!ignoreCase)
			{
				typeBuilder = name_cache[className] as TypeBuilder;
			}
			else
			{
				num = className.IndexOf('+');
				if (num < 0)
				{
					if (types != null)
					{
						typeBuilder = search_in_array(types, num_types, className);
					}
				}
				else
				{
					string className2 = className.Substring(0, num);
					string className3 = className.Substring(num + 1);
					typeBuilder = search_in_array(types, num_types, className2);
					if (typeBuilder != null)
					{
						typeBuilder = GetMaybeNested(typeBuilder, className3);
					}
				}
			}
			if (typeBuilder == null && throwOnError)
			{
				throw new TypeLoadException(message);
			}
			if (typeBuilder != null && text != null)
			{
				Type type = create_modified_type(typeBuilder, text);
				typeBuilder = type as TypeBuilder;
				if (typeBuilder == null)
				{
					return type;
				}
			}
			if (typeBuilder != null && typeBuilder.is_created)
			{
				return typeBuilder.CreateType();
			}
			return typeBuilder;
		}

		internal int get_next_table_index(object obj, int table, bool inc)
		{
			if (table_indexes == null)
			{
				table_indexes = new int[64];
				for (int i = 0; i < 64; i++)
				{
					table_indexes[i] = 1;
				}
				table_indexes[2] = 2;
			}
			if (inc)
			{
				return table_indexes[table]++;
			}
			return table_indexes[table];
		}

		public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
		{
			if (cattrs != null)
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

		public ISymbolWriter GetSymWriter()
		{
			return symbolWriter;
		}

		public ISymbolDocumentWriter DefineDocument(string url, Guid language, Guid languageVendor, Guid documentType)
		{
			if (symbolWriter != null)
			{
				return symbolWriter.DefineDocument(url, language, languageVendor, documentType);
			}
			return null;
		}

		public override Type[] GetTypes()
		{
			if (types == null)
			{
				return Type.EmptyTypes;
			}
			int num = num_types;
			Type[] array = new Type[num];
			Array.Copy(types, array, num);
			for (int i = 0; i < array.Length; i++)
			{
				if (types[i].is_created)
				{
					array[i] = types[i].CreateType();
				}
			}
			return array;
		}

		public IResourceWriter DefineResource(string name, string description, ResourceAttributes attribute)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name == string.Empty)
			{
				throw new ArgumentException("name cannot be empty");
			}
			if (transient)
			{
				throw new InvalidOperationException("The module is transient");
			}
			if (!assemblyb.IsSave)
			{
				throw new InvalidOperationException("The assembly is transient");
			}
			ResourceWriter resourceWriter = new ResourceWriter(new MemoryStream());
			if (resource_writers == null)
			{
				resource_writers = new Hashtable();
			}
			resource_writers[name] = resourceWriter;
			if (resources != null)
			{
				MonoResource[] destinationArray = new MonoResource[resources.Length + 1];
				Array.Copy(resources, destinationArray, resources.Length);
				resources = destinationArray;
			}
			else
			{
				resources = new MonoResource[1];
			}
			int num = resources.Length - 1;
			resources[num].name = name;
			resources[num].attrs = attribute;
			return resourceWriter;
		}

		public IResourceWriter DefineResource(string name, string description)
		{
			return DefineResource(name, description, ResourceAttributes.Public);
		}

		[MonoTODO]
		public void DefineUnmanagedResource(byte[] resource)
		{
			if (resource == null)
			{
				throw new ArgumentNullException("resource");
			}
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void DefineUnmanagedResource(string resourceFileName)
		{
			if (resourceFileName == null)
			{
				throw new ArgumentNullException("resourceFileName");
			}
			if (resourceFileName == string.Empty)
			{
				throw new ArgumentException("resourceFileName");
			}
			if (!File.Exists(resourceFileName) || Directory.Exists(resourceFileName))
			{
				throw new FileNotFoundException("File '" + resourceFileName + "' does not exists or is a directory.");
			}
			throw new NotImplementedException();
		}

		public void DefineManifestResource(string name, Stream stream, ResourceAttributes attribute)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name == string.Empty)
			{
				throw new ArgumentException("name cannot be empty");
			}
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (transient)
			{
				throw new InvalidOperationException("The module is transient");
			}
			if (!assemblyb.IsSave)
			{
				throw new InvalidOperationException("The assembly is transient");
			}
			if (resources != null)
			{
				MonoResource[] destinationArray = new MonoResource[resources.Length + 1];
				Array.Copy(resources, destinationArray, resources.Length);
				resources = destinationArray;
			}
			else
			{
				resources = new MonoResource[1];
			}
			int num = resources.Length - 1;
			resources[num].name = name;
			resources[num].attrs = attribute;
			resources[num].stream = stream;
		}

		[MonoTODO]
		public void SetSymCustomAttribute(string name, byte[] data)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void SetUserEntryPoint(MethodInfo entryPoint)
		{
			if (entryPoint == null)
			{
				throw new ArgumentNullException("entryPoint");
			}
			if (entryPoint.DeclaringType.Module != this)
			{
				throw new InvalidOperationException("entryPoint is not contained in this module");
			}
			throw new NotImplementedException();
		}

		public MethodToken GetMethodToken(MethodInfo method)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			if (method.DeclaringType.Module != this)
			{
				throw new InvalidOperationException("The method is not in this module");
			}
			return new MethodToken(GetToken(method));
		}

		public MethodToken GetArrayMethodToken(Type arrayClass, string methodName, CallingConventions callingConvention, Type returnType, Type[] parameterTypes)
		{
			return GetMethodToken(GetArrayMethod(arrayClass, methodName, callingConvention, returnType, parameterTypes));
		}

		[ComVisible(true)]
		public MethodToken GetConstructorToken(ConstructorInfo con)
		{
			if (con == null)
			{
				throw new ArgumentNullException("con");
			}
			return new MethodToken(GetToken(con));
		}

		public FieldToken GetFieldToken(FieldInfo field)
		{
			if (field == null)
			{
				throw new ArgumentNullException("field");
			}
			if (field.DeclaringType.Module != this)
			{
				throw new InvalidOperationException("The method is not in this module");
			}
			return new FieldToken(GetToken(field));
		}

		[MonoTODO]
		public SignatureToken GetSignatureToken(byte[] sigBytes, int sigLength)
		{
			throw new NotImplementedException();
		}

		public SignatureToken GetSignatureToken(SignatureHelper sigHelper)
		{
			if (sigHelper == null)
			{
				throw new ArgumentNullException("sigHelper");
			}
			return new SignatureToken(GetToken(sigHelper));
		}

		public StringToken GetStringConstant(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			return new StringToken(GetToken(str));
		}

		public TypeToken GetTypeToken(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (type.IsByRef)
			{
				throw new ArgumentException("type can't be a byref type", "type");
			}
			if (!IsTransient() && type.Module is ModuleBuilder && ((ModuleBuilder)type.Module).IsTransient())
			{
				throw new InvalidOperationException("a non-transient module can't reference a transient module");
			}
			return new TypeToken(GetToken(type));
		}

		public TypeToken GetTypeToken(string name)
		{
			return GetTypeToken(GetType(name));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int getUSIndex(ModuleBuilder mb, string str);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int getToken(ModuleBuilder mb, object obj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int getMethodToken(ModuleBuilder mb, MethodInfo method, Type[] opt_param_types);

		internal int GetToken(string str)
		{
			if (us_string_cache.Contains(str))
			{
				return (int)us_string_cache[str];
			}
			int uSIndex = getUSIndex(this, str);
			us_string_cache[str] = uSIndex;
			return uSIndex;
		}

		internal int GetToken(MemberInfo member)
		{
			return getToken(this, member);
		}

		internal int GetToken(MethodInfo method, Type[] opt_param_types)
		{
			return getMethodToken(this, method, opt_param_types);
		}

		internal int GetToken(SignatureHelper helper)
		{
			return getToken(this, helper);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void RegisterToken(object obj, int token);

		internal TokenGenerator GetTokenGenerator()
		{
			if (token_gen == null)
			{
				token_gen = new ModuleBuilderTokenGenerator(this);
			}
			return token_gen;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void build_metadata(ModuleBuilder mb);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void WriteToFile(IntPtr handle);

		internal void Save()
		{
			if (transient && !is_main)
			{
				return;
			}
			if (types != null)
			{
				for (int i = 0; i < num_types; i++)
				{
					if (!types[i].is_created)
					{
						throw new NotSupportedException("Type '" + types[i].FullName + "' was not completed.");
					}
				}
			}
			if (global_type != null && global_type_created == null)
			{
				global_type_created = global_type.CreateType();
			}
			if (resources != null)
			{
				for (int j = 0; j < resources.Length; j++)
				{
					IResourceWriter resourceWriter;
					if (resource_writers != null && (resourceWriter = resource_writers[resources[j].name] as IResourceWriter) != null)
					{
						ResourceWriter resourceWriter2 = (ResourceWriter)resourceWriter;
						resourceWriter2.Generate();
						MemoryStream memoryStream = (MemoryStream)resourceWriter2.Stream;
						resources[j].data = new byte[memoryStream.Length];
						memoryStream.Seek(0L, SeekOrigin.Begin);
						memoryStream.Read(resources[j].data, 0, (int)memoryStream.Length);
						continue;
					}
					Stream stream = resources[j].stream;
					if (stream != null)
					{
						try
						{
							long length = stream.Length;
							resources[j].data = new byte[length];
							stream.Seek(0L, SeekOrigin.Begin);
							stream.Read(resources[j].data, 0, (int)length);
						}
						catch
						{
						}
					}
				}
			}
			build_metadata(this);
			string text = fqname;
			if (assemblyb.AssemblyDir != null)
			{
				text = Path.Combine(assemblyb.AssemblyDir, text);
			}
			try
			{
				File.Delete(text);
			}
			catch
			{
			}
			using (FileStream fileStream = new FileStream(text, FileMode.Create, FileAccess.Write))
			{
				WriteToFile(fileStream.Handle);
			}
			File.SetAttributes(text, (FileAttributes)(-2147483648));
			if (types != null && symbolWriter != null)
			{
				for (int k = 0; k < num_types; k++)
				{
					types[k].GenerateDebugInfo(symbolWriter);
				}
				symbolWriter.Close();
			}
		}

		internal void CreateGlobalType()
		{
			if (global_type == null)
			{
				global_type = new TypeBuilder(this, TypeAttributes.NotPublic, 1);
			}
		}

		internal override Guid GetModuleVersionId()
		{
			return new Guid(guid);
		}

		internal static Guid Mono_GetGuid(ModuleBuilder mb)
		{
			return mb.GetModuleVersionId();
		}
	}
}
