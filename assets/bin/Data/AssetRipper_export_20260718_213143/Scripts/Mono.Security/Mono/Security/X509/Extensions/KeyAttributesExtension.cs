using System;
using System.Globalization;
using System.Text;

namespace Mono.Security.X509.Extensions
{
	public class KeyAttributesExtension : X509Extension
	{
		private byte[] keyId;

		private int kubits;

		private DateTime notBefore;

		private DateTime notAfter;

		public byte[] KeyIdentifier
		{
			get
			{
				if (keyId == null)
				{
					return null;
				}
				return (byte[])keyId.Clone();
			}
		}

		public override string Name
		{
			get
			{
				return "Key Attributes";
			}
		}

		public DateTime NotAfter
		{
			get
			{
				return notAfter;
			}
		}

		public DateTime NotBefore
		{
			get
			{
				return notBefore;
			}
		}

		public KeyAttributesExtension()
		{
			extnOid = "2.5.29.2";
		}

		public KeyAttributesExtension(ASN1 asn1)
			: base(asn1)
		{
		}

		public KeyAttributesExtension(X509Extension extension)
			: base(extension)
		{
		}

		protected override void Decode()
		{
			ASN1 aSN = new ASN1(extnValue.Value);
			if (aSN.Tag != 48)
			{
				throw new ArgumentException("Invalid KeyAttributesExtension extension");
			}
			int num = 0;
			if (num < aSN.Count)
			{
				ASN1 aSN2 = aSN[num];
				if (aSN2.Tag == 4)
				{
					num++;
					keyId = aSN2.Value;
				}
			}
			if (num < aSN.Count)
			{
				ASN1 aSN3 = aSN[num];
				if (aSN3.Tag == 3)
				{
					num++;
					int num2 = 1;
					while (num2 < aSN3.Value.Length)
					{
						kubits = (kubits << 8) + aSN3.Value[num2++];
					}
				}
			}
			if (num >= aSN.Count)
			{
				return;
			}
			ASN1 aSN4 = aSN[num];
			if (aSN4.Tag != 48)
			{
				return;
			}
			int num3 = 0;
			if (num3 < aSN4.Count)
			{
				ASN1 aSN5 = aSN4[num3];
				if (aSN5.Tag == 129)
				{
					num3++;
					notBefore = ASN1Convert.ToDateTime(aSN5);
				}
			}
			if (num3 < aSN4.Count)
			{
				ASN1 aSN6 = aSN4[num3];
				if (aSN6.Tag == 130)
				{
					notAfter = ASN1Convert.ToDateTime(aSN6);
				}
			}
		}

		public bool Support(KeyUsages usage)
		{
			int num = Convert.ToInt32(usage, CultureInfo.InvariantCulture);
			return (num & kubits) == num;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (keyId != null)
			{
				stringBuilder.Append("KeyID=");
				for (int i = 0; i < keyId.Length; i++)
				{
					stringBuilder.Append(keyId[i].ToString("X2", CultureInfo.InvariantCulture));
					if (i % 2 == 1)
					{
						stringBuilder.Append(" ");
					}
				}
				stringBuilder.Append(Environment.NewLine);
			}
			if (kubits != 0)
			{
				stringBuilder.Append("Key Usage=");
				if (Support(KeyUsages.digitalSignature))
				{
					stringBuilder.Append("Digital Signature");
				}
				if (Support(KeyUsages.nonRepudiation))
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(" , ");
					}
					stringBuilder.Append("Non-Repudiation");
				}
				if (Support(KeyUsages.keyEncipherment))
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(" , ");
					}
					stringBuilder.Append("Key Encipherment");
				}
				if (Support(KeyUsages.dataEncipherment))
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(" , ");
					}
					stringBuilder.Append("Data Encipherment");
				}
				if (Support(KeyUsages.keyAgreement))
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(" , ");
					}
					stringBuilder.Append("Key Agreement");
				}
				if (Support(KeyUsages.keyCertSign))
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(" , ");
					}
					stringBuilder.Append("Certificate Signing");
				}
				if (Support(KeyUsages.cRLSign))
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(" , ");
					}
					stringBuilder.Append("CRL Signing");
				}
				if (Support(KeyUsages.encipherOnly))
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(" , ");
					}
					stringBuilder.Append("Encipher Only ");
				}
				if (Support(KeyUsages.decipherOnly))
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(" , ");
					}
					stringBuilder.Append("Decipher Only");
				}
				stringBuilder.Append("(");
				stringBuilder.Append(kubits.ToString("X2", CultureInfo.InvariantCulture));
				stringBuilder.Append(")");
				stringBuilder.Append(Environment.NewLine);
			}
			if (notBefore != DateTime.MinValue)
			{
				stringBuilder.Append("Not Before=");
				stringBuilder.Append(notBefore.ToString(CultureInfo.CurrentUICulture));
				stringBuilder.Append(Environment.NewLine);
			}
			if (notAfter != DateTime.MinValue)
			{
				stringBuilder.Append("Not After=");
				stringBuilder.Append(notAfter.ToString(CultureInfo.CurrentUICulture));
				stringBuilder.Append(Environment.NewLine);
			}
			return stringBuilder.ToString();
		}
	}
}
