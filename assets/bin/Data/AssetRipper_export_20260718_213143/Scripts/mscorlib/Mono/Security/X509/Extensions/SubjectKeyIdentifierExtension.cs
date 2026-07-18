using System;
using System.Globalization;
using System.Text;

namespace Mono.Security.X509.Extensions
{
	internal class SubjectKeyIdentifierExtension : X509Extension
	{
		private byte[] ski;

		public override string Name
		{
			get
			{
				return "Subject Key Identifier";
			}
		}

		public byte[] Identifier
		{
			get
			{
				if (ski == null)
				{
					return null;
				}
				return (byte[])ski.Clone();
			}
		}

		public SubjectKeyIdentifierExtension()
		{
			extnOid = "2.5.29.14";
		}

		public SubjectKeyIdentifierExtension(ASN1 asn1)
			: base(asn1)
		{
		}

		public SubjectKeyIdentifierExtension(X509Extension extension)
			: base(extension)
		{
		}

		protected override void Decode()
		{
			ASN1 aSN = new ASN1(extnValue.Value);
			if (aSN.Tag != 4)
			{
				throw new ArgumentException("Invalid SubjectKeyIdentifier extension");
			}
			ski = aSN.Value;
		}

		public override string ToString()
		{
			if (ski == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < ski.Length; i++)
			{
				stringBuilder.Append(ski[i].ToString("X2", CultureInfo.InvariantCulture));
				if (i % 2 == 1)
				{
					stringBuilder.Append(" ");
				}
			}
			return stringBuilder.ToString();
		}
	}
}
