using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class RawSecurityDescriptor : GenericSecurityDescriptor
	{
		public override ControlFlags ControlFlags
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public RawAcl DiscretionaryAcl
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public override SecurityIdentifier Group
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public override SecurityIdentifier Owner
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public byte ResourceManagerControl
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public RawAcl SystemAcl
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public RawSecurityDescriptor(string sddlForm)
		{
		}

		public RawSecurityDescriptor(byte[] binaryForm, int offset)
		{
		}

		public RawSecurityDescriptor(ControlFlags flags, SecurityIdentifier owner, SecurityIdentifier group, RawAcl systemAcl, RawAcl discretionaryAcl)
		{
		}

		public void SetFlags(ControlFlags flags)
		{
			throw new NotImplementedException();
		}
	}
}
