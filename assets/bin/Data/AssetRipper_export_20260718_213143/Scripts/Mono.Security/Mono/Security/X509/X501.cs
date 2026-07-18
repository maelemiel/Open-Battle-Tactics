using System;
using System.Globalization;
using System.Text;
using Mono.Security.Cryptography;

namespace Mono.Security.X509
{
	public sealed class X501
	{
		private static byte[] countryName = new byte[3] { 85, 4, 6 };

		private static byte[] organizationName = new byte[3] { 85, 4, 10 };

		private static byte[] organizationalUnitName = new byte[3] { 85, 4, 11 };

		private static byte[] commonName = new byte[3] { 85, 4, 3 };

		private static byte[] localityName = new byte[3] { 85, 4, 7 };

		private static byte[] stateOrProvinceName = new byte[3] { 85, 4, 8 };

		private static byte[] streetAddress = new byte[3] { 85, 4, 9 };

		private static byte[] domainComponent = new byte[10] { 9, 146, 38, 137, 147, 242, 44, 100, 1, 25 };

		private static byte[] userid = new byte[10] { 9, 146, 38, 137, 147, 242, 44, 100, 1, 1 };

		private static byte[] email = new byte[9] { 42, 134, 72, 134, 247, 13, 1, 9, 1 };

		private static byte[] dnQualifier = new byte[3] { 85, 4, 46 };

		private static byte[] title = new byte[3] { 85, 4, 12 };

		private static byte[] surname = new byte[3] { 85, 4, 4 };

		private static byte[] givenName = new byte[3] { 85, 4, 42 };

		private static byte[] initial = new byte[3] { 85, 4, 43 };

		private X501()
		{
		}

		public static string ToString(ASN1 seq)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < seq.Count; i++)
			{
				ASN1 entry = seq[i];
				AppendEntry(stringBuilder, entry, true);
				if (i < seq.Count - 1)
				{
					stringBuilder.Append(", ");
				}
			}
			return stringBuilder.ToString();
		}

		public static string ToString(ASN1 seq, bool reversed, string separator, bool quotes)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (reversed)
			{
				for (int num = seq.Count - 1; num >= 0; num--)
				{
					ASN1 entry = seq[num];
					AppendEntry(stringBuilder, entry, quotes);
					if (num > 0)
					{
						stringBuilder.Append(separator);
					}
				}
			}
			else
			{
				for (int i = 0; i < seq.Count; i++)
				{
					ASN1 entry2 = seq[i];
					AppendEntry(stringBuilder, entry2, quotes);
					if (i < seq.Count - 1)
					{
						stringBuilder.Append(separator);
					}
				}
			}
			return stringBuilder.ToString();
		}

		private static void AppendEntry(StringBuilder sb, ASN1 entry, bool quotes)
		{
			for (int i = 0; i < entry.Count; i++)
			{
				ASN1 aSN = entry[i];
				ASN1 aSN2 = aSN[1];
				if (aSN2 == null)
				{
					continue;
				}
				ASN1 aSN3 = aSN[0];
				if (aSN3 == null)
				{
					continue;
				}
				if (aSN3.CompareValue(countryName))
				{
					sb.Append("C=");
				}
				else if (aSN3.CompareValue(organizationName))
				{
					sb.Append("O=");
				}
				else if (aSN3.CompareValue(organizationalUnitName))
				{
					sb.Append("OU=");
				}
				else if (aSN3.CompareValue(commonName))
				{
					sb.Append("CN=");
				}
				else if (aSN3.CompareValue(localityName))
				{
					sb.Append("L=");
				}
				else if (aSN3.CompareValue(stateOrProvinceName))
				{
					sb.Append("S=");
				}
				else if (aSN3.CompareValue(streetAddress))
				{
					sb.Append("STREET=");
				}
				else if (aSN3.CompareValue(domainComponent))
				{
					sb.Append("DC=");
				}
				else if (aSN3.CompareValue(userid))
				{
					sb.Append("UID=");
				}
				else if (aSN3.CompareValue(email))
				{
					sb.Append("E=");
				}
				else if (aSN3.CompareValue(dnQualifier))
				{
					sb.Append("dnQualifier=");
				}
				else if (aSN3.CompareValue(title))
				{
					sb.Append("T=");
				}
				else if (aSN3.CompareValue(surname))
				{
					sb.Append("SN=");
				}
				else if (aSN3.CompareValue(givenName))
				{
					sb.Append("G=");
				}
				else if (aSN3.CompareValue(initial))
				{
					sb.Append("I=");
				}
				else
				{
					sb.Append("OID.");
					sb.Append(ASN1Convert.ToOid(aSN3));
					sb.Append("=");
				}
				string text = null;
				if (aSN2.Tag == 30)
				{
					StringBuilder stringBuilder = new StringBuilder();
					for (int j = 1; j < aSN2.Value.Length; j += 2)
					{
						stringBuilder.Append((char)aSN2.Value[j]);
					}
					text = stringBuilder.ToString();
				}
				else
				{
					text = ((aSN2.Tag != 20) ? Encoding.UTF8.GetString(aSN2.Value) : Encoding.UTF7.GetString(aSN2.Value));
					char[] anyOf = new char[7] { ',', '+', '"', '\\', '<', '>', ';' };
					if (quotes && (text.IndexOfAny(anyOf, 0, text.Length) > 0 || text.StartsWith(" ") || text.EndsWith(" ")))
					{
						text = "\"" + text + "\"";
					}
				}
				sb.Append(text);
				if (i < entry.Count - 1)
				{
					sb.Append(", ");
				}
			}
		}

		private static X520.AttributeTypeAndValue GetAttributeFromOid(string attributeType)
		{
			string text = attributeType.ToUpper(CultureInfo.InvariantCulture).Trim();
			switch (text)
			{
			case "C":
				return new X520.CountryName();
			case "O":
				return new X520.OrganizationName();
			case "OU":
				return new X520.OrganizationalUnitName();
			case "CN":
				return new X520.CommonName();
			case "L":
				return new X520.LocalityName();
			case "S":
			case "ST":
				return new X520.StateOrProvinceName();
			case "E":
				return new X520.EmailAddress();
			case "DC":
				return new X520.DomainComponent();
			case "UID":
				return new X520.UserId();
			case "DNQUALIFIER":
				return new X520.DnQualifier();
			case "T":
				return new X520.Title();
			case "SN":
				return new X520.Surname();
			case "G":
				return new X520.GivenName();
			case "I":
				return new X520.Initial();
			default:
				if (text.StartsWith("OID."))
				{
					return new X520.Oid(text.Substring(4));
				}
				if (IsOid(text))
				{
					return new X520.Oid(text);
				}
				return null;
			}
		}

		private static bool IsOid(string oid)
		{
			try
			{
				ASN1 aSN = ASN1Convert.FromOid(oid);
				return aSN.Tag == 6;
			}
			catch
			{
				return false;
			}
		}

		private static X520.AttributeTypeAndValue ReadAttribute(string value, ref int pos)
		{
			while (value[pos] == ' ' && pos < value.Length)
			{
				pos++;
			}
			int num = value.IndexOf('=', pos);
			if (num == -1)
			{
				string text = global::Locale.GetText("No attribute found.");
				throw new FormatException(text);
			}
			string text2 = value.Substring(pos, num - pos);
			X520.AttributeTypeAndValue attributeFromOid = GetAttributeFromOid(text2);
			if (attributeFromOid == null)
			{
				string text3 = global::Locale.GetText("Unknown attribute '{0}'.");
				throw new FormatException(string.Format(text3, text2));
			}
			pos = num + 1;
			return attributeFromOid;
		}

		private static bool IsHex(char c)
		{
			if (char.IsDigit(c))
			{
				return true;
			}
			char c2 = char.ToUpper(c, CultureInfo.InvariantCulture);
			return c2 >= 'A' && c2 <= 'F';
		}

		private static string ReadHex(string value, ref int pos)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(value[pos++]);
			stringBuilder.Append(value[pos]);
			if (pos < value.Length - 4 && value[pos + 1] == '\\' && IsHex(value[pos + 2]))
			{
				pos += 2;
				stringBuilder.Append(value[pos++]);
				stringBuilder.Append(value[pos]);
			}
			byte[] bytes = CryptoConvert.FromHex(stringBuilder.ToString());
			return Encoding.UTF8.GetString(bytes);
		}

		private static int ReadEscaped(StringBuilder sb, string value, int pos)
		{
			switch (value[pos])
			{
			case '"':
			case '#':
			case '+':
			case ',':
			case ';':
			case '<':
			case '=':
			case '>':
			case '\\':
				sb.Append(value[pos]);
				return pos;
			default:
				if (pos >= value.Length - 2)
				{
					string text = global::Locale.GetText("Malformed escaped value '{0}'.");
					throw new FormatException(string.Format(text, value.Substring(pos)));
				}
				sb.Append(ReadHex(value, ref pos));
				return pos;
			}
		}

		private static int ReadQuoted(StringBuilder sb, string value, int pos)
		{
			int startIndex = pos;
			while (pos <= value.Length)
			{
				switch (value[pos])
				{
				case '"':
					return pos;
				case '\\':
					return ReadEscaped(sb, value, pos);
				}
				sb.Append(value[pos]);
				pos++;
			}
			string text = global::Locale.GetText("Malformed quoted value '{0}'.");
			throw new FormatException(string.Format(text, value.Substring(startIndex)));
		}

		private static string ReadValue(string value, ref int pos)
		{
			int startIndex = pos;
			StringBuilder stringBuilder = new StringBuilder();
			while (pos < value.Length)
			{
				switch (value[pos])
				{
				case '\\':
					pos = ReadEscaped(stringBuilder, value, ++pos);
					break;
				case '"':
					pos = ReadQuoted(stringBuilder, value, ++pos);
					break;
				case ';':
				case '<':
				case '=':
				case '>':
				{
					string text = global::Locale.GetText("Malformed value '{0}' contains '{1}' outside quotes.");
					throw new FormatException(string.Format(text, value.Substring(startIndex), value[pos]));
				}
				case '#':
				case '+':
					throw new NotImplementedException();
				case ',':
					pos++;
					return stringBuilder.ToString();
				default:
					stringBuilder.Append(value[pos]);
					break;
				}
				pos++;
			}
			return stringBuilder.ToString();
		}

		public static ASN1 FromString(string rdn)
		{
			if (rdn == null)
			{
				throw new ArgumentNullException("rdn");
			}
			int pos = 0;
			ASN1 aSN = new ASN1(48);
			while (pos < rdn.Length)
			{
				X520.AttributeTypeAndValue attributeTypeAndValue = ReadAttribute(rdn, ref pos);
				attributeTypeAndValue.Value = ReadValue(rdn, ref pos);
				ASN1 aSN2 = new ASN1(49);
				aSN2.Add(attributeTypeAndValue.GetASN1());
				aSN.Add(aSN2);
			}
			return aSN;
		}
	}
}
