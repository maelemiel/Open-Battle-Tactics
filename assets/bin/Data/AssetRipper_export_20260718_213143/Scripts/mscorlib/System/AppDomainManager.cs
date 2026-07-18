using System.Reflection;
using System.Runtime.Hosting;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Policy;
using System.Threading;

namespace System
{
	[ComVisible(true)]
	public class AppDomainManager : MarshalByRefObject
	{
		private ApplicationActivator _activator;

		private AppDomainManagerInitializationOptions _flags;

		public virtual ApplicationActivator ApplicationActivator
		{
			get
			{
				if (_activator == null)
				{
					_activator = new ApplicationActivator();
				}
				return _activator;
			}
		}

		public virtual Assembly EntryAssembly
		{
			get
			{
				return Assembly.GetEntryAssembly();
			}
		}

		[MonoTODO]
		public virtual HostExecutionContextManager HostExecutionContextManager
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual HostSecurityManager HostSecurityManager
		{
			get
			{
				return null;
			}
		}

		public AppDomainManagerInitializationOptions InitializationFlags
		{
			get
			{
				return _flags;
			}
			set
			{
				_flags = value;
			}
		}

		public AppDomainManager()
		{
			_flags = AppDomainManagerInitializationOptions.None;
		}

		public virtual AppDomain CreateDomain(string friendlyName, Evidence securityInfo, AppDomainSetup appDomainInfo)
		{
			InitializeNewDomain(appDomainInfo);
			AppDomain appDomain = CreateDomainHelper(friendlyName, securityInfo, appDomainInfo);
			if ((HostSecurityManager.Flags & HostSecurityManagerOptions.HostPolicyLevel) == HostSecurityManagerOptions.HostPolicyLevel)
			{
				PolicyLevel domainPolicy = HostSecurityManager.DomainPolicy;
				if (domainPolicy != null)
				{
					appDomain.SetAppDomainPolicy(domainPolicy);
				}
			}
			return appDomain;
		}

		public virtual void InitializeNewDomain(AppDomainSetup appDomainInfo)
		{
		}

		public virtual bool CheckSecuritySettings(SecurityState state)
		{
			return false;
		}

		protected static AppDomain CreateDomainHelper(string friendlyName, Evidence securityInfo, AppDomainSetup appDomainInfo)
		{
			return AppDomain.CreateDomain(friendlyName, securityInfo, appDomainInfo);
		}
	}
}
