using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class FileSystemAccessRule : AccessRule
	{
		private FileSystemRights rights;

		public FileSystemRights FileSystemRights
		{
			get
			{
				return rights;
			}
		}

		public FileSystemAccessRule(IdentityReference identity, FileSystemRights fileSystemRights, AccessControlType type)
			: this(identity, fileSystemRights, InheritanceFlags.None, PropagationFlags.None, type)
		{
		}

		public FileSystemAccessRule(string identity, FileSystemRights fileSystemRights, AccessControlType type)
			: this(new SecurityIdentifier(identity), fileSystemRights, InheritanceFlags.None, PropagationFlags.None, type)
		{
		}

		public FileSystemAccessRule(IdentityReference identity, FileSystemRights fileSystemRights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
			: base(identity, (int)fileSystemRights, false, inheritanceFlags, propagationFlags, type)
		{
			rights = fileSystemRights;
		}

		public FileSystemAccessRule(string identity, FileSystemRights fileSystemRights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
			: this(new SecurityIdentifier(identity), fileSystemRights, inheritanceFlags, propagationFlags, type)
		{
		}
	}
}
