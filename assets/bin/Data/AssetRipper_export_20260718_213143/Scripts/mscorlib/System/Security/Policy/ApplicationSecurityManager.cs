using System.Runtime.InteropServices;

namespace System.Security.Policy
{
	[ComVisible(true)]
	public static class ApplicationSecurityManager
	{
		private const string config = "ApplicationTrust.config";

		private static IApplicationTrustManager _appTrustManager;

		private static ApplicationTrustCollection _userAppTrusts;

		public static IApplicationTrustManager ApplicationTrustManager
		{
			get
			{
				if (_appTrustManager == null)
				{
					_appTrustManager = new MonoTrustManager();
				}
				return _appTrustManager;
			}
		}

		public static ApplicationTrustCollection UserApplicationTrusts
		{
			get
			{
				if (_userAppTrusts == null)
				{
					_userAppTrusts = new ApplicationTrustCollection();
				}
				return _userAppTrusts;
			}
		}

		[MonoTODO("Missing application manifest support")]
		public static bool DetermineApplicationTrust(ActivationContext activationContext, TrustManagerContext context)
		{
			if (activationContext == null)
			{
				throw new NullReferenceException("activationContext");
			}
			ApplicationTrust applicationTrust = ApplicationTrustManager.DetermineApplicationTrust(activationContext, context);
			return applicationTrust.IsApplicationTrustedToRun;
		}
	}
}
