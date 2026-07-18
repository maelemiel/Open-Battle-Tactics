using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public abstract class IsolatedStoragePermissionAttribute : CodeAccessSecurityAttribute
	{
		private IsolatedStorageContainment usage_allowed;

		private long user_quota;

		public IsolatedStorageContainment UsageAllowed
		{
			get
			{
				return usage_allowed;
			}
			set
			{
				usage_allowed = value;
			}
		}

		public long UserQuota
		{
			get
			{
				return user_quota;
			}
			set
			{
				user_quota = value;
			}
		}

		protected IsolatedStoragePermissionAttribute(SecurityAction action)
			: base(action)
		{
		}
	}
}
