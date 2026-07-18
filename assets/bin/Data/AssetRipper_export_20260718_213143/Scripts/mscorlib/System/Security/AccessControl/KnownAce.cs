using System.Security.Principal;

namespace System.Security.AccessControl
{
	public abstract class KnownAce : GenericAce
	{
		private int access_mask;

		private SecurityIdentifier identifier;

		public int AccessMask
		{
			get
			{
				return access_mask;
			}
			set
			{
				access_mask = value;
			}
		}

		public SecurityIdentifier SecurityIdentifier
		{
			get
			{
				return identifier;
			}
			set
			{
				identifier = value;
			}
		}

		internal KnownAce(InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags)
			: base(inheritanceFlags, propagationFlags)
		{
		}
	}
}
