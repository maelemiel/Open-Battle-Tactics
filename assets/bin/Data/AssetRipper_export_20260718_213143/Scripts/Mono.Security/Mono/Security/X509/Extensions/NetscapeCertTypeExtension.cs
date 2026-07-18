using System;
using System.Globalization;
using System.Text;

namespace Mono.Security.X509.Extensions
{
	public class NetscapeCertTypeExtension : X509Extension
	{
		[Flags]
		public enum CertTypes
		{
			SslClient = 0x80,
			SslServer = 0x40,
			Smime = 0x20,
			ObjectSigning = 0x10,
			SslCA = 4,
			SmimeCA = 2,
			ObjectSigningCA = 1
		}

		private int ctbits;

		public override string Name
		{
			get
			{
				return "NetscapeCertType";
			}
		}

		public NetscapeCertTypeExtension()
		{
			extnOid = "2.16.840.1.113730.1.1";
		}

		public NetscapeCertTypeExtension(ASN1 asn1)
			: base(asn1)
		{
		}

		public NetscapeCertTypeExtension(X509Extension extension)
			: base(extension)
		{
		}

		protected override void Decode()
		{
			ASN1 aSN = new ASN1(extnValue.Value);
			if (aSN.Tag != 3)
			{
				throw new ArgumentException("Invalid NetscapeCertType extension");
			}
			int num = 1;
			while (num < aSN.Value.Length)
			{
				ctbits = (ctbits << 8) + aSN.Value[num++];
			}
		}

		public bool Support(CertTypes usage)
		{
			int num = Convert.ToInt32(usage, CultureInfo.InvariantCulture);
			return (num & ctbits) == num;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (Support(CertTypes.SslClient))
			{
				stringBuilder.Append("SSL Client Authentication");
			}
			if (Support(CertTypes.SslServer))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" , ");
				}
				stringBuilder.Append("SSL Server Authentication");
			}
			if (Support(CertTypes.Smime))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" , ");
				}
				stringBuilder.Append("SMIME");
			}
			if (Support(CertTypes.ObjectSigning))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" , ");
				}
				stringBuilder.Append("Object Signing");
			}
			if (Support(CertTypes.SslCA))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" , ");
				}
				stringBuilder.Append("SSL CA");
			}
			if (Support(CertTypes.SmimeCA))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" , ");
				}
				stringBuilder.Append("SMIME CA");
			}
			if (Support(CertTypes.ObjectSigningCA))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" , ");
				}
				stringBuilder.Append("Object Signing CA");
			}
			stringBuilder.Append("(");
			stringBuilder.Append(ctbits.ToString("X2", CultureInfo.InvariantCulture));
			stringBuilder.Append(")");
			stringBuilder.Append(Environment.NewLine);
			return stringBuilder.ToString();
		}
	}
}
