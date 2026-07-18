using System.Runtime.InteropServices;

namespace System.Security.Principal
{
	[ComVisible(false)]
	[MonoTODO("not implemented")]
	public sealed class SecurityIdentifier : IdentityReference, IComparable<SecurityIdentifier>
	{
		private string _value;

		public static readonly int MaxBinaryLength;

		public static readonly int MinBinaryLength;

		public SecurityIdentifier AccountDomainSid
		{
			get
			{
				throw new ArgumentNullException("AccountDomainSid");
			}
		}

		public int BinaryLength
		{
			get
			{
				return -1;
			}
		}

		public override string Value
		{
			get
			{
				return _value;
			}
		}

		public SecurityIdentifier(string sddlForm)
		{
			if (sddlForm == null)
			{
				throw new ArgumentNullException("sddlForm");
			}
			_value = sddlForm.ToUpperInvariant();
		}

		public SecurityIdentifier(byte[] binaryForm, int offset)
		{
			if (binaryForm == null)
			{
				throw new ArgumentNullException("binaryForm");
			}
			if (offset < 0 || offset > binaryForm.Length - 1)
			{
				throw new ArgumentException("offset");
			}
			throw new NotImplementedException();
		}

		public SecurityIdentifier(IntPtr binaryForm)
		{
			throw new NotImplementedException();
		}

		public SecurityIdentifier(WellKnownSidType sidType, SecurityIdentifier domainSid)
		{
			switch (sidType)
			{
			case WellKnownSidType.LogonIdsSid:
				throw new ArgumentException("sidType");
			case WellKnownSidType.AccountAdministratorSid:
			case WellKnownSidType.AccountGuestSid:
			case WellKnownSidType.AccountKrbtgtSid:
			case WellKnownSidType.AccountDomainAdminsSid:
			case WellKnownSidType.AccountDomainUsersSid:
			case WellKnownSidType.AccountDomainGuestsSid:
			case WellKnownSidType.AccountComputersSid:
			case WellKnownSidType.AccountControllersSid:
			case WellKnownSidType.AccountCertAdminsSid:
			case WellKnownSidType.AccountSchemaAdminsSid:
			case WellKnownSidType.AccountEnterpriseAdminsSid:
			case WellKnownSidType.AccountPolicyAdminsSid:
			case WellKnownSidType.AccountRasAndIasServersSid:
				if (domainSid == null)
				{
					throw new ArgumentNullException("domainSid");
				}
				break;
			}
		}

		public int CompareTo(SecurityIdentifier sid)
		{
			return Value.CompareTo(sid.Value);
		}

		public override bool Equals(object o)
		{
			return Equals(o as SecurityIdentifier);
		}

		public bool Equals(SecurityIdentifier sid)
		{
			if (sid == null)
			{
				return false;
			}
			return sid.Value == Value;
		}

		public void GetBinaryForm(byte[] binaryForm, int offset)
		{
			if (binaryForm == null)
			{
				throw new ArgumentNullException("binaryForm");
			}
			if (offset < 0 || offset > binaryForm.Length - 1 - BinaryLength)
			{
				throw new ArgumentException("offset");
			}
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public bool IsAccountSid()
		{
			throw new NotImplementedException();
		}

		public bool IsEqualDomainSid(SecurityIdentifier sid)
		{
			throw new NotImplementedException();
		}

		public override bool IsValidTargetType(Type targetType)
		{
			if (targetType == typeof(SecurityIdentifier))
			{
				return true;
			}
			if (targetType == typeof(NTAccount))
			{
				return true;
			}
			return false;
		}

		public bool IsWellKnown(WellKnownSidType type)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return Value;
		}

		public override IdentityReference Translate(Type targetType)
		{
			if (targetType == typeof(SecurityIdentifier))
			{
				return this;
			}
			return null;
		}

		public static bool operator ==(SecurityIdentifier left, SecurityIdentifier right)
		{
			if ((object)left == null)
			{
				return (object)right == null;
			}
			if ((object)right == null)
			{
				return false;
			}
			return left.Value == right.Value;
		}

		public static bool operator !=(SecurityIdentifier left, SecurityIdentifier right)
		{
			if ((object)left == null)
			{
				return (object)right != null;
			}
			if ((object)right == null)
			{
				return true;
			}
			return left.Value != right.Value;
		}
	}
}
