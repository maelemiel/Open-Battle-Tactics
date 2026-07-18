using System.Security.Principal;

namespace System.Security.AccessControl
{
	public abstract class AccessRule : AuthorizationRule
	{
		private AccessControlType type;

		public AccessControlType AccessControlType
		{
			get
			{
				return type;
			}
		}

		protected AccessRule(IdentityReference identity, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
			: base(identity, accessMask, isInherited, inheritanceFlags, propagationFlags)
		{
			if (!(identity is SecurityIdentifier))
			{
				throw new ArgumentException("identity");
			}
			if (type < AccessControlType.Allow || type > AccessControlType.Deny)
			{
				throw new ArgumentException("type");
			}
			if (accessMask == 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.type = type;
		}
	}
}
