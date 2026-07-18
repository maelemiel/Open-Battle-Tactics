using System.Runtime.InteropServices;

namespace System.Deployment.Internal
{
	[ComVisible(false)]
	public static class InternalApplicationIdentityHelper
	{
		[MonoTODO("2.0 SP1 member")]
		public static object GetActivationContextData(ActivationContext appInfo)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static object GetInternalAppId(ApplicationIdentity id)
		{
			throw new NotImplementedException();
		}
	}
}
