using System;
using System.Globalization;
using System.Text;

namespace Mono.Security.X509.Extensions
{
	public class KeyUsageExtension : X509Extension
	{
		private int kubits;

		public KeyUsages KeyUsage
		{
			get
			{
				return (KeyUsages)kubits;
			}
			set
			{
				kubits = Convert.ToInt32(value, CultureInfo.InvariantCulture);
			}
		}

		public override string Name
		{
			get
			{
				return "Key Usage";
			}
		}

		public KeyUsageExtension(ASN1 asn1)
			: base(asn1)
		{
		}

		public KeyUsageExtension(X509Extension extension)
			: base(extension)
		{
		}

		public KeyUsageExtension()
		{
			extnOid = "2.5.29.15";
		}

		protected override void Decode()
		{
			ASN1 aSN = new ASN1(extnValue.Value);
			if (aSN.Tag != 3)
			{
				throw new ArgumentException("Invalid KeyUsage extension");
			}
			int num = 1;
			while (num < aSN.Value.Length)
			{
				kubits = (kubits << 8) + aSN.Value[num++];
			}
		}

		protected override void Encode()
		{
			extnValue = new ASN1(4);
			ushort num = (ushort)kubits;
			byte b = 16;
			if (num > 0)
			{
				b = 15;
				while (b > 0 && (num & 0x8000) != 32768)
				{
					num <<= 1;
					b--;
				}
				if (kubits > 255)
				{
					b -= 8;
					extnValue.Add(new ASN1(3, new byte[3]
					{
						b,
						(byte)kubits,
						(byte)(kubits >> 8)
					}));
				}
				else
				{
					extnValue.Add(new ASN1(3, new byte[2]
					{
						b,
						(byte)kubits
					}));
				}
			}
			else
			{
				extnValue.Add(new ASN1(3, new byte[2] { 7, 0 }));
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
			return stringBuilder.ToString();
		}
	}
}
