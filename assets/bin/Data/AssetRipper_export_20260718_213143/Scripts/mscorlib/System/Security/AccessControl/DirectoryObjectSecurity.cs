using System.Security.Principal;

namespace System.Security.AccessControl
{
	public abstract class DirectoryObjectSecurity : ObjectSecurity
	{
		protected DirectoryObjectSecurity()
			: base(false, true)
		{
		}

		protected DirectoryObjectSecurity(CommonSecurityDescriptor securityDescriptor)
			: base(securityDescriptor != null && securityDescriptor.IsContainer, true)
		{
			if (securityDescriptor == null)
			{
				throw new ArgumentNullException("securityDescriptor");
			}
		}

		public virtual AccessRule AccessRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type, Guid objectType, Guid inheritedObjectType)
		{
			throw new NotImplementedException();
		}

		public virtual AuditRule AuditRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags, Guid objectType, Guid inheritedObjectType)
		{
			throw new NotImplementedException();
		}

		public AuthorizationRuleCollection GetAccessRules(bool includeExplicit, bool includeInherited, Type targetType)
		{
			throw new NotImplementedException();
		}

		public AuthorizationRuleCollection GetAuditRules(bool includeExplicit, bool includeInherited, Type targetType)
		{
			throw new NotImplementedException();
		}

		protected void AddAccessRule(ObjectAccessRule rule)
		{
			throw new NotImplementedException();
		}

		protected void AddAuditRule(ObjectAuditRule rule)
		{
			throw new NotImplementedException();
		}

		protected override bool ModifyAccess(AccessControlModification modification, AccessRule rule, out bool modified)
		{
			throw new NotImplementedException();
		}

		protected override bool ModifyAudit(AccessControlModification modification, AuditRule rule, out bool modified)
		{
			throw new NotImplementedException();
		}

		protected bool RemoveAccessRule(ObjectAccessRule rule)
		{
			throw new NotImplementedException();
		}

		protected void RemoveAccessRuleAll(ObjectAccessRule rule)
		{
			throw new NotImplementedException();
		}

		protected void RemoveAccessRuleSpecific(ObjectAccessRule rule)
		{
			throw new NotImplementedException();
		}

		protected bool RemoveAuditRule(ObjectAuditRule rule)
		{
			throw new NotImplementedException();
		}

		protected void RemoveAuditRuleAll(ObjectAuditRule rule)
		{
			throw new NotImplementedException();
		}

		protected void RemoveAuditRuleSpecific(ObjectAuditRule rule)
		{
			throw new NotImplementedException();
		}

		protected void ResetAccessRule(ObjectAccessRule rule)
		{
			throw new NotImplementedException();
		}

		protected void SetAccessRule(ObjectAccessRule rule)
		{
			throw new NotImplementedException();
		}

		protected void SetAuditRule(ObjectAuditRule rule)
		{
			throw new NotImplementedException();
		}
	}
}
