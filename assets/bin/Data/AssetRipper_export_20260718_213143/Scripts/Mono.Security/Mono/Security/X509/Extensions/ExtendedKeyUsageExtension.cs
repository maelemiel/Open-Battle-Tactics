using System;
using System.Collections;
using System.Text;

namespace Mono.Security.X509.Extensions
{
	public class ExtendedKeyUsageExtension : X509Extension
	{
		private ArrayList keyPurpose;

		public ArrayList KeyPurpose
		{
			get
			{
				return keyPurpose;
			}
		}

		public override string Name
		{
			get
			{
				return "Extended Key Usage";
			}
		}

		public ExtendedKeyUsageExtension()
		{
			extnOid = "2.5.29.37";
			keyPurpose = new ArrayList();
		}

		public ExtendedKeyUsageExtension(ASN1 asn1)
			: base(asn1)
		{
		}

		public ExtendedKeyUsageExtension(X509Extension extension)
			: base(extension)
		{
		}

		protected override void Decode()
		{
			keyPurpose = new ArrayList();
			ASN1 aSN = new ASN1(extnValue.Value);
			if (aSN.Tag != 48)
			{
				throw new ArgumentException("Invalid ExtendedKeyUsage extension");
			}
			for (int i = 0; i < aSN.Count; i++)
			{
				keyPurpose.Add(ASN1Convert.ToOid(aSN[i]));
			}
		}

		protected override void Encode()
		{
			ASN1 aSN = new ASN1(48);
			foreach (string item in keyPurpose)
			{
				aSN.Add(ASN1Convert.FromOid(item));
			}
			extnValue = new ASN1(4);
			extnValue.Add(aSN);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string item in keyPurpose)
			{
				switch (item)
				{
				case "1.3.6.1.5.5.7.3.1":
					stringBuilder.Append("Server Authentication");
					break;
				case "1.3.6.1.5.5.7.3.2":
					stringBuilder.Append("Client Authentication");
					break;
				case "1.3.6.1.5.5.7.3.3":
					stringBuilder.Append("Code Signing");
					break;
				case "1.3.6.1.5.5.7.3.4":
					stringBuilder.Append("Email Protection");
					break;
				case "1.3.6.1.5.5.7.3.8":
					stringBuilder.Append("Time Stamping");
					break;
				case "1.3.6.1.5.5.7.3.9":
					stringBuilder.Append("OCSP Signing");
					break;
				default:
					stringBuilder.Append("unknown");
					break;
				}
				stringBuilder.AppendFormat(" ({0}){1}", item, Environment.NewLine);
			}
			return stringBuilder.ToString();
		}
	}
}
