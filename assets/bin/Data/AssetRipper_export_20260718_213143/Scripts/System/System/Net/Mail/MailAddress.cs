using System.Text;

namespace System.Net.Mail
{
	public class MailAddress
	{
		private string address;

		private string displayName;

		public string Address
		{
			get
			{
				return address;
			}
		}

		public string DisplayName
		{
			get
			{
				if (displayName == null)
				{
					return string.Empty;
				}
				return displayName;
			}
		}

		public string Host
		{
			get
			{
				return Address.Substring(address.IndexOf("@") + 1);
			}
		}

		public string User
		{
			get
			{
				return Address.Substring(0, address.IndexOf("@"));
			}
		}

		public MailAddress(string address)
			: this(address, null)
		{
		}

		public MailAddress(string address, string displayName)
			: this(address, displayName, Encoding.Default)
		{
		}

		public MailAddress(string address, string displayName, Encoding displayNameEncoding)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			int num = address.IndexOf('"');
			if (num == 0)
			{
				int num2 = address.IndexOf('"', num + 1);
				if (num2 == -1)
				{
					throw CreateFormatException();
				}
				this.displayName = address.Substring(num + 1, num2 - 1).Trim();
				address = address.Substring(num2 + 1);
			}
			int num3 = address.IndexOf('<');
			if (num3 != -1)
			{
				if (num3 + 1 >= address.Length)
				{
					throw CreateFormatException();
				}
				int num4 = address.IndexOf('>', num3 + 1);
				if (num4 == -1)
				{
					throw CreateFormatException();
				}
				if (this.displayName == null)
				{
					this.displayName = address.Substring(0, num3).Trim();
				}
				address = address.Substring(++num3, num4 - num3);
			}
			if (displayName != null)
			{
				this.displayName = displayName.Trim();
			}
			this.address = address.Trim();
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as MailAddress);
		}

		private bool Equals(MailAddress other)
		{
			return other != null && Address == other.Address;
		}

		public override int GetHashCode()
		{
			return address.GetHashCode();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (DisplayName != null && DisplayName.Length > 0)
			{
				stringBuilder.Append("\"");
				stringBuilder.Append(DisplayName);
				stringBuilder.Append("\"");
				stringBuilder.Append(" ");
				stringBuilder.Append("<");
				stringBuilder.Append(Address);
				stringBuilder.Append(">");
			}
			else
			{
				stringBuilder.Append(Address);
			}
			return stringBuilder.ToString();
		}

		private static FormatException CreateFormatException()
		{
			return new FormatException("The specified string is not in the form required for an e-mail address.");
		}
	}
}
