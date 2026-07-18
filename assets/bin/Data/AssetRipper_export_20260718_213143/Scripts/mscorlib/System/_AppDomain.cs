using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Security;
using System.Security.Policy;
using System.Security.Principal;

namespace System
{
	[ComVisible(true)]
	[Guid("05F696DC-2B29-3663-AD8B-C4389CF2A713")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[CLSCompliant(false)]
	public interface _AppDomain
	{
		string BaseDirectory { get; }

		string DynamicDirectory { get; }

		Evidence Evidence { get; }

		string FriendlyName { get; }

		string RelativeSearchPath { get; }

		bool ShadowCopyFiles { get; }

		event AssemblyLoadEventHandler AssemblyLoad;

		event ResolveEventHandler AssemblyResolve;

		event EventHandler DomainUnload;

		event EventHandler ProcessExit;

		event ResolveEventHandler ResourceResolve;

		event ResolveEventHandler TypeResolve;

		event UnhandledExceptionEventHandler UnhandledException;

		void AppendPrivatePath(string path);

		void ClearPrivatePath();

		void ClearShadowCopyPath();

		ObjectHandle CreateInstance(string assemblyName, string typeName);

		ObjectHandle CreateInstance(string assemblyName, string typeName, object[] activationAttributes);

		ObjectHandle CreateInstance(string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, Evidence securityAttributes);

		ObjectHandle CreateInstanceFrom(string assemblyFile, string typeName);

		ObjectHandle CreateInstanceFrom(string assemblyFile, string typeName, object[] activationAttributes);

		ObjectHandle CreateInstanceFrom(string assemblyFile, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, Evidence securityAttributes);

		AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access);

		AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, Evidence evidence);

		AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir);

		AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir, Evidence evidence);

		AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions);

		AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, Evidence evidence, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions);

		AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions);

		AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir, Evidence evidence, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions);

		AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir, Evidence evidence, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions, bool isSynchronized);

		void DoCallBack(CrossAppDomainDelegate theDelegate);

		new bool Equals(object other);

		int ExecuteAssembly(string assemblyFile);

		int ExecuteAssembly(string assemblyFile, Evidence assemblySecurity);

		int ExecuteAssembly(string assemblyFile, Evidence assemblySecurity, string[] args);

		Assembly[] GetAssemblies();

		object GetData(string name);

		new int GetHashCode();

		object GetLifetimeService();

		new Type GetType();

		object InitializeLifetimeService();

		Assembly Load(AssemblyName assemblyRef);

		Assembly Load(byte[] rawAssembly);

		Assembly Load(string assemblyString);

		Assembly Load(AssemblyName assemblyRef, Evidence assemblySecurity);

		Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore);

		Assembly Load(string assemblyString, Evidence assemblySecurity);

		Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore, Evidence securityEvidence);

		void SetAppDomainPolicy(PolicyLevel domainPolicy);

		void SetCachePath(string s);

		void SetData(string name, object data);

		void SetPrincipalPolicy(PrincipalPolicy policy);

		void SetShadowCopyPath(string s);

		void SetThreadPrincipal(IPrincipal principal);

		new string ToString();

		void GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId);

		void GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo);

		void GetTypeInfoCount(out uint pcTInfo);

		void Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr);
	}
}
