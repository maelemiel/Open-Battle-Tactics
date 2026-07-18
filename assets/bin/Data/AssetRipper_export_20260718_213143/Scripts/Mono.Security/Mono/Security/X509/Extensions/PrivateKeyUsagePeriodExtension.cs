using System;
using System.Globalization;
using System.Text;

namespace Mono.Security.X509.Extensions
{
	public class PrivateKeyUsagePeriodExtension : X509Extension
	{
		private DateTime notBefore;

		private DateTime notAfter;

		public override string Name
		{
			get
			{
				return "Private Key Usage Period";
			}
		}

		public PrivateKeyUsagePeriodExtension()
		{
			extnOid = "2.5.29.16";
		}

		public PrivateKeyUsagePeriodExtension(ASN1 asn1)
			: base(asn1)
		{
		}

		public PrivateKeyUsagePeriodExtension(X509Extension extension)
			: base(extension)
		{
		}

		protected override void Decode()
		{
			ASN1 aSN = new ASN1(extnValue.Value);
			if (aSN.Tag != 48)
			{
				throw new ArgumentException("Invalid PrivateKeyUsagePeriod extension");
			}
			for (int i = 0; i < aSN.Count; i++)
			{
				switch (aSN[i].Tag)
				{
				case 128:
					notBefore = ASN1Convert.ToDateTime(aSN[i]);
					break;
				case 129:
					notAfter = ASN1Convert.ToDateTime(aSN[i]);
					break;
				default:
					throw new ArgumentException("Invalid PrivateKeyUsagePeriod extension");
				}
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (notBefore != DateTime.MinValue)
			{
				stringBuilder.Append("Not Before: ");
				stringBuilder.Append(notBefore.ToString(CultureInfo.CurrentUICulture));
				stringBuilder.Append(Environment.NewLine);
			}
			if (notAfter != DateTime.MinValue)
			{
				stringBuilder.Append("Not After: ");
				stringBuilder.Append(notAfter.ToString(CultureInfo.CurrentUICulture));
				stringBuilder.Append(Environment.NewLine);
			}
			return stringBuilder.ToString();
		}
	}
}
