using System;
using System.Collections;
using System.Text;

namespace Mono.Security.X509.Extensions
{
	public class CRLDistributionPointsExtension : X509Extension
	{
		internal class DP
		{
			public string DistributionPoint;

			public ReasonFlags Reasons;

			public string CRLIssuer;

			public DP(string dp, ReasonFlags reasons, string issuer)
			{
				DistributionPoint = dp;
				Reasons = reasons;
				CRLIssuer = issuer;
			}

			public DP(ASN1 dp)
			{
				for (int i = 0; i < dp.Count; i++)
				{
					ASN1 aSN = dp[i];
					switch (aSN.Tag)
					{
					case 160:
					{
						for (int j = 0; j < aSN.Count; j++)
						{
							ASN1 aSN2 = aSN[j];
							if (aSN2.Tag == 160)
							{
								DistributionPoint = new GeneralNames(aSN2).ToString();
							}
						}
						break;
					}
					}
				}
			}
		}

		[Flags]
		public enum ReasonFlags
		{
			Unused = 0,
			KeyCompromise = 1,
			CACompromise = 2,
			AffiliationChanged = 3,
			Superseded = 4,
			CessationOfOperation = 5,
			CertificateHold = 6,
			PrivilegeWithdrawn = 7,
			AACompromise = 8
		}

		private ArrayList dps;

		public override string Name
		{
			get
			{
				return "CRL Distribution Points";
			}
		}

		public CRLDistributionPointsExtension()
		{
			extnOid = "2.5.29.31";
			dps = new ArrayList();
		}

		public CRLDistributionPointsExtension(ASN1 asn1)
			: base(asn1)
		{
		}

		public CRLDistributionPointsExtension(X509Extension extension)
			: base(extension)
		{
		}

		protected override void Decode()
		{
			dps = new ArrayList();
			ASN1 aSN = new ASN1(extnValue.Value);
			if (aSN.Tag != 48)
			{
				throw new ArgumentException("Invalid CRLDistributionPoints extension");
			}
			for (int i = 0; i < aSN.Count; i++)
			{
				dps.Add(new DP(aSN[i]));
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 1;
			foreach (DP dp in dps)
			{
				stringBuilder.Append("[");
				stringBuilder.Append(num++);
				stringBuilder.Append("]CRL Distribution Point");
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append("\tDistribution Point Name:");
				stringBuilder.Append("\t\tFull Name:");
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append("\t\t\t");
				stringBuilder.Append(dp.DistributionPoint);
				stringBuilder.Append(Environment.NewLine);
			}
			return stringBuilder.ToString();
		}
	}
}
