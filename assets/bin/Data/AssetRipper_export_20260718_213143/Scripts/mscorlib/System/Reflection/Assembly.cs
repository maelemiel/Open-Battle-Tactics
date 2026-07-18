using System.Collections;
using System.Configuration.Assemblies;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Policy;

namespace System.Reflection
{
	[Serializable]
	[ClassInterface(ClassInterfaceType.None)]
	[ComDefaultInterface(typeof(_Assembly))]
	[ComVisible(true)]
	public class Assembly : ICustomAttributeProvider, _Assembly
	{
		internal class ResolveEventHolder
		{
			public event ModuleResolveEventHandler ModuleResolve;
		}

		private class ResourceCloseHandler
		{
			private Module module;

			public ResourceCloseHandler(Module module)
			{
				this.module = module;
			}

			public void OnClose(object sender, EventArgs e)
			{
				module = null;
			}
		}

		private IntPtr _mono_assembly;

		private ResolveEventHolder resolve_event_holder;

		private Evidence _evidence;

		internal PermissionSet _minimum;

		internal PermissionSet _optional;

		internal PermissionSet _refuse;

		private PermissionSet _granted;

		private PermissionSet _denied;

		private bool fromByteArray;

		private string assemblyName;

		public virtual string CodeBase
		{
			get
			{
				return GetCodeBase(false);
			}
		}

		public virtual string EscapedCodeBase
		{
			get
			{
				return GetCodeBase(true);
			}
		}

		public virtual string FullName
		{
			get
			{
				return ToString();
			}
		}

		public virtual extern MethodInfo EntryPoint
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public virtual Evidence Evidence
		{
			get
			{
				return UnprotectedGetEvidence();
			}
		}

		public bool GlobalAssemblyCache
		{
			get
			{
				return get_global_assembly_cache();
			}
		}

		internal bool FromByteArray
		{
			set
			{
				fromByteArray = value;
			}
		}

		public virtual string Location
		{
			get
			{
				if (fromByteArray)
				{
					return string.Empty;
				}
				return get_location();
			}
		}

		[ComVisible(false)]
		public virtual string ImageRuntimeVersion
		{
			get
			{
				return InternalImageRuntimeVersion();
			}
		}

		[ComVisible(false)]
		[MonoTODO("Always returns zero")]
		public long HostContext
		{
			get
			{
				return 0L;
			}
		}

		[ComVisible(false)]
		public Module ManifestModule
		{
			get
			{
				return GetManifestModule();
			}
		}

		[ComVisible(false)]
		public virtual extern bool ReflectionOnly
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal PermissionSet GrantedPermissionSet
		{
			get
			{
				if (_granted == null)
				{
					if (SecurityManager.ResolvingPolicyLevel != null)
					{
						if (SecurityManager.ResolvingPolicyLevel.IsFullTrustAssembly(this))
						{
							return DefaultPolicies.FullTrust;
						}
						return null;
					}
					Resolve();
				}
				return _granted;
			}
		}

		internal PermissionSet DeniedPermissionSet
		{
			get
			{
				if (_granted == null)
				{
					if (SecurityManager.ResolvingPolicyLevel != null)
					{
						if (SecurityManager.ResolvingPolicyLevel.IsFullTrustAssembly(this))
						{
							return null;
						}
						return DefaultPolicies.FullTrust;
					}
					Resolve();
				}
				return _denied;
			}
		}

		public event ModuleResolveEventHandler ModuleResolve
		{
			add
			{
				resolve_event_holder.ModuleResolve += value;
			}
			remove
			{
				resolve_event_holder.ModuleResolve -= value;
			}
		}

		internal Assembly()
		{
			resolve_event_holder = new ResolveEventHolder();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string get_code_base(bool escaped);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string get_fullname();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string get_location();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string InternalImageRuntimeVersion();

		private string GetCodeBase(bool escaped)
		{
			return get_code_base(escaped);
		}

		internal Evidence UnprotectedGetEvidence()
		{
			if (_evidence == null)
			{
				lock (this)
				{
					_evidence = Evidence.GetDefaultHostEvidence(this);
				}
			}
			return _evidence;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool get_global_assembly_cache();

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			UnitySerializationHolder.GetAssemblyData(this, info, context);
		}

		public virtual bool IsDefined(Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.IsDefined(this, attributeType, inherit);
		}

		public virtual object[] GetCustomAttributes(bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, inherit);
		}

		public virtual object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, attributeType, inherit);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern object GetFilesInternal(string name, bool getResourceModules);

		public virtual FileStream[] GetFiles()
		{
			return GetFiles(false);
		}

		public virtual FileStream[] GetFiles(bool getResourceModules)
		{
			string[] array = (string[])GetFilesInternal(null, getResourceModules);
			if (array == null)
			{
				return new FileStream[0];
			}
			string location = Location;
			FileStream[] array2;
			if (location != string.Empty)
			{
				array2 = new FileStream[array.Length + 1];
				array2[0] = new FileStream(location, FileMode.Open, FileAccess.Read);
				for (int i = 0; i < array.Length; i++)
				{
					array2[i + 1] = new FileStream(array[i], FileMode.Open, FileAccess.Read);
				}
			}
			else
			{
				array2 = new FileStream[array.Length];
				for (int j = 0; j < array.Length; j++)
				{
					array2[j] = new FileStream(array[j], FileMode.Open, FileAccess.Read);
				}
			}
			return array2;
		}

		public virtual FileStream GetFile(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(null, "Name cannot be null.");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException("Empty name is not valid");
			}
			string text = (string)GetFilesInternal(name, true);
			if (text != null)
			{
				return new FileStream(text, FileMode.Open, FileAccess.Read);
			}
			return null;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern IntPtr GetManifestResourceInternal(string name, out int size, out Module module);

		public unsafe virtual Stream GetManifestResourceStream(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException("String cannot have zero length.", "name");
			}
			ManifestResourceInfo manifestResourceInfo = GetManifestResourceInfo(name);
			if (manifestResourceInfo == null)
			{
				return null;
			}
			if (manifestResourceInfo.ReferencedAssembly != null)
			{
				return manifestResourceInfo.ReferencedAssembly.GetManifestResourceStream(name);
			}
			if (manifestResourceInfo.FileName != null && manifestResourceInfo.ResourceLocation == (ResourceLocation)0)
			{
				if (fromByteArray)
				{
					throw new FileNotFoundException(manifestResourceInfo.FileName);
				}
				string directoryName = Path.GetDirectoryName(Location);
				string path = Path.Combine(directoryName, manifestResourceInfo.FileName);
				return new FileStream(path, FileMode.Open, FileAccess.Read);
			}
			int size;
			Module module;
			IntPtr manifestResourceInternal = GetManifestResourceInternal(name, out size, out module);
			if (manifestResourceInternal == (IntPtr)0)
			{
				return null;
			}
			UnmanagedMemoryStream unmanagedMemoryStream = new UnmanagedMemoryStream((byte*)(void*)manifestResourceInternal, size);
			unmanagedMemoryStream.Closed += new ResourceCloseHandler(module).OnClose;
			return unmanagedMemoryStream;
		}

		public virtual Stream GetManifestResourceStream(Type type, string name)
		{
			string text;
			if (type != null)
			{
				text = type.Namespace;
			}
			else
			{
				if (name == null)
				{
					throw new ArgumentNullException("type");
				}
				text = null;
			}
			if (text == null || text.Length == 0)
			{
				return GetManifestResourceStream(name);
			}
			return GetManifestResourceStream(text + "." + name);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal virtual extern Type[] GetTypes(bool exportedOnly);

		public virtual Type[] GetTypes()
		{
			return GetTypes(false);
		}

		public virtual Type[] GetExportedTypes()
		{
			return GetTypes(true);
		}

		public virtual Type GetType(string name, bool throwOnError)
		{
			return GetType(name, throwOnError, false);
		}

		public virtual Type GetType(string name)
		{
			return GetType(name, false, false);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Type InternalGetType(Module module, string name, bool throwOnError, bool ignoreCase);

		public Type GetType(string name, bool throwOnError, bool ignoreCase)
		{
			if (name == null)
			{
				throw new ArgumentNullException(name);
			}
			if (name.Length == 0)
			{
				throw new ArgumentException("name", "Name cannot be empty");
			}
			return InternalGetType(null, name, throwOnError, ignoreCase);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalGetAssemblyName(string assemblyFile, AssemblyName aname);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FillName(Assembly ass, AssemblyName aname);

		[MonoTODO("copiedName == true is not supported")]
		public virtual AssemblyName GetName(bool copiedName)
		{
			if (SecurityManager.SecurityEnabled)
			{
				GetCodeBase(true);
			}
			return UnprotectedGetName();
		}

		public virtual AssemblyName GetName()
		{
			return GetName(false);
		}

		internal virtual AssemblyName UnprotectedGetName()
		{
			AssemblyName assemblyName = new AssemblyName();
			FillName(this, assemblyName);
			return assemblyName;
		}

		public override string ToString()
		{
			if (assemblyName != null)
			{
				return assemblyName;
			}
			assemblyName = get_fullname();
			return assemblyName;
		}

		public static string CreateQualifiedName(string assemblyName, string typeName)
		{
			return typeName + ", " + assemblyName;
		}

		public static Assembly GetAssembly(Type type)
		{
			if (type != null)
			{
				return type.Assembly;
			}
			throw new ArgumentNullException("type");
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Assembly GetEntryAssembly();

		public Assembly GetSatelliteAssembly(CultureInfo culture)
		{
			return GetSatelliteAssembly(culture, null, true);
		}

		public Assembly GetSatelliteAssembly(CultureInfo culture, Version version)
		{
			return GetSatelliteAssembly(culture, version, true);
		}

		internal Assembly GetSatelliteAssemblyNoThrow(CultureInfo culture, Version version)
		{
			return GetSatelliteAssembly(culture, version, false);
		}

		private Assembly GetSatelliteAssembly(CultureInfo culture, Version version, bool throwOnError)
		{
			if (culture == null)
			{
				throw new ArgumentException("culture");
			}
			AssemblyName name = GetName(true);
			if (version != null)
			{
				name.Version = version;
			}
			name.CultureInfo = culture;
			name.Name += ".resources";
			try
			{
				Assembly assembly = AppDomain.CurrentDomain.LoadSatellite(name, false);
				if (assembly != null)
				{
					return assembly;
				}
			}
			catch (FileNotFoundException)
			{
				Assembly assembly = null;
			}
			string directoryName = Path.GetDirectoryName(Location);
			string text = Path.Combine(directoryName, Path.Combine(culture.Name, name.Name + ".dll"));
			if (!throwOnError && !File.Exists(text))
			{
				return null;
			}
			return LoadFrom(text);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Assembly LoadFrom(string assemblyFile, bool refonly);

		public static Assembly LoadFrom(string assemblyFile)
		{
			return LoadFrom(assemblyFile, false);
		}

		public static Assembly LoadFrom(string assemblyFile, Evidence securityEvidence)
		{
			return LoadFrom(assemblyFile, false);
		}

		[MonoTODO("This overload is not currently implemented")]
		public static Assembly LoadFrom(string assemblyFile, Evidence securityEvidence, byte[] hashValue, AssemblyHashAlgorithm hashAlgorithm)
		{
			if (assemblyFile == null)
			{
				throw new ArgumentNullException("assemblyFile");
			}
			if (assemblyFile == string.Empty)
			{
				throw new ArgumentException("Name can't be the empty string", "assemblyFile");
			}
			throw new NotImplementedException();
		}

		public static Assembly LoadFile(string path, Evidence securityEvidence)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path == string.Empty)
			{
				throw new ArgumentException("Path can't be empty", "path");
			}
			return LoadFrom(path, securityEvidence);
		}

		public static Assembly LoadFile(string path)
		{
			return LoadFile(path, null);
		}

		public static Assembly Load(string assemblyString)
		{
			return AppDomain.CurrentDomain.Load(assemblyString);
		}

		public static Assembly Load(string assemblyString, Evidence assemblySecurity)
		{
			return AppDomain.CurrentDomain.Load(assemblyString, assemblySecurity);
		}

		public static Assembly Load(AssemblyName assemblyRef)
		{
			return AppDomain.CurrentDomain.Load(assemblyRef);
		}

		public static Assembly Load(AssemblyName assemblyRef, Evidence assemblySecurity)
		{
			return AppDomain.CurrentDomain.Load(assemblyRef, assemblySecurity);
		}

		public static Assembly Load(byte[] rawAssembly)
		{
			return AppDomain.CurrentDomain.Load(rawAssembly);
		}

		public static Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore)
		{
			return AppDomain.CurrentDomain.Load(rawAssembly, rawSymbolStore);
		}

		public static Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore, Evidence securityEvidence)
		{
			return AppDomain.CurrentDomain.Load(rawAssembly, rawSymbolStore, securityEvidence);
		}

		public static Assembly ReflectionOnlyLoad(byte[] rawAssembly)
		{
			return AppDomain.CurrentDomain.Load(rawAssembly, null, null, true);
		}

		public static Assembly ReflectionOnlyLoad(string assemblyString)
		{
			return AppDomain.CurrentDomain.Load(assemblyString, null, true);
		}

		public static Assembly ReflectionOnlyLoadFrom(string assemblyFile)
		{
			if (assemblyFile == null)
			{
				throw new ArgumentNullException("assemblyFile");
			}
			return LoadFrom(assemblyFile, true);
		}

		[Obsolete("")]
		public static Assembly LoadWithPartialName(string partialName)
		{
			return LoadWithPartialName(partialName, null);
		}

		[MonoTODO("Not implemented")]
		public Module LoadModule(string moduleName, byte[] rawModule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Not implemented")]
		public Module LoadModule(string moduleName, byte[] rawModule, byte[] rawSymbolStore)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Assembly load_with_partial_name(string name, Evidence e);

		[Obsolete("")]
		public static Assembly LoadWithPartialName(string partialName, Evidence securityEvidence)
		{
			return LoadWithPartialName(partialName, securityEvidence, true);
		}

		internal static Assembly LoadWithPartialName(string partialName, Evidence securityEvidence, bool oldBehavior)
		{
			if (!oldBehavior)
			{
				throw new NotImplementedException();
			}
			if (partialName == null)
			{
				throw new NullReferenceException();
			}
			return load_with_partial_name(partialName, securityEvidence);
		}

		public object CreateInstance(string typeName)
		{
			return CreateInstance(typeName, false);
		}

		public object CreateInstance(string typeName, bool ignoreCase)
		{
			Type type = GetType(typeName, false, ignoreCase);
			if (type == null)
			{
				return null;
			}
			try
			{
				return Activator.CreateInstance(type);
			}
			catch (InvalidOperationException)
			{
				throw new ArgumentException("It is illegal to invoke a method on a Type loaded via ReflectionOnly methods.");
			}
		}

		public object CreateInstance(string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes)
		{
			Type type = GetType(typeName, false, ignoreCase);
			if (type == null)
			{
				return null;
			}
			try
			{
				return Activator.CreateInstance(type, bindingAttr, binder, args, culture, activationAttributes);
			}
			catch (InvalidOperationException)
			{
				throw new ArgumentException("It is illegal to invoke a method on a Type loaded via ReflectionOnly methods.");
			}
		}

		public Module[] GetLoadedModules()
		{
			return GetLoadedModules(false);
		}

		public Module[] GetLoadedModules(bool getResourceModules)
		{
			return GetModules(getResourceModules);
		}

		public Module[] GetModules()
		{
			return GetModules(false);
		}

		public Module GetModule(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException("Name can't be empty");
			}
			Module[] modules = GetModules(true);
			Module[] array = modules;
			foreach (Module module in array)
			{
				if (module.ScopeName == name)
				{
					return module;
				}
			}
			return null;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal virtual extern Module[] GetModulesInternal();

		public Module[] GetModules(bool getResourceModules)
		{
			Module[] modulesInternal = GetModulesInternal();
			if (!getResourceModules)
			{
				ArrayList arrayList = new ArrayList(modulesInternal.Length);
				Module[] array = modulesInternal;
				foreach (Module module in array)
				{
					if (!module.IsResource())
					{
						arrayList.Add(module);
					}
				}
				return (Module[])arrayList.ToArray(typeof(Module));
			}
			return modulesInternal;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string[] GetNamespaces();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern string[] GetManifestResourceNames();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Assembly GetExecutingAssembly();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Assembly GetCallingAssembly();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AssemblyName[] GetReferencedAssemblies();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool GetManifestResourceInfoInternal(string name, ManifestResourceInfo info);

		public virtual ManifestResourceInfo GetManifestResourceInfo(string resourceName)
		{
			if (resourceName == null)
			{
				throw new ArgumentNullException("resourceName");
			}
			if (resourceName.Length == 0)
			{
				throw new ArgumentException("String cannot have zero length.");
			}
			ManifestResourceInfo manifestResourceInfo = new ManifestResourceInfo();
			if (GetManifestResourceInfoInternal(resourceName, manifestResourceInfo))
			{
				return manifestResourceInfo;
			}
			return null;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int MonoDebugger_GetMethodToken(MethodBase method);

		internal virtual Module GetManifestModule()
		{
			return GetManifestModuleInternal();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Module GetManifestModuleInternal();

		internal void Resolve()
		{
			lock (this)
			{
				LoadAssemblyPermissions();
				Evidence evidence = new Evidence(UnprotectedGetEvidence());
				evidence.AddHost(new PermissionRequestEvidence(_minimum, _optional, _refuse));
				_granted = SecurityManager.ResolvePolicy(evidence, _minimum, _optional, _refuse, out _denied);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool LoadPermissions(Assembly a, ref IntPtr minimum, ref int minLength, ref IntPtr optional, ref int optLength, ref IntPtr refused, ref int refLength);

		private void LoadAssemblyPermissions()
		{
			IntPtr minimum = IntPtr.Zero;
			IntPtr optional = IntPtr.Zero;
			IntPtr refused = IntPtr.Zero;
			int minLength = 0;
			int optLength = 0;
			int refLength = 0;
			if (LoadPermissions(this, ref minimum, ref minLength, ref optional, ref optLength, ref refused, ref refLength))
			{
				if (minLength > 0)
				{
					byte[] array = new byte[minLength];
					Marshal.Copy(minimum, array, 0, minLength);
					_minimum = SecurityManager.Decode(array);
				}
				if (optLength > 0)
				{
					byte[] array2 = new byte[optLength];
					Marshal.Copy(optional, array2, 0, optLength);
					_optional = SecurityManager.Decode(array2);
				}
				if (refLength > 0)
				{
					byte[] array3 = new byte[refLength];
					Marshal.Copy(refused, array3, 0, refLength);
					_refuse = SecurityManager.Decode(array3);
				}
			}
		}

		virtual Type _Assembly.GetType()
		{
			return GetType();
		}
	}
}
