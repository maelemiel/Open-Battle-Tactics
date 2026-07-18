using System.Runtime.InteropServices;

namespace System.Security.Principal
{
	[ComVisible(false)]
	public sealed class NTAccount : IdentityReference
	{
		private string _value;

		public override string Value
		{
			get
			{
				return _value;
			}
		}

		public NTAccount(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException(Locale.GetText("Empty"), "name");
			}
			_value = name.ToUpper();
		}

		public NTAccount(string domainName, string accountName)
		{
			if (accountName == null)
			{
				throw new ArgumentNullException("accountName");
			}
			if (accountName.Length == 0)
			{
				throw new ArgumentException(Locale.GetText("Empty"), "accountName");
			}
			if (domainName == null)
			{
				_value = domainName.ToUpper();
			}
			else
			{
				_value = domainName.ToUpper() + "\\" + domainName.ToUpper();
			}
		}

		public override bool Equals(object o)
		{
			NTAccount nTAccount = o as NTAccount;
			if (nTAccount == null)
			{
				return false;
			}
			return nTAccount.Value == Value;
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public override bool IsValidTargetType(Type targetType)
		{
			if (targetType == typeof(NTAccount))
			{
				return true;
			}
			if (targetType == typeof(SecurityIdentifier))
			{
				return true;
			}
			return false;
		}

		public override string ToString()
		{
			return Value;
		}

		public override IdentityReference Translate(Type targetType)
		{
			if (targetType == typeof(NTAccount))
			{
				return this;
			}
			return null;
		}

		public static bool operator ==(NTAccount left, NTAccount right)
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

		public static bool operator !=(NTAccount left, NTAccount right)
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
