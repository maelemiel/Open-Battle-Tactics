using System;
using System.Collections;
using System.Text;

namespace Mono.Security.X509.Extensions
{
	public class CertificatePoliciesExtension : X509Extension
	{
		private Hashtable policies;

		public override string Name
		{
			get
			{
				return "Certificate Policies";
			}
		}

		public CertificatePoliciesExtension()
		{
			extnOid = "2.5.29.32";
			policies = new Hashtable();
		}

		public CertificatePoliciesExtension(ASN1 asn1)
			: base(asn1)
		{
		}

		public CertificatePoliciesExtension(X509Extension extension)
			: base(extension)
		{
		}

		protected override void Decode()
		{
			policies = new Hashtable();
			ASN1 aSN = new ASN1(extnValue.Value);
			if (aSN.Tag != 48)
			{
				throw new ArgumentException("Invalid CertificatePolicies extension");
			}
			for (int i = 0; i < aSN.Count; i++)
			{
				policies.Add(ASN1Convert.ToOid(aSN[i][0]), null);
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 1;
			foreach (DictionaryEntry policy in policies)
			{
				stringBuilder.Append("[");
				stringBuilder.Append(num++);
				stringBuilder.Append("]Certificate Policy:");
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append("\tPolicyIdentifier=");
				stringBuilder.Append((string)policy.Key);
				stringBuilder.Append(Environment.NewLine);
			}
			return stringBuilder.ToString();
		}
	}
}
