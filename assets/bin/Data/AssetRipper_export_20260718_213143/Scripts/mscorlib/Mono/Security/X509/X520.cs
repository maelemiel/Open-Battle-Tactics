using System;
using System.Text;

namespace Mono.Security.X509
{
	internal class X520
	{
		public abstract class AttributeTypeAndValue
		{
			private string oid;

			private string attrValue;

			private int upperBound;

			private byte encoding;

			public string Value
			{
				get
				{
					return attrValue;
				}
				set
				{
					if (attrValue != null && attrValue.Length > upperBound)
					{
						string text = Locale.GetText("Value length bigger than upperbound ({0}).");
						throw new FormatException(string.Format(text, upperBound));
					}
					attrValue = value;
				}
			}

			public ASN1 ASN1
			{
				get
				{
					return GetASN1();
				}
			}

			protected AttributeTypeAndValue(string oid, int upperBound)
			{
				this.oid = oid;
				this.upperBound = upperBound;
				encoding = byte.MaxValue;
			}

			protected AttributeTypeAndValue(string oid, int upperBound, byte encoding)
			{
				this.oid = oid;
				this.upperBound = upperBound;
				this.encoding = encoding;
			}

			internal ASN1 GetASN1(byte encoding)
			{
				byte b = encoding;
				if (b == byte.MaxValue)
				{
					b = SelectBestEncoding();
				}
				ASN1 aSN = new ASN1(48);
				aSN.Add(ASN1Convert.FromOid(oid));
				switch (b)
				{
				case 19:
					aSN.Add(new ASN1(19, Encoding.ASCII.GetBytes(attrValue)));
					break;
				case 22:
					aSN.Add(new ASN1(22, Encoding.ASCII.GetBytes(attrValue)));
					break;
				case 30:
					aSN.Add(new ASN1(30, Encoding.BigEndianUnicode.GetBytes(attrValue)));
					break;
				}
				return aSN;
			}

			internal ASN1 GetASN1()
			{
				return GetASN1(encoding);
			}

			public byte[] GetBytes(byte encoding)
			{
				return GetASN1(encoding).GetBytes();
			}

			public byte[] GetBytes()
			{
				return GetASN1().GetBytes();
			}

			private byte SelectBestEncoding()
			{
				string text = attrValue;
				foreach (char c in text)
				{
					char c2 = c;
					if (c2 == '@' || c2 == '_')
					{
						return 30;
					}
					if (c > '\u007f')
					{
						return 30;
					}
				}
				return 19;
			}
		}

		public class Name : AttributeTypeAndValue
		{
			public Name()
				: base("2.5.4.41", 32768)
			{
			}
		}

		public class CommonName : AttributeTypeAndValue
		{
			public CommonName()
				: base("2.5.4.3", 64)
			{
			}
		}

		public class SerialNumber : AttributeTypeAndValue
		{
			public SerialNumber()
				: base("2.5.4.5", 64, 19)
			{
			}
		}

		public class LocalityName : AttributeTypeAndValue
		{
			public LocalityName()
				: base("2.5.4.7", 128)
			{
			}
		}

		public class StateOrProvinceName : AttributeTypeAndValue
		{
			public StateOrProvinceName()
				: base("2.5.4.8", 128)
			{
			}
		}

		public class OrganizationName : AttributeTypeAndValue
		{
			public OrganizationName()
				: base("2.5.4.10", 64)
			{
			}
		}

		public class OrganizationalUnitName : AttributeTypeAndValue
		{
			public OrganizationalUnitName()
				: base("2.5.4.11", 64)
			{
			}
		}

		public class EmailAddress : AttributeTypeAndValue
		{
			public EmailAddress()
				: base("1.2.840.113549.1.9.1", 128, 22)
			{
			}
		}

		public class DomainComponent : AttributeTypeAndValue
		{
			public DomainComponent()
				: base("0.9.2342.19200300.100.1.25", int.MaxValue, 22)
			{
			}
		}

		public class UserId : AttributeTypeAndValue
		{
			public UserId()
				: base("0.9.2342.19200300.100.1.1", 256)
			{
			}
		}

		public class Oid : AttributeTypeAndValue
		{
			public Oid(string oid)
				: base(oid, int.MaxValue)
			{
			}
		}

		public class Title : AttributeTypeAndValue
		{
			public Title()
				: base("2.5.4.12", 64)
			{
			}
		}

		public class CountryName : AttributeTypeAndValue
		{
			public CountryName()
				: base("2.5.4.6", 2, 19)
			{
			}
		}

		public class DnQualifier : AttributeTypeAndValue
		{
			public DnQualifier()
				: base("2.5.4.46", 2, 19)
			{
			}
		}

		public class Surname : AttributeTypeAndValue
		{
			public Surname()
				: base("2.5.4.4", 32768)
			{
			}
		}

		public class GivenName : AttributeTypeAndValue
		{
			public GivenName()
				: base("2.5.4.42", 16)
			{
			}
		}

		public class Initial : AttributeTypeAndValue
		{
			public Initial()
				: base("2.5.4.43", 5)
			{
			}
		}
	}
}
