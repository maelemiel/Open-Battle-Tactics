using System.Runtime.InteropServices;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	[ComVisible(false)]
	public sealed class SemaphoreAccessRule : AccessRule
	{
		private SemaphoreRights semaphoreRights;

		public SemaphoreRights SemaphoreRights
		{
			get
			{
				return semaphoreRights;
			}
		}

		public SemaphoreAccessRule(IdentityReference identity, SemaphoreRights semaphoreRights, AccessControlType type)
			: base(identity, 0, false, InheritanceFlags.None, PropagationFlags.None, type)
		{
			this.semaphoreRights = semaphoreRights;
		}

		public SemaphoreAccessRule(string identity, SemaphoreRights semaphoreRights, AccessControlType type)
			: base(null, 0, false, InheritanceFlags.None, PropagationFlags.None, type)
		{
			this.semaphoreRights = semaphoreRights;
		}
	}
}
