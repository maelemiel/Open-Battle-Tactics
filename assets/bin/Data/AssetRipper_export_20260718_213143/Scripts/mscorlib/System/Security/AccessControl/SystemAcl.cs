using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class SystemAcl : CommonAcl
	{
		public SystemAcl(bool isContainer, bool isDS, int capacity)
			: this(isContainer, isDS, 0, capacity)
		{
		}

		public SystemAcl(bool isContainer, bool isDS, RawAcl rawAcl)
			: this(isContainer, isDS, 0)
		{
		}

		public SystemAcl(bool isContainer, bool isDS, byte revision, int capacity)
			: base(isContainer, isDS, revision, capacity)
		{
		}

		public void AddAudit(AuditFlags auditFlags, SecurityIdentifier sid, int accessMask, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags)
		{
			throw new NotImplementedException();
		}

		public void AddAudit(AuditFlags auditFlags, SecurityIdentifier sid, int accessMask, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, ObjectAceFlags objectFlags, Guid objectType, Guid inheritedObjectType)
		{
			throw new NotImplementedException();
		}

		public bool RemoveAudit(AuditFlags auditFlags, SecurityIdentifier sid, int accessMask, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags)
		{
			throw new NotImplementedException();
		}

		public bool RemoveAudit(AuditFlags auditFlags, SecurityIdentifier sid, int accessMask, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, ObjectAceFlags objectFlags, Guid objectType, Guid inheritedObjectType)
		{
			throw new NotImplementedException();
		}

		public void RemoveAuditSpecific(AuditFlags auditFlags, SecurityIdentifier sid, int accessMask, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags)
		{
			throw new NotImplementedException();
		}

		public void RemoveAuditSpecific(AuditFlags auditFlags, SecurityIdentifier sid, int accessMask, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, ObjectAceFlags objectFlags, Guid objectType, Guid inheritedObjectType)
		{
			throw new NotImplementedException();
		}

		public void SetAudit(AuditFlags auditFlags, SecurityIdentifier sid, int accessMask, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags)
		{
			throw new NotImplementedException();
		}

		public void SetAudit(AuditFlags auditFlags, SecurityIdentifier sid, int accessMask, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, ObjectAceFlags objectFlags, Guid objectType, Guid inheritedObjectType)
		{
			throw new NotImplementedException();
		}
	}
}
