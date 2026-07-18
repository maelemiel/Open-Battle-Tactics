using System.Security.Principal;

namespace System.Security.AccessControl
{
	public abstract class GenericSecurityDescriptor
	{
		public int BinaryLength
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public abstract ControlFlags ControlFlags { get; }

		public abstract SecurityIdentifier Group { get; set; }

		public abstract SecurityIdentifier Owner { get; set; }

		public static byte Revision
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void GetBinaryForm(byte[] binaryForm, int offset)
		{
			throw new NotImplementedException();
		}

		public string GetSddlForm(AccessControlSections includeSections)
		{
			throw new NotImplementedException();
		}

		public static bool IsSddlConversionSupported()
		{
			throw new NotImplementedException();
		}
	}
}
