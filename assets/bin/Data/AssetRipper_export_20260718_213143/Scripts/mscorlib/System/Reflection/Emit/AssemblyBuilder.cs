using System.Collections;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using Mono.Security;

namespace System.Reflection.Emit
{
	[ClassInterface(ClassInterfaceType.None)]
	[ComVisible(true)]
	[ComDefaultInterface(typeof(_AssemblyBuilder))]
	public sealed class AssemblyBuilder : Assembly, _AssemblyBuilder
	{
		private const AssemblyBuilderAccess COMPILER_ACCESS = (AssemblyBuilderAccess)2048;

		private UIntPtr dynamic_assembly;

		private MethodInfo entry_point;

		private ModuleBuilder[] modules;

		private string name;

		private string dir;

		private CustomAttributeBuilder[] cattrs;

		private MonoResource[] resources;

		private byte[] public_key;

		private string version;

		private string culture;

		private uint algid;

		private uint flags;

		private PEFileKinds pekind = PEFileKinds.Dll;

		private bool delay_sign;

		private uint access;

		private Module[] loaded_modules;

		private MonoWin32Resource[] win32_resources;

		private RefEmitPermissionSet[] permissions_minimum;

		private RefEmitPermissionSet[] permissions_optional;

		private RefEmitPermissionSet[] permissions_refused;

		private PortableExecutableKinds peKind;

		private ImageFileMachine machine;

		private bool corlib_internal;

		private Type[] type_forwarders;

		private byte[] pktoken;

		internal Type corlib_object_type = typeof(object);

		internal Type corlib_value_type = typeof(ValueType);

		internal Type corlib_enum_type = typeof(Enum);

		internal Type corlib_void_type = typeof(void);

		private ArrayList resource_writers;

		private Win32VersionResource version_res;

		private bool created;

		private bool is_module_only;

		private StrongName sn;

		private NativeResourceType native_resource;

		private readonly bool is_compiler_context;

		private string versioninfo_culture;

		private ModuleBuilder manifest_module;

		public override string CodeBase
		{
			get
			{
				throw not_supported();
			}
		}

		public override MethodInfo EntryPoint
		{
			get
			{
				return entry_point;
			}
		}

		public override string Location
		{
			get
			{
				throw not_supported();
			}
		}

		public override string ImageRuntimeVersion
		{
			get
			{
				return base.ImageRuntimeVersion;
			}
		}

		[MonoTODO]
		public override bool ReflectionOnly
		{
			get
			{
				return base.ReflectionOnly;
			}
		}

		internal bool IsCompilerContext
		{
			get
			{
				return is_compiler_context;
			}
		}

		internal bool IsSave
		{
			get
			{
				return access != 1;
			}
		}

		internal bool IsRun
		{
			get
			{
				return access == 1 || access == 3;
			}
		}

		internal string AssemblyDir
		{
			get
			{
				return dir;
			}
		}

		internal bool IsModuleOnly
		{
			get
			{
				return is_module_only;
			}
			set
			{
				is_module_only = value;
			}
		}

		internal AssemblyBuilder(AssemblyName n, string directory, AssemblyBuilderAccess access, bool corlib_internal)
		{
			is_compiler_context = (access & (AssemblyBuilderAccess)2048) != 0;
			access &= (AssemblyBuilderAccess)(-2049);
			if (!Enum.IsDefined(typeof(AssemblyBuilderAccess), access))
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Argument value {0} is not valid.", (int)access), "access");
			}
			name = n.Name;
			this.access = (uint)access;
			flags = (uint)n.Flags;
			if (IsSave && (directory == null || directory.Length == 0))
			{
				dir = Directory.GetCurrentDirectory();
			}
			else
			{
				dir = directory;
			}
			if (n.CultureInfo != null)
			{
				culture = n.CultureInfo.Name;
				versioninfo_culture = n.CultureInfo.Name;
			}
			Version version = n.Version;
			if (version != null)
			{
				this.version = version.ToString();
			}
			if (n.KeyPair != null)
			{
				sn = n.KeyPair.StrongName();
			}
			else
			{
				byte[] publicKey = n.GetPublicKey();
				if (publicKey != null && publicKey.Length > 0)
				{
					sn = new StrongName(publicKey);
				}
			}
			if (sn != null)
			{
				flags |= 1u;
			}
			this.corlib_internal = corlib_internal;
			if (sn != null)
			{
				pktoken = new byte[sn.PublicKeyToken.Length * 2];
				int num = 0;
				byte[] publicKeyToken = sn.PublicKeyToken;
				foreach (byte b in publicKeyToken)
				{
					string text = b.ToString("x2");
					pktoken[num++] = (byte)text[0];
					pktoken[num++] = (byte)text[1];
				}
			}
			basic_init(this);
		}

		void _AssemblyBuilder.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _AssemblyBuilder.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _AssemblyBuilder.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _AssemblyBuilder.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void basic_init(AssemblyBuilder ab);

		public void AddResourceFile(string name, string fileName)
		{
			AddResourceFile(name, fileName, ResourceAttributes.Public);
		}

		public void AddResourceFile(string name, string fileName, ResourceAttributes attribute)
		{
			AddResourceFile(name, fileName, attribute, true);
		}

		private void AddResourceFile(string name, string fileName, ResourceAttributes attribute, bool fileNeedsToExists)
		{
			check_name_and_filename(name, fileName, fileNeedsToExists);
			if (dir != null)
			{
				fileName = Path.Combine(dir, fileName);
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
			resources[num].filename = fileName;
			resources[num].attrs = attribute;
		}

		internal void AddPermissionRequests(PermissionSet required, PermissionSet optional, PermissionSet refused)
		{
		}

		internal void EmbedResourceFile(string name, string fileName)
		{
			EmbedResourceFile(name, fileName, ResourceAttributes.Public);
		}

		internal void EmbedResourceFile(string name, string fileName, ResourceAttributes attribute)
		{
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
			try
			{
				FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
				long length = fileStream.Length;
				resources[num].data = new byte[length];
				fileStream.Read(resources[num].data, 0, (int)length);
				fileStream.Close();
			}
			catch
			{
			}
		}

		internal void EmbedResource(string name, byte[] blob, ResourceAttributes attribute)
		{
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
			resources[num].data = blob;
		}

		internal void AddTypeForwarder(Type t)
		{
			if (t == null)
			{
				throw new ArgumentNullException("t");
			}
			if (type_forwarders == null)
			{
				type_forwarders = new Type[1] { t };
				return;
			}
			Type[] array = new Type[type_forwarders.Length + 1];
			Array.Copy(type_forwarders, array, type_forwarders.Length);
			array[type_forwarders.Length] = t;
			type_forwarders = array;
		}

		public ModuleBuilder DefineDynamicModule(string name)
		{
			return DefineDynamicModule(name, name, false, true);
		}

		public ModuleBuilder DefineDynamicModule(string name, bool emitSymbolInfo)
		{
			return DefineDynamicModule(name, name, emitSymbolInfo, true);
		}

		public ModuleBuilder DefineDynamicModule(string name, string fileName)
		{
			return DefineDynamicModule(name, fileName, false, false);
		}

		public ModuleBuilder DefineDynamicModule(string name, string fileName, bool emitSymbolInfo)
		{
			return DefineDynamicModule(name, fileName, emitSymbolInfo, false);
		}

		private ModuleBuilder DefineDynamicModule(string name, string fileName, bool emitSymbolInfo, bool transient)
		{
			check_name_and_filename(name, fileName, false);
			if (!transient)
			{
				if (Path.GetExtension(fileName) == string.Empty)
				{
					throw new ArgumentException("Module file name '" + fileName + "' must have file extension.");
				}
				if (!IsSave)
				{
					throw new NotSupportedException("Persistable modules are not supported in a dynamic assembly created with AssemblyBuilderAccess.Run");
				}
				if (created)
				{
					throw new InvalidOperationException("Assembly was already saved.");
				}
			}
			ModuleBuilder moduleBuilder = new ModuleBuilder(this, name, fileName, emitSymbolInfo, transient);
			if (modules != null && is_module_only)
			{
				throw new InvalidOperationException("A module-only assembly can only contain one module.");
			}
			if (modules != null)
			{
				ModuleBuilder[] destinationArray = new ModuleBuilder[modules.Length + 1];
				Array.Copy(modules, destinationArray, modules.Length);
				modules = destinationArray;
			}
			else
			{
				modules = new ModuleBuilder[1];
			}
			modules[modules.Length - 1] = moduleBuilder;
			return moduleBuilder;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Module InternalAddModule(string fileName);

		internal Module AddModule(string fileName)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException(fileName);
			}
			Module module = InternalAddModule(fileName);
			if (loaded_modules != null)
			{
				Module[] destinationArray = new Module[loaded_modules.Length + 1];
				Array.Copy(loaded_modules, destinationArray, loaded_modules.Length);
				loaded_modules = destinationArray;
			}
			else
			{
				loaded_modules = new Module[1];
			}
			loaded_modules[loaded_modules.Length - 1] = module;
			return module;
		}

		public IResourceWriter DefineResource(string name, string description, string fileName)
		{
			return DefineResource(name, description, fileName, ResourceAttributes.Public);
		}

		public IResourceWriter DefineResource(string name, string description, string fileName, ResourceAttributes attribute)
		{
			AddResourceFile(name, fileName, attribute, false);
			IResourceWriter resourceWriter = new ResourceWriter(fileName);
			if (resource_writers == null)
			{
				resource_writers = new ArrayList();
			}
			resource_writers.Add(resourceWriter);
			return resourceWriter;
		}

		private void AddUnmanagedResource(Win32Resource res)
		{
			MemoryStream memoryStream = new MemoryStream();
			res.WriteTo(memoryStream);
			if (win32_resources != null)
			{
				MonoWin32Resource[] destinationArray = new MonoWin32Resource[win32_resources.Length + 1];
				Array.Copy(win32_resources, destinationArray, win32_resources.Length);
				win32_resources = destinationArray;
			}
			else
			{
				win32_resources = new MonoWin32Resource[1];
			}
			win32_resources[win32_resources.Length - 1] = new MonoWin32Resource(res.Type.Id, res.Name.Id, res.Language, memoryStream.ToArray());
		}

		[MonoTODO("Not currently implemenented")]
		public void DefineUnmanagedResource(byte[] resource)
		{
			if (resource == null)
			{
				throw new ArgumentNullException("resource");
			}
			if (native_resource != NativeResourceType.None)
			{
				throw new ArgumentException("Native resource has already been defined.");
			}
			native_resource = NativeResourceType.Unmanaged;
			throw new NotImplementedException();
		}

		public void DefineUnmanagedResource(string resourceFileName)
		{
			if (resourceFileName == null)
			{
				throw new ArgumentNullException("resourceFileName");
			}
			if (resourceFileName.Length == 0)
			{
				throw new ArgumentException("resourceFileName");
			}
			if (!File.Exists(resourceFileName) || Directory.Exists(resourceFileName))
			{
				throw new FileNotFoundException("File '" + resourceFileName + "' does not exists or is a directory.");
			}
			if (native_resource != NativeResourceType.None)
			{
				throw new ArgumentException("Native resource has already been defined.");
			}
			native_resource = NativeResourceType.Unmanaged;
			using (FileStream s = new FileStream(resourceFileName, FileMode.Open, FileAccess.Read))
			{
				Win32ResFileReader win32ResFileReader = new Win32ResFileReader(s);
				foreach (Win32EncodedResource item in win32ResFileReader.ReadResources())
				{
					if (item.Name.IsName || item.Type.IsName)
					{
						throw new InvalidOperationException("resource files with named resources or non-default resource types are not supported.");
					}
					AddUnmanagedResource(item);
				}
			}
		}

		public void DefineVersionInfoResource()
		{
			if (native_resource != NativeResourceType.None)
			{
				throw new ArgumentException("Native resource has already been defined.");
			}
			native_resource = NativeResourceType.Assembly;
			version_res = new Win32VersionResource(1, 0, IsCompilerContext);
		}

		public void DefineVersionInfoResource(string product, string productVersion, string company, string copyright, string trademark)
		{
			if (native_resource != NativeResourceType.None)
			{
				throw new ArgumentException("Native resource has already been defined.");
			}
			native_resource = NativeResourceType.Explicit;
			version_res = new Win32VersionResource(1, 0, false);
			version_res.ProductName = ((product == null) ? " " : product);
			version_res.ProductVersion = ((productVersion == null) ? " " : productVersion);
			version_res.CompanyName = ((company == null) ? " " : company);
			version_res.LegalCopyright = ((copyright == null) ? " " : copyright);
			version_res.LegalTrademarks = ((trademark == null) ? " " : trademark);
		}

		internal void DefineIconResource(string iconFileName)
		{
			if (iconFileName == null)
			{
				throw new ArgumentNullException("iconFileName");
			}
			if (iconFileName.Length == 0)
			{
				throw new ArgumentException("iconFileName");
			}
			if (!File.Exists(iconFileName) || Directory.Exists(iconFileName))
			{
				throw new FileNotFoundException("File '" + iconFileName + "' does not exists or is a directory.");
			}
			using (FileStream s = new FileStream(iconFileName, FileMode.Open, FileAccess.Read))
			{
				Win32IconFileReader win32IconFileReader = new Win32IconFileReader(s);
				ICONDIRENTRY[] array = win32IconFileReader.ReadIcons();
				Win32IconResource[] array2 = new Win32IconResource[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array2[i] = new Win32IconResource(i + 1, 0, array[i]);
					AddUnmanagedResource(array2[i]);
				}
				Win32GroupIconResource res = new Win32GroupIconResource(1, 0, array2);
				AddUnmanagedResource(res);
			}
		}

		private void DefineVersionInfoResourceImpl(string fileName)
		{
			if (versioninfo_culture != null)
			{
				version_res.FileLanguage = new CultureInfo(versioninfo_culture).LCID;
			}
			version_res.Version = ((version != null) ? version : "0.0.0.0");
			if (cattrs != null)
			{
				switch (native_resource)
				{
				case NativeResourceType.Assembly:
				{
					CustomAttributeBuilder[] array2 = cattrs;
					foreach (CustomAttributeBuilder customAttributeBuilder2 in array2)
					{
						switch (customAttributeBuilder2.Ctor.ReflectedType.FullName)
						{
						case "System.Reflection.AssemblyProductAttribute":
							version_res.ProductName = customAttributeBuilder2.string_arg();
							break;
						case "System.Reflection.AssemblyCompanyAttribute":
							version_res.CompanyName = customAttributeBuilder2.string_arg();
							break;
						case "System.Reflection.AssemblyCopyrightAttribute":
							version_res.LegalCopyright = customAttributeBuilder2.string_arg();
							break;
						case "System.Reflection.AssemblyTrademarkAttribute":
							version_res.LegalTrademarks = customAttributeBuilder2.string_arg();
							break;
						case "System.Reflection.AssemblyCultureAttribute":
							if (!IsCompilerContext)
							{
								version_res.FileLanguage = new CultureInfo(customAttributeBuilder2.string_arg()).LCID;
							}
							break;
						case "System.Reflection.AssemblyFileVersionAttribute":
						{
							string text = customAttributeBuilder2.string_arg();
							if (!IsCompilerContext || (text != null && text.Length != 0))
							{
								version_res.FileVersion = text;
							}
							break;
						}
						case "System.Reflection.AssemblyInformationalVersionAttribute":
							version_res.ProductVersion = customAttributeBuilder2.string_arg();
							break;
						case "System.Reflection.AssemblyTitleAttribute":
							version_res.FileDescription = customAttributeBuilder2.string_arg();
							break;
						case "System.Reflection.AssemblyDescriptionAttribute":
							version_res.Comments = customAttributeBuilder2.string_arg();
							break;
						}
					}
					break;
				}
				case NativeResourceType.Explicit:
				{
					CustomAttributeBuilder[] array = cattrs;
					foreach (CustomAttributeBuilder customAttributeBuilder in array)
					{
						string fullName = customAttributeBuilder.Ctor.ReflectedType.FullName;
						if (fullName == "System.Reflection.AssemblyCultureAttribute")
						{
							if (!IsCompilerContext)
							{
								version_res.FileLanguage = new CultureInfo(customAttributeBuilder.string_arg()).LCID;
							}
						}
						else if (fullName == "System.Reflection.AssemblyDescriptionAttribute")
						{
							version_res.Comments = customAttributeBuilder.string_arg();
						}
					}
					break;
				}
				}
			}
			version_res.OriginalFilename = fileName;
			if (IsCompilerContext)
			{
				version_res.InternalName = fileName;
				if (version_res.ProductVersion.Trim().Length == 0)
				{
					version_res.ProductVersion = version_res.FileVersion;
				}
			}
			else
			{
				version_res.InternalName = Path.GetFileNameWithoutExtension(fileName);
			}
			AddUnmanagedResource(version_res);
		}

		public ModuleBuilder GetDynamicModule(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException("Empty name is not legal.", "name");
			}
			if (modules != null)
			{
				for (int i = 0; i < modules.Length; i++)
				{
					if (modules[i].name == name)
					{
						return modules[i];
					}
				}
			}
			return null;
		}

		public override Type[] GetExportedTypes()
		{
			throw not_supported();
		}

		public override FileStream GetFile(string name)
		{
			throw not_supported();
		}

		public override FileStream[] GetFiles(bool getResourceModules)
		{
			throw not_supported();
		}

		internal override Module[] GetModulesInternal()
		{
			if (modules == null)
			{
				return new Module[0];
			}
			return (Module[])modules.Clone();
		}

		internal override Type[] GetTypes(bool exportedOnly)
		{
			Type[] array = null;
			if (modules != null)
			{
				for (int i = 0; i < modules.Length; i++)
				{
					Type[] types = modules[i].GetTypes();
					if (array == null)
					{
						array = types;
						continue;
					}
					Type[] destinationArray = new Type[array.Length + types.Length];
					Array.Copy(array, 0, destinationArray, 0, array.Length);
					Array.Copy(types, 0, destinationArray, array.Length, types.Length);
				}
			}
			if (loaded_modules != null)
			{
				for (int j = 0; j < loaded_modules.Length; j++)
				{
					Type[] types2 = loaded_modules[j].GetTypes();
					if (array == null)
					{
						array = types2;
						continue;
					}
					Type[] destinationArray2 = new Type[array.Length + types2.Length];
					Array.Copy(array, 0, destinationArray2, 0, array.Length);
					Array.Copy(types2, 0, destinationArray2, array.Length, types2.Length);
				}
			}
			return (array != null) ? array : Type.EmptyTypes;
		}

		public override ManifestResourceInfo GetManifestResourceInfo(string resourceName)
		{
			throw not_supported();
		}

		public override string[] GetManifestResourceNames()
		{
			throw not_supported();
		}

		public override Stream GetManifestResourceStream(string name)
		{
			throw not_supported();
		}

		public override Stream GetManifestResourceStream(Type type, string name)
		{
			throw not_supported();
		}

		internal override Module GetManifestModule()
		{
			if (manifest_module == null)
			{
				manifest_module = DefineDynamicModule("Default Dynamic Module");
			}
			return manifest_module;
		}

		[MonoLimitation("No support for PE32+ assemblies for AMD64 and IA64")]
		public void Save(string assemblyFileName, PortableExecutableKinds portableExecutableKind, ImageFileMachine imageFileMachine)
		{
			peKind = portableExecutableKind;
			machine = imageFileMachine;
			if ((peKind & PortableExecutableKinds.PE32Plus) != PortableExecutableKinds.NotAPortableExecutableImage || (peKind & PortableExecutableKinds.Unmanaged32Bit) != PortableExecutableKinds.NotAPortableExecutableImage)
			{
				throw new NotImplementedException(peKind.ToString());
			}
			if (machine == ImageFileMachine.IA64 || machine == ImageFileMachine.AMD64)
			{
				throw new NotImplementedException(machine.ToString());
			}
			if (resource_writers != null)
			{
				foreach (IResourceWriter resource_writer in resource_writers)
				{
					resource_writer.Generate();
					resource_writer.Close();
				}
			}
			ModuleBuilder moduleBuilder = null;
			if (modules != null)
			{
				ModuleBuilder[] array = modules;
				foreach (ModuleBuilder moduleBuilder2 in array)
				{
					if (moduleBuilder2.FullyQualifiedName == assemblyFileName)
					{
						moduleBuilder = moduleBuilder2;
					}
				}
			}
			if (moduleBuilder == null)
			{
				moduleBuilder = DefineDynamicModule("RefEmit_OnDiskManifestModule", assemblyFileName);
			}
			if (!is_module_only)
			{
				moduleBuilder.IsMain = true;
			}
			if (entry_point != null && entry_point.DeclaringType.Module != moduleBuilder)
			{
				Type[] array2 = ((entry_point.GetParameters().Length != 1) ? Type.EmptyTypes : new Type[1] { typeof(string) });
				MethodBuilder methodBuilder = moduleBuilder.DefineGlobalMethod("__EntryPoint$", MethodAttributes.Static, entry_point.ReturnType, array2);
				ILGenerator iLGenerator = methodBuilder.GetILGenerator();
				if (array2.Length == 1)
				{
					iLGenerator.Emit(OpCodes.Ldarg_0);
				}
				iLGenerator.Emit(OpCodes.Tailcall);
				iLGenerator.Emit(OpCodes.Call, entry_point);
				iLGenerator.Emit(OpCodes.Ret);
				entry_point = methodBuilder;
			}
			if (version_res != null)
			{
				DefineVersionInfoResourceImpl(assemblyFileName);
			}
			if (sn != null)
			{
				public_key = sn.PublicKey;
			}
			ModuleBuilder[] array3 = modules;
			foreach (ModuleBuilder moduleBuilder3 in array3)
			{
				if (moduleBuilder3 != moduleBuilder)
				{
					moduleBuilder3.Save();
				}
			}
			moduleBuilder.Save();
			if (sn != null && sn.CanSign)
			{
				sn.Sign(Path.Combine(AssemblyDir, assemblyFileName));
			}
			created = true;
		}

		public void Save(string assemblyFileName)
		{
			Save(assemblyFileName, PortableExecutableKinds.ILOnly, ImageFileMachine.I386);
		}

		public void SetEntryPoint(MethodInfo entryMethod)
		{
			SetEntryPoint(entryMethod, PEFileKinds.ConsoleApplication);
		}

		public void SetEntryPoint(MethodInfo entryMethod, PEFileKinds fileKind)
		{
			if (entryMethod == null)
			{
				throw new ArgumentNullException("entryMethod");
			}
			if (entryMethod.DeclaringType.Assembly != this)
			{
				throw new InvalidOperationException("Entry method is not defined in the same assembly.");
			}
			entry_point = entryMethod;
			pekind = fileKind;
		}

		public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
		{
			if (customBuilder == null)
			{
				throw new ArgumentNullException("customBuilder");
			}
			if (IsCompilerContext)
			{
				switch (customBuilder.Ctor.ReflectedType.FullName)
				{
				case "System.Reflection.AssemblyVersionAttribute":
					version = create_assembly_version(customBuilder.string_arg());
					return;
				case "System.Reflection.AssemblyCultureAttribute":
					culture = GetCultureString(customBuilder.string_arg());
					break;
				case "System.Reflection.AssemblyAlgorithmIdAttribute":
				{
					byte[] data = customBuilder.Data;
					int num = 2;
					algid = data[num];
					algid |= (uint)(data[num + 1] << 8);
					algid |= (uint)(data[num + 2] << 16);
					algid |= (uint)(data[num + 3] << 24);
					break;
				}
				case "System.Reflection.AssemblyFlagsAttribute":
				{
					byte[] data = customBuilder.Data;
					int num = 2;
					flags |= data[num];
					flags |= (uint)(data[num + 1] << 8);
					flags |= (uint)(data[num + 2] << 16);
					flags |= (uint)(data[num + 3] << 24);
					if (sn == null)
					{
						flags &= 4294967294u;
					}
					break;
				}
				}
			}
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
			if (con == null)
			{
				throw new ArgumentNullException("con");
			}
			if (binaryAttribute == null)
			{
				throw new ArgumentNullException("binaryAttribute");
			}
			SetCustomAttribute(new CustomAttributeBuilder(con, binaryAttribute));
		}

		internal void SetCorlibTypeBuilders(Type corlib_object_type, Type corlib_value_type, Type corlib_enum_type)
		{
			this.corlib_object_type = corlib_object_type;
			this.corlib_value_type = corlib_value_type;
			this.corlib_enum_type = corlib_enum_type;
		}

		internal void SetCorlibTypeBuilders(Type corlib_object_type, Type corlib_value_type, Type corlib_enum_type, Type corlib_void_type)
		{
			SetCorlibTypeBuilders(corlib_object_type, corlib_value_type, corlib_enum_type);
			this.corlib_void_type = corlib_void_type;
		}

		private Exception not_supported()
		{
			return new NotSupportedException("The invoked member is not supported in a dynamic module.");
		}

		private void check_name_and_filename(string name, string fileName, bool fileNeedsToExists)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException("Empty name is not legal.", "name");
			}
			if (fileName.Length == 0)
			{
				throw new ArgumentException("Empty file name is not legal.", "fileName");
			}
			if (Path.GetFileName(fileName) != fileName)
			{
				throw new ArgumentException("fileName '" + fileName + "' must not include a path.", "fileName");
			}
			string text = fileName;
			if (dir != null)
			{
				text = Path.Combine(dir, fileName);
			}
			if (fileNeedsToExists && !File.Exists(text))
			{
				throw new FileNotFoundException("Could not find file '" + fileName + "'");
			}
			if (resources != null)
			{
				for (int i = 0; i < resources.Length; i++)
				{
					if (resources[i].filename == text)
					{
						throw new ArgumentException("Duplicate file name '" + fileName + "'");
					}
					if (resources[i].name == name)
					{
						throw new ArgumentException("Duplicate name '" + name + "'");
					}
				}
			}
			if (modules == null)
			{
				return;
			}
			for (int j = 0; j < modules.Length; j++)
			{
				if (!modules[j].IsTransient() && modules[j].FileName == fileName)
				{
					throw new ArgumentException("Duplicate file name '" + fileName + "'");
				}
				if (modules[j].Name == name)
				{
					throw new ArgumentException("Duplicate name '" + name + "'");
				}
			}
		}

		private string create_assembly_version(string version)
		{
			string[] array = version.Split('.');
			int[] array2 = new int[4];
			if (array.Length < 0 || array.Length > 4)
			{
				throw new ArgumentException("The version specified '" + version + "' is invalid");
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == "*")
				{
					DateTime now = DateTime.Now;
					switch (i)
					{
					case 2:
						array2[2] = (now - new DateTime(2000, 1, 1)).Days;
						if (array.Length == 3)
						{
							array2[3] = (now.Second + now.Minute * 60 + now.Hour * 3600) / 2;
						}
						break;
					case 3:
						array2[3] = (now.Second + now.Minute * 60 + now.Hour * 3600) / 2;
						break;
					default:
						throw new ArgumentException("The version specified '" + version + "' is invalid");
					}
				}
				else
				{
					try
					{
						array2[i] = int.Parse(array[i]);
					}
					catch (FormatException)
					{
						throw new ArgumentException("The version specified '" + version + "' is invalid");
					}
				}
			}
			return array2[0] + "." + array2[1] + "." + array2[2] + "." + array2[3];
		}

		private string GetCultureString(string str)
		{
			return (!(str == "neutral")) ? str : string.Empty;
		}

		internal override AssemblyName UnprotectedGetName()
		{
			AssemblyName assemblyName = base.UnprotectedGetName();
			if (sn != null)
			{
				assemblyName.SetPublicKey(sn.PublicKey);
				assemblyName.SetPublicKeyToken(sn.PublicKeyToken);
			}
			return assemblyName;
		}
	}
}
