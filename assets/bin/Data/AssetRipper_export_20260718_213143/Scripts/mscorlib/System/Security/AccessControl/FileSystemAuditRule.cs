using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class FileSystemAuditRule : AuditRule
	{
		private FileSystemRights rights;

		public FileSystemRights FileSystemRights
		{
			get
			{
				return rights;
			}
		}

		public FileSystemAuditRule(IdentityReference identity, FileSystemRights fileSystemRights, AuditFlags flags)
			: this(identity, fileSystemRights, InheritanceFlags.None, PropagationFlags.None, flags)
		{
		}

		public FileSystemAuditRule(string identity, FileSystemRights fileSystemRights, AuditFlags flags)
			: this(new SecurityIdentifier(identity), fileSystemRights, flags)
		{
		}

		public FileSystemAuditRule(IdentityReference identity, FileSystemRights fileSystemRights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
			: base(identity, 0, false, inheritanceFlags, propagationFlags, flags)
		{
			rights = fileSystemRights;
		}

		public FileSystemAuditRule(string identity, FileSystemRights fileSystemRights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
			: this(new SecurityIdentifier(identity), fileSystemRights, inheritanceFlags, propagationFlags, flags)
		{
		}
	}
}
