using System.Security.Principal;

namespace System.Security.AccessControl
{
	public abstract class FileSystemSecurity : NativeObjectSecurity
	{
		public override Type AccessRightType
		{
			get
			{
				return typeof(FileSystemRights);
			}
		}

		public override Type AccessRuleType
		{
			get
			{
				return typeof(FileSystemAccessRule);
			}
		}

		public override Type AuditRuleType
		{
			get
			{
				return typeof(FileSystemAuditRule);
			}
		}

		internal FileSystemSecurity(bool isContainer)
			: base(isContainer, ResourceType.FileObject)
		{
		}

		internal FileSystemSecurity(bool isContainer, string name, AccessControlSections includeSections)
			: base(isContainer, ResourceType.FileObject, name, includeSections)
		{
		}

		[MonoTODO]
		public sealed override AccessRule AccessRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
		{
			return new FileSystemAccessRule(identityReference, (FileSystemRights)accessMask, inheritanceFlags, propagationFlags, type);
		}

		[MonoTODO]
		public void AddAccessRule(FileSystemAccessRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public bool RemoveAccessRule(FileSystemAccessRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void RemoveAccessRuleAll(FileSystemAccessRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void RemoveAccessRuleSpecific(FileSystemAccessRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void ResetAccessRule(FileSystemAccessRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void SetAccessRule(FileSystemAccessRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public sealed override AuditRule AuditRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
		{
			return new FileSystemAuditRule(identityReference, (FileSystemRights)accessMask, inheritanceFlags, propagationFlags, flags);
		}

		[MonoTODO]
		public void AddAuditRule(FileSystemAuditRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public bool RemoveAuditRule(FileSystemAuditRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void RemoveAuditRuleAll(FileSystemAuditRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void RemoveAuditRuleSpecific(FileSystemAuditRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void SetAuditRule(FileSystemAuditRule rule)
		{
			throw new NotImplementedException();
		}
	}
}
