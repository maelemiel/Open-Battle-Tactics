using System.Security.Principal;

namespace System.Security.AccessControl
{
	public abstract class AuditRule : AuthorizationRule
	{
		private AuditFlags auditFlags;

		public AuditFlags AuditFlags
		{
			get
			{
				return auditFlags;
			}
		}

		protected AuditRule(IdentityReference identity, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags auditFlags)
			: base(identity, accessMask, isInherited, inheritanceFlags, propagationFlags)
		{
			if (!(identity is SecurityIdentifier))
			{
				throw new ArgumentException("identity");
			}
			if (accessMask == 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.auditFlags = auditFlags;
		}
	}
}
