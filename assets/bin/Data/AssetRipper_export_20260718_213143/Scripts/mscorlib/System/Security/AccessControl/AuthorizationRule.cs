using System.Security.Principal;

namespace System.Security.AccessControl
{
	public abstract class AuthorizationRule
	{
		private IdentityReference identity;

		private int accessMask;

		private bool isInherited;

		private InheritanceFlags inheritanceFlags;

		private PropagationFlags propagationFlags;

		public IdentityReference IdentityReference
		{
			get
			{
				return identity;
			}
		}

		public InheritanceFlags InheritanceFlags
		{
			get
			{
				return inheritanceFlags;
			}
		}

		public bool IsInherited
		{
			get
			{
				return isInherited;
			}
		}

		public PropagationFlags PropagationFlags
		{
			get
			{
				return propagationFlags;
			}
		}

		protected internal int AccessMask
		{
			get
			{
				return accessMask;
			}
		}

		internal AuthorizationRule()
		{
		}

		protected internal AuthorizationRule(IdentityReference identity, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags)
		{
			if (!(identity is SecurityIdentifier))
			{
				throw new ArgumentException("identity");
			}
			if (accessMask == 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.identity = identity;
			this.accessMask = accessMask;
			this.isInherited = isInherited;
			this.inheritanceFlags = inheritanceFlags;
			this.propagationFlags = propagationFlags;
		}
	}
}
