using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class MutexAccessRule : AccessRule
	{
		private MutexRights rights;

		public MutexRights MutexRights
		{
			get
			{
				return rights;
			}
		}

		public MutexAccessRule(IdentityReference identity, MutexRights eventRights, AccessControlType type)
			: base(identity, 0, false, InheritanceFlags.None, PropagationFlags.None, type)
		{
			rights = eventRights;
		}

		public MutexAccessRule(string identity, MutexRights eventRights, AccessControlType type)
			: this(new SecurityIdentifier(identity), eventRights, type)
		{
		}
	}
}
