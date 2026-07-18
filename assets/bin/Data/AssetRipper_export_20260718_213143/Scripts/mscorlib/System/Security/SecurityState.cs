namespace System.Security
{
	public abstract class SecurityState
	{
		public abstract void EnsureState();

		public bool IsStateAvailable()
		{
			AppDomainManager domainManager = AppDomain.CurrentDomain.DomainManager;
			if (domainManager == null)
			{
				return false;
			}
			return domainManager.CheckSecuritySettings(this);
		}
	}
}
