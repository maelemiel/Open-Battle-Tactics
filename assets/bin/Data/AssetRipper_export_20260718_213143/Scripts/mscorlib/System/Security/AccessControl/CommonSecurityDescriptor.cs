using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class CommonSecurityDescriptor : GenericSecurityDescriptor
	{
		private bool isContainer;

		private bool isDS;

		private ControlFlags flags;

		private SecurityIdentifier owner;

		private SecurityIdentifier group;

		private SystemAcl systemAcl;

		private DiscretionaryAcl discretionaryAcl;

		public override ControlFlags ControlFlags
		{
			get
			{
				return flags;
			}
		}

		public DiscretionaryAcl DiscretionaryAcl
		{
			get
			{
				return discretionaryAcl;
			}
			set
			{
				if (value == null)
				{
				}
				discretionaryAcl = value;
			}
		}

		public override SecurityIdentifier Group
		{
			get
			{
				return group;
			}
			set
			{
				group = value;
			}
		}

		public bool IsContainer
		{
			get
			{
				return isContainer;
			}
		}

		public bool IsDiscretionaryAclCanonical
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool IsDS
		{
			get
			{
				return isDS;
			}
		}

		public bool IsSystemAclCanonical
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override SecurityIdentifier Owner
		{
			get
			{
				return owner;
			}
			set
			{
				owner = value;
			}
		}

		public SystemAcl SystemAcl
		{
			get
			{
				return systemAcl;
			}
			set
			{
				systemAcl = value;
			}
		}

		public CommonSecurityDescriptor(bool isContainer, bool isDS, RawSecurityDescriptor rawSecurityDescriptor)
		{
			throw new NotImplementedException();
		}

		public CommonSecurityDescriptor(bool isContainer, bool isDS, string sddlForm)
		{
			throw new NotImplementedException();
		}

		public CommonSecurityDescriptor(bool isContainer, bool isDS, byte[] binaryForm, int offset)
		{
			throw new NotImplementedException();
		}

		public CommonSecurityDescriptor(bool isContainer, bool isDS, ControlFlags flags, SecurityIdentifier owner, SecurityIdentifier group, SystemAcl systemAcl, DiscretionaryAcl discretionaryAcl)
		{
			this.isContainer = isContainer;
			this.isDS = isDS;
			this.flags = flags;
			this.owner = owner;
			this.group = group;
			this.systemAcl = systemAcl;
			this.discretionaryAcl = discretionaryAcl;
			throw new NotImplementedException();
		}

		public void PurgeAccessControl(SecurityIdentifier sid)
		{
			throw new NotImplementedException();
		}

		public void PurgeAudit(SecurityIdentifier sid)
		{
			throw new NotImplementedException();
		}

		public void SetDiscretionaryAclProtection(bool isProtected, bool preserveInheritance)
		{
			throw new NotImplementedException();
		}

		public void SetSystemAclProtection(bool isProtected, bool preserveInheritance)
		{
			throw new NotImplementedException();
		}
	}
}
