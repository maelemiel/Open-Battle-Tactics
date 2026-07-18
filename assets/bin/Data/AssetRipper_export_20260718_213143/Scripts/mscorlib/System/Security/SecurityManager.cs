using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;

namespace System.Security
{
	[ComVisible(true)]
	public static class SecurityManager
	{
		private static object _lockObject;

		private static ArrayList _hierarchy;

		private static IPermission _unmanagedCode;

		private static Hashtable _declsecCache;

		private static PolicyLevel _level;

		private static SecurityPermission _execution;

		public static extern bool CheckExecutionRights
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("The security manager cannot be turned off on MS runtime")]
		public static extern bool SecurityEnabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		private static IEnumerator Hierarchy
		{
			get
			{
				lock (_lockObject)
				{
					if (_hierarchy == null)
					{
						InitializePolicyHierarchy();
					}
				}
				return _hierarchy.GetEnumerator();
			}
		}

		internal static PolicyLevel ResolvingPolicyLevel
		{
			get
			{
				return _level;
			}
			set
			{
				_level = value;
			}
		}

		private static IPermission UnmanagedCode
		{
			get
			{
				lock (_lockObject)
				{
					if (_unmanagedCode == null)
					{
						_unmanagedCode = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
					}
				}
				return _unmanagedCode;
			}
		}

		static SecurityManager()
		{
			_execution = new SecurityPermission(SecurityPermissionFlag.Execution);
			_lockObject = new object();
		}

		[MonoTODO("CAS support is experimental (and unsupported). This method only works in FullTrust.")]
		public static void GetZoneAndOrigin(out ArrayList zone, out ArrayList origin)
		{
			zone = new ArrayList();
			origin = new ArrayList();
		}

		public static bool IsGranted(IPermission perm)
		{
			if (perm == null)
			{
				return true;
			}
			if (!SecurityEnabled)
			{
				return true;
			}
			return IsGranted(Assembly.GetCallingAssembly(), perm);
		}

		internal static bool IsGranted(Assembly a, IPermission perm)
		{
			PermissionSet grantedPermissionSet = a.GrantedPermissionSet;
			if (grantedPermissionSet != null && !grantedPermissionSet.IsUnrestricted())
			{
				CodeAccessPermission target = (CodeAccessPermission)grantedPermissionSet.GetPermission(perm.GetType());
				if (!perm.IsSubsetOf(target))
				{
					return false;
				}
			}
			PermissionSet deniedPermissionSet = a.DeniedPermissionSet;
			if (deniedPermissionSet != null && !deniedPermissionSet.IsEmpty())
			{
				if (deniedPermissionSet.IsUnrestricted())
				{
					return false;
				}
				CodeAccessPermission codeAccessPermission = (CodeAccessPermission)a.DeniedPermissionSet.GetPermission(perm.GetType());
				if (codeAccessPermission != null && perm.IsSubsetOf(codeAccessPermission))
				{
					return false;
				}
			}
			return true;
		}

		internal static IPermission CheckPermissionSet(Assembly a, PermissionSet ps, bool noncas)
		{
			if (ps.IsEmpty())
			{
				return null;
			}
			foreach (IPermission p in ps)
			{
				if (!noncas && p is CodeAccessPermission)
				{
					if (!IsGranted(a, p))
					{
						return p;
					}
					continue;
				}
				try
				{
					p.Demand();
				}
				catch (SecurityException)
				{
					return p;
				}
			}
			return null;
		}

		internal static IPermission CheckPermissionSet(AppDomain ad, PermissionSet ps)
		{
			if (ps == null || ps.IsEmpty())
			{
				return null;
			}
			PermissionSet grantedPermissionSet = ad.GrantedPermissionSet;
			if (grantedPermissionSet == null)
			{
				return null;
			}
			if (grantedPermissionSet.IsUnrestricted())
			{
				return null;
			}
			if (ps.IsUnrestricted())
			{
				return new SecurityPermission(SecurityPermissionFlag.NoFlags);
			}
			foreach (IPermission p in ps)
			{
				if (p is CodeAccessPermission)
				{
					CodeAccessPermission codeAccessPermission = (CodeAccessPermission)grantedPermissionSet.GetPermission(p.GetType());
					if (codeAccessPermission == null)
					{
						if ((!grantedPermissionSet.IsUnrestricted() || !(p is IUnrestrictedPermission)) && !p.IsSubsetOf(null))
						{
							return p;
						}
					}
					else if (!p.IsSubsetOf(codeAccessPermission))
					{
						return p;
					}
				}
				else
				{
					try
					{
						p.Demand();
					}
					catch (SecurityException)
					{
						return p;
					}
				}
			}
			return null;
		}

		public static PolicyLevel LoadPolicyLevelFromFile(string path, PolicyLevelType type)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			PolicyLevel policyLevel = null;
			try
			{
				policyLevel = new PolicyLevel(type.ToString(), type);
				policyLevel.LoadFromFile(path);
				return policyLevel;
			}
			catch (Exception innerException)
			{
				throw new ArgumentException(Locale.GetText("Invalid policy XML"), innerException);
			}
		}

		public static PolicyLevel LoadPolicyLevelFromString(string str, PolicyLevelType type)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			PolicyLevel policyLevel = null;
			try
			{
				policyLevel = new PolicyLevel(type.ToString(), type);
				policyLevel.LoadFromString(str);
				return policyLevel;
			}
			catch (Exception innerException)
			{
				throw new ArgumentException(Locale.GetText("Invalid policy XML"), innerException);
			}
		}

		public static IEnumerator PolicyHierarchy()
		{
			return Hierarchy;
		}

		public static PermissionSet ResolvePolicy(Evidence evidence)
		{
			if (evidence == null)
			{
				return new PermissionSet(PermissionState.None);
			}
			PermissionSet ps = null;
			IEnumerator hierarchy = Hierarchy;
			while (hierarchy.MoveNext())
			{
				PolicyLevel pl = (PolicyLevel)hierarchy.Current;
				if (ResolvePolicyLevel(ref ps, pl, evidence))
				{
					break;
				}
			}
			ResolveIdentityPermissions(ps, evidence);
			return ps;
		}

		[MonoTODO("(2.0) more tests are needed")]
		public static PermissionSet ResolvePolicy(Evidence[] evidences)
		{
			if (evidences == null || evidences.Length == 0 || (evidences.Length == 1 && evidences[0].Count == 0))
			{
				return new PermissionSet(PermissionState.None);
			}
			PermissionSet permissionSet = ResolvePolicy(evidences[0]);
			for (int i = 1; i < evidences.Length; i++)
			{
				permissionSet = permissionSet.Intersect(ResolvePolicy(evidences[i]));
			}
			return permissionSet;
		}

		public static PermissionSet ResolveSystemPolicy(Evidence evidence)
		{
			if (evidence == null)
			{
				return new PermissionSet(PermissionState.None);
			}
			PermissionSet ps = null;
			IEnumerator hierarchy = Hierarchy;
			while (hierarchy.MoveNext())
			{
				PolicyLevel policyLevel = (PolicyLevel)hierarchy.Current;
				if (policyLevel.Type == PolicyLevelType.AppDomain || ResolvePolicyLevel(ref ps, policyLevel, evidence))
				{
					break;
				}
			}
			ResolveIdentityPermissions(ps, evidence);
			return ps;
		}

		public static PermissionSet ResolvePolicy(Evidence evidence, PermissionSet reqdPset, PermissionSet optPset, PermissionSet denyPset, out PermissionSet denied)
		{
			PermissionSet permissionSet = ResolvePolicy(evidence);
			if (reqdPset != null && !reqdPset.IsSubsetOf(permissionSet))
			{
				throw new PolicyException(Locale.GetText("Policy doesn't grant the minimal permissions required to execute the assembly."));
			}
			if (CheckExecutionRights)
			{
				bool flag = false;
				if (permissionSet != null)
				{
					if (permissionSet.IsUnrestricted())
					{
						flag = true;
					}
					else
					{
						IPermission permission = permissionSet.GetPermission(typeof(SecurityPermission));
						flag = _execution.IsSubsetOf(permission);
					}
				}
				if (!flag)
				{
					throw new PolicyException(Locale.GetText("Policy doesn't grant the right to execute the assembly."));
				}
			}
			denied = denyPset;
			return permissionSet;
		}

		public static IEnumerator ResolvePolicyGroups(Evidence evidence)
		{
			if (evidence == null)
			{
				throw new ArgumentNullException("evidence");
			}
			ArrayList arrayList = new ArrayList();
			IEnumerator hierarchy = Hierarchy;
			while (hierarchy.MoveNext())
			{
				PolicyLevel policyLevel = (PolicyLevel)hierarchy.Current;
				CodeGroup value = policyLevel.ResolveMatchingCodeGroups(evidence);
				arrayList.Add(value);
			}
			return arrayList.GetEnumerator();
		}

		public static void SavePolicy()
		{
			IEnumerator hierarchy = Hierarchy;
			while (hierarchy.MoveNext())
			{
				PolicyLevel policyLevel = hierarchy.Current as PolicyLevel;
				policyLevel.Save();
			}
		}

		public static void SavePolicyLevel(PolicyLevel level)
		{
			level.Save();
		}

		private static void InitializePolicyHierarchy()
		{
			string directoryName = Path.GetDirectoryName(Environment.GetMachineConfigPath());
			string path = Path.Combine(Environment.InternalGetFolderPath(Environment.SpecialFolder.ApplicationData), "mono");
			PolicyLevel policyLevel = (_level = new PolicyLevel("Enterprise", PolicyLevelType.Enterprise));
			policyLevel.LoadFromFile(Path.Combine(directoryName, "enterprisesec.config"));
			PolicyLevel policyLevel2 = (_level = new PolicyLevel("Machine", PolicyLevelType.Machine));
			policyLevel2.LoadFromFile(Path.Combine(directoryName, "security.config"));
			PolicyLevel policyLevel3 = (_level = new PolicyLevel("User", PolicyLevelType.User));
			policyLevel3.LoadFromFile(Path.Combine(path, "security.config"));
			ArrayList arrayList = new ArrayList();
			arrayList.Add(policyLevel);
			arrayList.Add(policyLevel2);
			arrayList.Add(policyLevel3);
			_hierarchy = ArrayList.Synchronized(arrayList);
			_level = null;
		}

		internal static bool ResolvePolicyLevel(ref PermissionSet ps, PolicyLevel pl, Evidence evidence)
		{
			PolicyStatement policyStatement = pl.Resolve(evidence);
			if (policyStatement != null)
			{
				if (ps == null)
				{
					ps = policyStatement.PermissionSet;
				}
				else
				{
					ps = ps.Intersect(policyStatement.PermissionSet);
					if (ps == null)
					{
						ps = new PermissionSet(PermissionState.None);
					}
				}
				if ((policyStatement.Attributes & PolicyStatementAttribute.LevelFinal) == PolicyStatementAttribute.LevelFinal)
				{
					return true;
				}
			}
			return false;
		}

		internal static void ResolveIdentityPermissions(PermissionSet ps, Evidence evidence)
		{
			if (ps.IsUnrestricted())
			{
				return;
			}
			IEnumerator hostEnumerator = evidence.GetHostEnumerator();
			while (hostEnumerator.MoveNext())
			{
				IIdentityPermissionFactory identityPermissionFactory = hostEnumerator.Current as IIdentityPermissionFactory;
				if (identityPermissionFactory != null)
				{
					IPermission perm = identityPermissionFactory.CreateIdentityPermission(evidence);
					ps.AddPermission(perm);
				}
			}
		}

		internal static PermissionSet Decode(IntPtr permissions, int length)
		{
			PermissionSet permissionSet = null;
			lock (_lockObject)
			{
				if (_declsecCache == null)
				{
					_declsecCache = new Hashtable();
				}
				object key = (int)permissions;
				permissionSet = (PermissionSet)_declsecCache[key];
				if (permissionSet == null)
				{
					byte[] array = new byte[length];
					Marshal.Copy(permissions, array, 0, length);
					permissionSet = Decode(array);
					permissionSet.DeclarativeSecurity = true;
					_declsecCache.Add(key, permissionSet);
				}
			}
			return permissionSet;
		}

		internal static PermissionSet Decode(byte[] encodedPermissions)
		{
			if (encodedPermissions == null || encodedPermissions.Length < 1)
			{
				throw new SecurityException("Invalid metadata format.");
			}
			switch (encodedPermissions[0])
			{
			case 60:
			{
				string xml = Encoding.Unicode.GetString(encodedPermissions);
				return new PermissionSet(xml);
			}
			case 46:
				return PermissionSet.CreateFromBinaryFormat(encodedPermissions);
			default:
				throw new SecurityException(Locale.GetText("Unknown metadata format."));
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern bool GetLinkDemandSecurity(MethodBase method, RuntimeDeclSecurityActions* cdecl, RuntimeDeclSecurityActions* mdecl);

		internal unsafe static void ReflectedLinkDemandInvoke(MethodBase mb)
		{
			RuntimeDeclSecurityActions runtimeDeclSecurityActions = default(RuntimeDeclSecurityActions);
			RuntimeDeclSecurityActions runtimeDeclSecurityActions2 = default(RuntimeDeclSecurityActions);
			if (GetLinkDemandSecurity(mb, &runtimeDeclSecurityActions, &runtimeDeclSecurityActions2))
			{
				PermissionSet permissionSet = null;
				if (runtimeDeclSecurityActions.cas.size > 0)
				{
					permissionSet = Decode(runtimeDeclSecurityActions.cas.blob, runtimeDeclSecurityActions.cas.size);
				}
				if (runtimeDeclSecurityActions.noncas.size > 0)
				{
					PermissionSet permissionSet2 = Decode(runtimeDeclSecurityActions.noncas.blob, runtimeDeclSecurityActions.noncas.size);
					permissionSet = ((permissionSet != null) ? permissionSet.Union(permissionSet2) : permissionSet2);
				}
				if (runtimeDeclSecurityActions2.cas.size > 0)
				{
					PermissionSet permissionSet3 = Decode(runtimeDeclSecurityActions2.cas.blob, runtimeDeclSecurityActions2.cas.size);
					permissionSet = ((permissionSet != null) ? permissionSet.Union(permissionSet3) : permissionSet3);
				}
				if (runtimeDeclSecurityActions2.noncas.size > 0)
				{
					PermissionSet permissionSet4 = Decode(runtimeDeclSecurityActions2.noncas.blob, runtimeDeclSecurityActions2.noncas.size);
					permissionSet = ((permissionSet != null) ? permissionSet.Union(permissionSet4) : permissionSet4);
				}
				if (permissionSet != null)
				{
					permissionSet.Demand();
				}
			}
		}

		internal unsafe static bool ReflectedLinkDemandQuery(MethodBase mb)
		{
			RuntimeDeclSecurityActions runtimeDeclSecurityActions = default(RuntimeDeclSecurityActions);
			RuntimeDeclSecurityActions runtimeDeclSecurityActions2 = default(RuntimeDeclSecurityActions);
			if (!GetLinkDemandSecurity(mb, &runtimeDeclSecurityActions, &runtimeDeclSecurityActions2))
			{
				return true;
			}
			return LinkDemand(mb.ReflectedType.Assembly, &runtimeDeclSecurityActions, &runtimeDeclSecurityActions2);
		}

		private unsafe static bool LinkDemand(Assembly a, RuntimeDeclSecurityActions* klass, RuntimeDeclSecurityActions* method)
		{
			try
			{
				PermissionSet permissionSet = null;
				bool flag = true;
				if (klass->cas.size > 0)
				{
					permissionSet = Decode(klass->cas.blob, klass->cas.size);
					flag = CheckPermissionSet(a, permissionSet, false) == null;
				}
				if (flag && klass->noncas.size > 0)
				{
					permissionSet = Decode(klass->noncas.blob, klass->noncas.size);
					flag = CheckPermissionSet(a, permissionSet, true) == null;
				}
				if (flag && method->cas.size > 0)
				{
					permissionSet = Decode(method->cas.blob, method->cas.size);
					flag = CheckPermissionSet(a, permissionSet, false) == null;
				}
				if (flag && method->noncas.size > 0)
				{
					permissionSet = Decode(method->noncas.blob, method->noncas.size);
					flag = CheckPermissionSet(a, permissionSet, true) == null;
				}
				return flag;
			}
			catch (SecurityException)
			{
				return false;
			}
		}

		private static bool LinkDemandFullTrust(Assembly a)
		{
			PermissionSet grantedPermissionSet = a.GrantedPermissionSet;
			if (grantedPermissionSet != null && !grantedPermissionSet.IsUnrestricted())
			{
				return false;
			}
			PermissionSet deniedPermissionSet = a.DeniedPermissionSet;
			if (deniedPermissionSet != null && !deniedPermissionSet.IsEmpty())
			{
				return false;
			}
			return true;
		}

		private static bool LinkDemandUnmanaged(Assembly a)
		{
			return IsGranted(a, UnmanagedCode);
		}

		private static void LinkDemandSecurityException(int securityViolation, IntPtr methodHandle)
		{
			RuntimeMethodHandle handle = new RuntimeMethodHandle(methodHandle);
			MethodInfo methodInfo = (MethodInfo)MethodBase.GetMethodFromHandle(handle);
			Assembly assembly = methodInfo.DeclaringType.Assembly;
			string text = null;
			AssemblyName assemblyName = null;
			PermissionSet grant = null;
			PermissionSet refused = null;
			object demanded = null;
			IPermission permThatFailed = null;
			if (assembly != null)
			{
				assemblyName = assembly.UnprotectedGetName();
				grant = assembly.GrantedPermissionSet;
				refused = assembly.DeniedPermissionSet;
			}
			switch (securityViolation)
			{
			case 1:
				text = Locale.GetText("Permissions refused to call this method.");
				break;
			case 2:
				text = Locale.GetText("Partially trusted callers aren't allowed to call into this assembly.");
				demanded = DefaultPolicies.FullTrust;
				break;
			case 4:
				text = Locale.GetText("Calling internal calls is restricted to ECMA signed assemblies.");
				break;
			case 8:
				text = Locale.GetText("Calling unmanaged code isn't allowed from this assembly.");
				demanded = _unmanagedCode;
				permThatFailed = _unmanagedCode;
				break;
			default:
				text = Locale.GetText("JIT time LinkDemand failed.");
				break;
			}
			throw new SecurityException(text, assemblyName, grant, refused, methodInfo, SecurityAction.LinkDemand, demanded, permThatFailed, null);
		}

		private static void InheritanceDemandSecurityException(int securityViolation, Assembly a, Type t, MethodInfo method)
		{
			string text = null;
			AssemblyName assemblyName = null;
			PermissionSet grant = null;
			PermissionSet refused = null;
			if (a != null)
			{
				assemblyName = a.UnprotectedGetName();
				grant = a.GrantedPermissionSet;
				refused = a.DeniedPermissionSet;
			}
			switch (securityViolation)
			{
			case 1:
				text = string.Format(Locale.GetText("Class inheritance refused for {0}."), t);
				break;
			case 2:
				text = Locale.GetText("Method override refused.");
				break;
			default:
				text = Locale.GetText("Load time InheritDemand failed.");
				break;
			}
			throw new SecurityException(text, assemblyName, grant, refused, method, SecurityAction.InheritanceDemand, null, null, null);
		}

		private static void ThrowException(Exception ex)
		{
			throw ex;
		}

		private unsafe static bool InheritanceDemand(AppDomain ad, Assembly a, RuntimeDeclSecurityActions* actions)
		{
			try
			{
				PermissionSet permissionSet = null;
				bool flag = true;
				if (actions->cas.size > 0)
				{
					permissionSet = Decode(actions->cas.blob, actions->cas.size);
					flag = CheckPermissionSet(a, permissionSet, false) == null;
					if (flag)
					{
						flag = CheckPermissionSet(ad, permissionSet) == null;
					}
				}
				if (actions->noncas.size > 0)
				{
					permissionSet = Decode(actions->noncas.blob, actions->noncas.size);
					flag = CheckPermissionSet(a, permissionSet, true) == null;
					if (flag)
					{
						flag = CheckPermissionSet(ad, permissionSet) == null;
					}
				}
				return flag;
			}
			catch (SecurityException)
			{
				return false;
			}
		}

		private static void DemandUnmanaged()
		{
			UnmanagedCode.Demand();
		}

		private static void InternalDemand(IntPtr permissions, int length)
		{
			PermissionSet permissionSet = Decode(permissions, length);
			permissionSet.Demand();
		}

		private static void InternalDemandChoice(IntPtr permissions, int length)
		{
			throw new SecurityException("SecurityAction.DemandChoice was removed from 2.0");
		}
	}
}
