using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class RegistryAuditRule : AuditRule
	{
		private RegistryRights rights;

		public RegistryRights RegistryRights
		{
			get
			{
				return rights;
			}
		}

		public RegistryAuditRule(IdentityReference identity, RegistryRights registryRights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
			: base(identity, 0, false, inheritanceFlags, propagationFlags, flags)
		{
			rights = registryRights;
		}

		public RegistryAuditRule(string identity, RegistryRights registryRights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
			: this(new SecurityIdentifier(identity), registryRights, inheritanceFlags, propagationFlags, flags)
		{
		}
	}
}
