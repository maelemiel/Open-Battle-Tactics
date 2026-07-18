using System.Collections;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Security.Principal;
using System.Threading;
using Mono.Security;

namespace System
{
	[ClassInterface(ClassInterfaceType.None)]
	[ComVisible(true)]
	public sealed class AppDomain : MarshalByRefObject
	{
		private IntPtr _mono_app_domain;

		private static string _process_guid;

		[ThreadStatic]
		private static Hashtable type_resolve_in_progress;

		[ThreadStatic]
		private static Hashtable assembly_resolve_in_progress;

		[ThreadStatic]
		private static Hashtable assembly_resolve_in_progress_refonly;

		private Evidence _evidence;

		private PermissionSet _granted;

		private PrincipalPolicy _principalPolicy;

		[ThreadStatic]
		private static IPrincipal _principal;

		private static AppDomain default_domain;

		private AppDomainManager _domain_manager;

		private ActivationContext _activation;

		private ApplicationIdentity _applicationIdentity;

		internal AppDomainSetup SetupInformationNoCopy
		{
			get
			{
				return getSetup();
			}
		}

		public AppDomainSetup SetupInformation
		{
			get
			{
				AppDomainSetup setup = getSetup();
				return new AppDomainSetup(setup);
			}
		}

		public string BaseDirectory
		{
			get
			{
				string applicationBase = SetupInformationNoCopy.ApplicationBase;
				if (SecurityManager.SecurityEnabled && applicationBase != null && applicationBase.Length > 0)
				{
					new FileIOPermission(FileIOPermissionAccess.PathDiscovery, applicationBase).Demand();
				}
				return applicationBase;
			}
		}

		public string RelativeSearchPath
		{
			get
			{
				string privateBinPath = SetupInformationNoCopy.PrivateBinPath;
				if (SecurityManager.SecurityEnabled && privateBinPath != null && privateBinPath.Length > 0)
				{
					new FileIOPermission(FileIOPermissionAccess.PathDiscovery, privateBinPath).Demand();
				}
				return privateBinPath;
			}
		}

		public string DynamicDirectory
		{
			get
			{
				AppDomainSetup setupInformationNoCopy = SetupInformationNoCopy;
				if (setupInformationNoCopy.DynamicBase == null)
				{
					return null;
				}
				string text = Path.Combine(setupInformationNoCopy.DynamicBase, setupInformationNoCopy.ApplicationName);
				if (SecurityManager.SecurityEnabled && text != null && text.Length > 0)
				{
					new FileIOPermission(FileIOPermissionAccess.PathDiscovery, text).Demand();
				}
				return text;
			}
		}

		public bool ShadowCopyFiles
		{
			get
			{
				return SetupInformationNoCopy.ShadowCopyFiles == "true";
			}
		}

		public string FriendlyName
		{
			get
			{
				return getFriendlyName();
			}
		}

		public Evidence Evidence
		{
			get
			{
				if (_evidence == null)
				{
					lock (this)
					{
						Assembly entryAssembly = Assembly.GetEntryAssembly();
						if (entryAssembly == null)
						{
							if (this == DefaultDomain)
							{
								return new Evidence();
							}
							_evidence = DefaultDomain.Evidence;
						}
						else
						{
							_evidence = Evidence.GetDefaultHostEvidence(entryAssembly);
						}
					}
				}
				return new Evidence(_evidence);
			}
		}

		internal IPrincipal DefaultPrincipal
		{
			get
			{
				if (_principal == null)
				{
					switch (_principalPolicy)
					{
					case PrincipalPolicy.UnauthenticatedPrincipal:
						_principal = new GenericPrincipal(new GenericIdentity(string.Empty, string.Empty), null);
						break;
					case PrincipalPolicy.WindowsPrincipal:
						_principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
						break;
					}
				}
				return _principal;
			}
		}

		internal PermissionSet GrantedPermissionSet
		{
			get
			{
				return _granted;
			}
		}

		public static AppDomain CurrentDomain
		{
			get
			{
				return getCurDomain();
			}
		}

		internal static AppDomain DefaultDomain
		{
			get
			{
				if (default_domain == null)
				{
					AppDomain rootDomain = getRootDomain();
					if (rootDomain == CurrentDomain)
					{
						default_domain = rootDomain;
					}
					else
					{
						default_domain = (AppDomain)RemotingServices.GetDomainProxy(rootDomain);
					}
				}
				return default_domain;
			}
		}

		public AppDomainManager DomainManager
		{
			get
			{
				return _domain_manager;
			}
		}

		public ActivationContext ActivationContext
		{
			get
			{
				return _activation;
			}
		}

		public ApplicationIdentity ApplicationIdentity
		{
			get
			{
				return _applicationIdentity;
			}
		}

		public int Id
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				return getDomainID();
			}
		}

		public event AssemblyLoadEventHandler AssemblyLoad;

		public event ResolveEventHandler AssemblyResolve;

		public event EventHandler DomainUnload;

		public event EventHandler ProcessExit;

		public event ResolveEventHandler ResourceResolve;

		public event ResolveEventHandler TypeResolve;

		public event UnhandledExceptionEventHandler UnhandledException;

		public event ResolveEventHandler ReflectionOnlyAssemblyResolve;

		private AppDomain()
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern AppDomainSetup getSetup();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string getFriendlyName();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AppDomain getCurDomain();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AppDomain getRootDomain();

		[Obsolete("AppDomain.AppendPrivatePath has been deprecated. Please investigate the use of AppDomainSetup.PrivateBinPath instead.")]
		public void AppendPrivatePath(string path)
		{
			if (path == null || path.Length == 0)
			{
				return;
			}
			AppDomainSetup setupInformationNoCopy = SetupInformationNoCopy;
			string privateBinPath = setupInformationNoCopy.PrivateBinPath;
			if (privateBinPath == null || privateBinPath.Length == 0)
			{
				setupInformationNoCopy.PrivateBinPath = path;
				return;
			}
			privateBinPath = privateBinPath.Trim();
			if (privateBinPath[privateBinPath.Length - 1] != Path.PathSeparator)
			{
				privateBinPath += Path.PathSeparator;
			}
			setupInformationNoCopy.PrivateBinPath = privateBinPath + path;
		}

		[Obsolete("AppDomain.ClearPrivatePath has been deprecated. Please investigate the use of AppDomainSetup.PrivateBinPath instead.")]
		public void ClearPrivatePath()
		{
			SetupInformationNoCopy.PrivateBinPath = string.Empty;
		}

		[Obsolete("Use AppDomainSetup.ShadowCopyDirectories")]
		public void ClearShadowCopyPath()
		{
			SetupInformationNoCopy.ShadowCopyDirectories = string.Empty;
		}

		public ObjectHandle CreateInstance(string assemblyName, string typeName)
		{
			if (assemblyName == null)
			{
				throw new ArgumentNullException("assemblyName");
			}
			return Activator.CreateInstance(assemblyName, typeName);
		}

		public ObjectHandle CreateInstance(string assemblyName, string typeName, object[] activationAttributes)
		{
			if (assemblyName == null)
			{
				throw new ArgumentNullException("assemblyName");
			}
			return Activator.CreateInstance(assemblyName, typeName, activationAttributes);
		}

		public ObjectHandle CreateInstance(string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, Evidence securityAttributes)
		{
			if (assemblyName == null)
			{
				throw new ArgumentNullException("assemblyName");
			}
			return Activator.CreateInstance(assemblyName, typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes, securityAttributes);
		}

		public object CreateInstanceAndUnwrap(string assemblyName, string typeName)
		{
			ObjectHandle objectHandle = CreateInstance(assemblyName, typeName);
			return (objectHandle == null) ? null : objectHandle.Unwrap();
		}

		public object CreateInstanceAndUnwrap(string assemblyName, string typeName, object[] activationAttributes)
		{
			ObjectHandle objectHandle = CreateInstance(assemblyName, typeName, activationAttributes);
			return (objectHandle == null) ? null : objectHandle.Unwrap();
		}

		public object CreateInstanceAndUnwrap(string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, Evidence securityAttributes)
		{
			ObjectHandle objectHandle = CreateInstance(assemblyName, typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes, securityAttributes);
			return (objectHandle == null) ? null : objectHandle.Unwrap();
		}

		public ObjectHandle CreateInstanceFrom(string assemblyFile, string typeName)
		{
			if (assemblyFile == null)
			{
				throw new ArgumentNullException("assemblyFile");
			}
			return Activator.CreateInstanceFrom(assemblyFile, typeName);
		}

		public ObjectHandle CreateInstanceFrom(string assemblyFile, string typeName, object[] activationAttributes)
		{
			if (assemblyFile == null)
			{
				throw new ArgumentNullException("assemblyFile");
			}
			return Activator.CreateInstanceFrom(assemblyFile, typeName, activationAttributes);
		}

		public ObjectHandle CreateInstanceFrom(string assemblyFile, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, Evidence securityAttributes)
		{
			if (assemblyFile == null)
			{
				throw new ArgumentNullException("assemblyFile");
			}
			return Activator.CreateInstanceFrom(assemblyFile, typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes, securityAttributes);
		}

		public object CreateInstanceFromAndUnwrap(string assemblyName, string typeName)
		{
			ObjectHandle objectHandle = CreateInstanceFrom(assemblyName, typeName);
			return (objectHandle == null) ? null : objectHandle.Unwrap();
		}

		public object CreateInstanceFromAndUnwrap(string assemblyName, string typeName, object[] activationAttributes)
		{
			ObjectHandle objectHandle = CreateInstanceFrom(assemblyName, typeName, activationAttributes);
			return (objectHandle == null) ? null : objectHandle.Unwrap();
		}

		public object CreateInstanceFromAndUnwrap(string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, Evidence securityAttributes)
		{
			ObjectHandle objectHandle = CreateInstanceFrom(assemblyName, typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes, securityAttributes);
			return (objectHandle == null) ? null : objectHandle.Unwrap();
		}

		public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access)
		{
			return DefineDynamicAssembly(name, access, null, null, null, null, null, false);
		}

		public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, Evidence evidence)
		{
			return DefineDynamicAssembly(name, access, null, evidence, null, null, null, false);
		}

		public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir)
		{
			return DefineDynamicAssembly(name, access, dir, null, null, null, null, false);
		}

		public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir, Evidence evidence)
		{
			return DefineDynamicAssembly(name, access, dir, evidence, null, null, null, false);
		}

		public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions)
		{
			return DefineDynamicAssembly(name, access, null, null, requiredPermissions, optionalPermissions, refusedPermissions, false);
		}

		public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, Evidence evidence, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions)
		{
			return DefineDynamicAssembly(name, access, null, evidence, requiredPermissions, optionalPermissions, refusedPermissions, false);
		}

		public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions)
		{
			return DefineDynamicAssembly(name, access, dir, null, requiredPermissions, optionalPermissions, refusedPermissions, false);
		}

		public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir, Evidence evidence, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions)
		{
			return DefineDynamicAssembly(name, access, dir, evidence, requiredPermissions, optionalPermissions, refusedPermissions, false);
		}

		public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir, Evidence evidence, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions, bool isSynchronized)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			ValidateAssemblyName(name.Name);
			AssemblyBuilder assemblyBuilder = new AssemblyBuilder(name, dir, access, false);
			assemblyBuilder.AddPermissionRequests(requiredPermissions, optionalPermissions, refusedPermissions);
			return assemblyBuilder;
		}

		public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir, Evidence evidence, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions, bool isSynchronized, IEnumerable<CustomAttributeBuilder> assemblyAttributes)
		{
			AssemblyBuilder assemblyBuilder = DefineDynamicAssembly(name, access, dir, evidence, requiredPermissions, optionalPermissions, refusedPermissions, isSynchronized);
			if (assemblyAttributes != null)
			{
				foreach (CustomAttributeBuilder assemblyAttribute in assemblyAttributes)
				{
					assemblyBuilder.SetCustomAttribute(assemblyAttribute);
				}
			}
			return assemblyBuilder;
		}

		public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, IEnumerable<CustomAttributeBuilder> assemblyAttributes)
		{
			return DefineDynamicAssembly(name, access, null, null, null, null, null, false, assemblyAttributes);
		}

		internal AssemblyBuilder DefineInternalDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access)
		{
			return new AssemblyBuilder(name, null, access, true);
		}

		public void DoCallBack(CrossAppDomainDelegate callBackDelegate)
		{
			if (callBackDelegate != null)
			{
				callBackDelegate();
			}
		}

		public int ExecuteAssembly(string assemblyFile)
		{
			return ExecuteAssembly(assemblyFile, null, null);
		}

		public int ExecuteAssembly(string assemblyFile, Evidence assemblySecurity)
		{
			return ExecuteAssembly(assemblyFile, assemblySecurity, null);
		}

		public int ExecuteAssembly(string assemblyFile, Evidence assemblySecurity, string[] args)
		{
			Assembly a = Assembly.LoadFrom(assemblyFile, assemblySecurity);
			return ExecuteAssemblyInternal(a, args);
		}

		public int ExecuteAssembly(string assemblyFile, Evidence assemblySecurity, string[] args, byte[] hashValue, AssemblyHashAlgorithm hashAlgorithm)
		{
			Assembly a = Assembly.LoadFrom(assemblyFile, assemblySecurity, hashValue, hashAlgorithm);
			return ExecuteAssemblyInternal(a, args);
		}

		private int ExecuteAssemblyInternal(Assembly a, string[] args)
		{
			if (a.EntryPoint == null)
			{
				throw new MissingMethodException("Entry point not found in assembly '" + a.FullName + "'.");
			}
			return ExecuteAssembly(a, args);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int ExecuteAssembly(Assembly a, string[] args);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Assembly[] GetAssemblies(bool refOnly);

		public Assembly[] GetAssemblies()
		{
			return GetAssemblies(false);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern object GetData(string name);

		public new Type GetType()
		{
			return base.GetType();
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Assembly LoadAssembly(string assemblyRef, Evidence securityEvidence, bool refOnly);

		public Assembly Load(AssemblyName assemblyRef)
		{
			return Load(assemblyRef, null);
		}

		internal Assembly LoadSatellite(AssemblyName assemblyRef, bool throwOnError)
		{
			if (assemblyRef == null)
			{
				throw new ArgumentNullException("assemblyRef");
			}
			Assembly assembly = LoadAssembly(assemblyRef.FullName, null, false);
			if (assembly == null && throwOnError)
			{
				throw new FileNotFoundException(null, assemblyRef.Name);
			}
			return assembly;
		}

		public Assembly Load(AssemblyName assemblyRef, Evidence assemblySecurity)
		{
			if (assemblyRef == null)
			{
				throw new ArgumentNullException("assemblyRef");
			}
			if (assemblyRef.Name == null || assemblyRef.Name.Length == 0)
			{
				if (assemblyRef.CodeBase != null)
				{
					return Assembly.LoadFrom(assemblyRef.CodeBase, assemblySecurity);
				}
				throw new ArgumentException(Locale.GetText("assemblyRef.Name cannot be empty."), "assemblyRef");
			}
			Assembly assembly = LoadAssembly(assemblyRef.FullName, assemblySecurity, false);
			if (assembly != null)
			{
				return assembly;
			}
			if (assemblyRef.CodeBase == null)
			{
				throw new FileNotFoundException(null, assemblyRef.Name);
			}
			string text = assemblyRef.CodeBase;
			if (text.ToLower(CultureInfo.InvariantCulture).StartsWith("file://"))
			{
				text = new Uri(text).LocalPath;
			}
			try
			{
				assembly = Assembly.LoadFrom(text, assemblySecurity);
			}
			catch
			{
				throw new FileNotFoundException(null, assemblyRef.Name);
			}
			AssemblyName name = assembly.GetName();
			if (assemblyRef.Name != name.Name)
			{
				throw new FileNotFoundException(null, assemblyRef.Name);
			}
			if (assemblyRef.Version != new Version() && assemblyRef.Version != name.Version)
			{
				throw new FileNotFoundException(null, assemblyRef.Name);
			}
			if (assemblyRef.CultureInfo != null && assemblyRef.CultureInfo.Equals(name))
			{
				throw new FileNotFoundException(null, assemblyRef.Name);
			}
			byte[] publicKeyToken = assemblyRef.GetPublicKeyToken();
			if (publicKeyToken != null)
			{
				byte[] publicKeyToken2 = name.GetPublicKeyToken();
				if (publicKeyToken2 == null || publicKeyToken.Length != publicKeyToken2.Length)
				{
					throw new FileNotFoundException(null, assemblyRef.Name);
				}
				for (int num = publicKeyToken.Length - 1; num >= 0; num--)
				{
					if (publicKeyToken2[num] != publicKeyToken[num])
					{
						throw new FileNotFoundException(null, assemblyRef.Name);
					}
				}
			}
			return assembly;
		}

		public Assembly Load(string assemblyString)
		{
			return Load(assemblyString, null, false);
		}

		public Assembly Load(string assemblyString, Evidence assemblySecurity)
		{
			return Load(assemblyString, assemblySecurity, false);
		}

		internal Assembly Load(string assemblyString, Evidence assemblySecurity, bool refonly)
		{
			if (assemblyString == null)
			{
				throw new ArgumentNullException("assemblyString");
			}
			if (assemblyString.Length == 0)
			{
				throw new ArgumentException("assemblyString cannot have zero length");
			}
			Assembly assembly = LoadAssembly(assemblyString, assemblySecurity, refonly);
			if (assembly == null)
			{
				throw new FileNotFoundException(null, assemblyString);
			}
			return assembly;
		}

		public Assembly Load(byte[] rawAssembly)
		{
			return Load(rawAssembly, null, null);
		}

		public Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore)
		{
			return Load(rawAssembly, rawSymbolStore, null);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Assembly LoadAssemblyRaw(byte[] rawAssembly, byte[] rawSymbolStore, Evidence securityEvidence, bool refonly);

		public Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore, Evidence securityEvidence)
		{
			return Load(rawAssembly, rawSymbolStore, securityEvidence, false);
		}

		internal Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore, Evidence securityEvidence, bool refonly)
		{
			if (rawAssembly == null)
			{
				throw new ArgumentNullException("rawAssembly");
			}
			Assembly assembly = LoadAssemblyRaw(rawAssembly, rawSymbolStore, securityEvidence, refonly);
			assembly.FromByteArray = true;
			return assembly;
		}

		public void SetAppDomainPolicy(PolicyLevel domainPolicy)
		{
			if (domainPolicy == null)
			{
				throw new ArgumentNullException("domainPolicy");
			}
			if (_granted != null)
			{
				throw new PolicyException(Locale.GetText("An AppDomain policy is already specified."));
			}
			if (IsFinalizingForUnload())
			{
				throw new AppDomainUnloadedException();
			}
			PolicyStatement policyStatement = domainPolicy.Resolve(_evidence);
			_granted = policyStatement.PermissionSet;
		}

		[Obsolete("Use AppDomainSetup.SetCachePath")]
		public void SetCachePath(string path)
		{
			SetupInformationNoCopy.CachePath = path;
		}

		public void SetPrincipalPolicy(PrincipalPolicy policy)
		{
			if (IsFinalizingForUnload())
			{
				throw new AppDomainUnloadedException();
			}
			_principalPolicy = policy;
			_principal = null;
		}

		[Obsolete("Use AppDomainSetup.ShadowCopyFiles")]
		public void SetShadowCopyFiles()
		{
			SetupInformationNoCopy.ShadowCopyFiles = "true";
		}

		[Obsolete("Use AppDomainSetup.ShadowCopyDirectories")]
		public void SetShadowCopyPath(string path)
		{
			SetupInformationNoCopy.ShadowCopyDirectories = path;
		}

		public void SetThreadPrincipal(IPrincipal principal)
		{
			if (principal == null)
			{
				throw new ArgumentNullException("principal");
			}
			if (_principal != null)
			{
				throw new PolicyException(Locale.GetText("principal already present."));
			}
			if (IsFinalizingForUnload())
			{
				throw new AppDomainUnloadedException();
			}
			_principal = principal;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AppDomain InternalSetDomainByID(int domain_id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AppDomain InternalSetDomain(AppDomain context);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalPushDomainRef(AppDomain domain);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalPushDomainRefByID(int domain_id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalPopDomainRef();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Context InternalSetContext(Context context);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Context InternalGetContext();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Context InternalGetDefaultContext();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string InternalGetProcessGuid(string newguid);

		internal static object InvokeInDomain(AppDomain domain, MethodInfo method, object obj, object[] args)
		{
			AppDomain currentDomain = CurrentDomain;
			bool flag = false;
			try
			{
				InternalPushDomainRef(domain);
				flag = true;
				InternalSetDomain(domain);
				Exception exc;
				object result = ((MonoMethod)method).InternalInvoke(obj, args, out exc);
				if (exc != null)
				{
					throw exc;
				}
				return result;
			}
			finally
			{
				InternalSetDomain(currentDomain);
				if (flag)
				{
					InternalPopDomainRef();
				}
			}
		}

		internal static object InvokeInDomainByID(int domain_id, MethodInfo method, object obj, object[] args)
		{
			AppDomain currentDomain = CurrentDomain;
			bool flag = false;
			try
			{
				InternalPushDomainRefByID(domain_id);
				flag = true;
				InternalSetDomainByID(domain_id);
				Exception exc;
				object result = ((MonoMethod)method).InternalInvoke(obj, args, out exc);
				if (exc != null)
				{
					throw exc;
				}
				return result;
			}
			finally
			{
				InternalSetDomain(currentDomain);
				if (flag)
				{
					InternalPopDomainRef();
				}
			}
		}

		internal static string GetProcessGuid()
		{
			if (_process_guid == null)
			{
				_process_guid = InternalGetProcessGuid(Guid.NewGuid().ToString());
			}
			return _process_guid;
		}

		public static AppDomain CreateDomain(string friendlyName)
		{
			return CreateDomain(friendlyName, null, null);
		}

		public static AppDomain CreateDomain(string friendlyName, Evidence securityInfo)
		{
			return CreateDomain(friendlyName, securityInfo, null);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AppDomain createDomain(string friendlyName, AppDomainSetup info);

		[MonoLimitation("Currently it does not allow the setup in the other domain")]
		public static AppDomain CreateDomain(string friendlyName, Evidence securityInfo, AppDomainSetup info)
		{
			if (friendlyName == null)
			{
				throw new ArgumentNullException("friendlyName");
			}
			AppDomain defaultDomain = DefaultDomain;
			info = ((info != null) ? new AppDomainSetup(info) : ((defaultDomain != null) ? defaultDomain.SetupInformation : new AppDomainSetup()));
			if (defaultDomain != null)
			{
				if (!info.Equals(defaultDomain.SetupInformation))
				{
					if (info.ApplicationBase == null)
					{
						info.ApplicationBase = defaultDomain.SetupInformation.ApplicationBase;
					}
					if (info.ConfigurationFile == null)
					{
						info.ConfigurationFile = Path.GetFileName(defaultDomain.SetupInformation.ConfigurationFile);
					}
				}
			}
			else if (info.ConfigurationFile == null)
			{
				info.ConfigurationFile = "[I don't have a config file]";
			}
			AppDomain appDomain = (AppDomain)RemotingServices.GetDomainProxy(createDomain(friendlyName, info));
			if (securityInfo == null)
			{
				if (defaultDomain == null)
				{
					appDomain._evidence = null;
				}
				else
				{
					appDomain._evidence = defaultDomain.Evidence;
				}
			}
			else
			{
				appDomain._evidence = new Evidence(securityInfo);
			}
			return appDomain;
		}

		public static AppDomain CreateDomain(string friendlyName, Evidence securityInfo, string appBasePath, string appRelativeSearchPath, bool shadowCopyFiles)
		{
			return CreateDomain(friendlyName, securityInfo, CreateDomainSetup(appBasePath, appRelativeSearchPath, shadowCopyFiles));
		}

		private static AppDomainSetup CreateDomainSetup(string appBasePath, string appRelativeSearchPath, bool shadowCopyFiles)
		{
			AppDomainSetup appDomainSetup = new AppDomainSetup();
			appDomainSetup.ApplicationBase = appBasePath;
			appDomainSetup.PrivateBinPath = appRelativeSearchPath;
			if (shadowCopyFiles)
			{
				appDomainSetup.ShadowCopyFiles = "true";
			}
			else
			{
				appDomainSetup.ShadowCopyFiles = "false";
			}
			return appDomainSetup;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool InternalIsFinalizingForUnload(int domain_id);

		public bool IsFinalizingForUnload()
		{
			return InternalIsFinalizingForUnload(getDomainID());
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalUnload(int domain_id);

		private int getDomainID()
		{
			return Thread.GetDomainID();
		}

		[ReliabilityContract(Consistency.MayCorruptAppDomain, Cer.MayFail)]
		public static void Unload(AppDomain domain)
		{
			if (domain == null)
			{
				throw new ArgumentNullException("domain");
			}
			InternalUnload(domain.getDomainID());
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetData(string name, object data);

		[MonoTODO]
		public void SetData(string name, object data, IPermission permission)
		{
			throw new NotImplementedException();
		}

		[Obsolete("AppDomain.GetCurrentThreadId has been deprecated because it does not provide a stable Id when managed threads are running on fibers (aka lightweight threads). To get a stable identifier for a managed thread, use the ManagedThreadId property on Thread.'")]
		public static int GetCurrentThreadId()
		{
			return Thread.CurrentThreadId;
		}

		public override string ToString()
		{
			return getFriendlyName();
		}

		private static void ValidateAssemblyName(string name)
		{
			if (name == null || name.Length == 0)
			{
				throw new ArgumentException("The Name of AssemblyName cannot be null or a zero-length string.");
			}
			bool flag = true;
			for (int i = 0; i < name.Length; i++)
			{
				char c = name[i];
				if (i == 0 && char.IsWhiteSpace(c))
				{
					flag = false;
					break;
				}
				if (c == '/' || c == '\\' || c == ':')
				{
					flag = false;
					break;
				}
			}
			if (!flag)
			{
				throw new ArgumentException("The Name of AssemblyName cannot start with whitespace, or contain '/', '\\'  or ':'.");
			}
		}

		private void DoAssemblyLoad(Assembly assembly)
		{
			if (this.AssemblyLoad != null)
			{
				this.AssemblyLoad(this, new AssemblyLoadEventArgs(assembly));
			}
		}

		private Assembly DoAssemblyResolve(string name, bool refonly)
		{
			ResolveEventHandler assemblyResolve = this.AssemblyResolve;
			if (assemblyResolve == null)
			{
				return null;
			}
			Hashtable hashtable;
			if (refonly)
			{
				hashtable = assembly_resolve_in_progress_refonly;
				if (hashtable == null)
				{
					hashtable = (assembly_resolve_in_progress_refonly = new Hashtable());
				}
			}
			else
			{
				hashtable = assembly_resolve_in_progress;
				if (hashtable == null)
				{
					hashtable = (assembly_resolve_in_progress = new Hashtable());
				}
			}
			string text = (string)hashtable[name];
			if (text != null)
			{
				return null;
			}
			hashtable[name] = name;
			try
			{
				Delegate[] invocationList = assemblyResolve.GetInvocationList();
				Delegate[] array = invocationList;
				foreach (Delegate obj in array)
				{
					ResolveEventHandler resolveEventHandler = (ResolveEventHandler)obj;
					Assembly assembly = resolveEventHandler(this, new ResolveEventArgs(name));
					if (assembly != null)
					{
						return assembly;
					}
				}
				return null;
			}
			finally
			{
				hashtable.Remove(name);
			}
		}

		internal Assembly DoTypeResolve(object name_or_tb)
		{
			if (this.TypeResolve == null)
			{
				return null;
			}
			string text = ((!(name_or_tb is TypeBuilder)) ? ((string)name_or_tb) : ((TypeBuilder)name_or_tb).FullName);
			Hashtable hashtable = type_resolve_in_progress;
			if (hashtable == null)
			{
				hashtable = (type_resolve_in_progress = new Hashtable());
			}
			if (hashtable.Contains(text))
			{
				return null;
			}
			hashtable[text] = text;
			try
			{
				Delegate[] invocationList = this.TypeResolve.GetInvocationList();
				foreach (Delegate obj in invocationList)
				{
					ResolveEventHandler resolveEventHandler = (ResolveEventHandler)obj;
					Assembly assembly = resolveEventHandler(this, new ResolveEventArgs(text));
					if (assembly != null)
					{
						return assembly;
					}
				}
				return null;
			}
			finally
			{
				hashtable.Remove(text);
			}
		}

		private void DoDomainUnload()
		{
			if (this.DomainUnload != null)
			{
				this.DomainUnload(this, null);
			}
		}

		internal void ProcessMessageInDomain(byte[] arrRequest, CADMethodCallMessage cadMsg, out byte[] arrResponse, out CADMethodReturnMessage cadMrm)
		{
			IMessage msg = ((arrRequest == null) ? new MethodCall(cadMsg) : CADSerializer.DeserializeMessage(new MemoryStream(arrRequest), null));
			IMessage message = ChannelServices.SyncDispatchMessage(msg);
			cadMrm = CADMethodReturnMessage.Create(message);
			if (cadMrm == null)
			{
				arrResponse = CADSerializer.SerializeMessage(message).GetBuffer();
			}
			else
			{
				arrResponse = null;
			}
		}

		[ComVisible(false)]
		[MonoTODO("This routine only returns the parameter currently")]
		public string ApplyPolicy(string assemblyName)
		{
			if (assemblyName == null)
			{
				throw new ArgumentNullException("assemblyName");
			}
			if (assemblyName.Length == 0)
			{
				throw new ArgumentException("assemblyName");
			}
			return assemblyName;
		}

		public static AppDomain CreateDomain(string friendlyName, Evidence securityInfo, string appBasePath, string appRelativeSearchPath, bool shadowCopyFiles, AppDomainInitializer adInit, string[] adInitArgs)
		{
			AppDomainSetup appDomainSetup = CreateDomainSetup(appBasePath, appRelativeSearchPath, shadowCopyFiles);
			appDomainSetup.AppDomainInitializerArguments = adInitArgs;
			appDomainSetup.AppDomainInitializer = adInit;
			return CreateDomain(friendlyName, securityInfo, appDomainSetup);
		}

		public int ExecuteAssemblyByName(string assemblyName)
		{
			return ExecuteAssemblyByName(assemblyName, (Evidence)null, (string[])null);
		}

		public int ExecuteAssemblyByName(string assemblyName, Evidence assemblySecurity)
		{
			return ExecuteAssemblyByName(assemblyName, assemblySecurity, (string[])null);
		}

		public int ExecuteAssemblyByName(string assemblyName, Evidence assemblySecurity, params string[] args)
		{
			Assembly a = Assembly.Load(assemblyName, assemblySecurity);
			return ExecuteAssemblyInternal(a, args);
		}

		public int ExecuteAssemblyByName(AssemblyName assemblyName, Evidence assemblySecurity, params string[] args)
		{
			Assembly a = Assembly.Load(assemblyName, assemblySecurity);
			return ExecuteAssemblyInternal(a, args);
		}

		public bool IsDefaultAppDomain()
		{
			return object.ReferenceEquals(this, DefaultDomain);
		}

		public Assembly[] ReflectionOnlyGetAssemblies()
		{
			return GetAssemblies(true);
		}
	}
}
