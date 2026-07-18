using System;
using System.Globalization;
using System.Text;

namespace Mono.Security.X509.Extensions
{
	public class BasicConstraintsExtension : X509Extension
	{
		public const int NoPathLengthConstraint = -1;

		private bool cA;

		private int pathLenConstraint;

		public bool CertificateAuthority
		{
			get
			{
				return cA;
			}
			set
			{
				cA = value;
			}
		}

		public override string Name
		{
			get
			{
				return "Basic Constraints";
			}
		}

		public int PathLenConstraint
		{
			get
			{
				return pathLenConstraint;
			}
			set
			{
				if (value < -1)
				{
					string text = global::Locale.GetText("PathLenConstraint must be positive or -1 for none ({0}).", value);
					throw new ArgumentOutOfRangeException(text);
				}
				pathLenConstraint = value;
			}
		}

		public BasicConstraintsExtension()
		{
			extnOid = "2.5.29.19";
			pathLenConstraint = -1;
		}

		public BasicConstraintsExtension(ASN1 asn1)
			: base(asn1)
		{
		}

		public BasicConstraintsExtension(X509Extension extension)
			: base(extension)
		{
		}

		protected override void Decode()
		{
			cA = false;
			pathLenConstraint = -1;
			ASN1 aSN = new ASN1(extnValue.Value);
			if (aSN.Tag != 48)
			{
				throw new ArgumentException("Invalid BasicConstraints extension");
			}
			int num = 0;
			ASN1 aSN2 = aSN[num++];
			if (aSN2 != null && aSN2.Tag == 1)
			{
				cA = aSN2.Value[0] == byte.MaxValue;
				aSN2 = aSN[num++];
			}
			if (aSN2 != null && aSN2.Tag == 2)
			{
				pathLenConstraint = ASN1Convert.ToInt32(aSN2);
			}
		}

		protected override void Encode()
		{
			ASN1 aSN = new ASN1(48);
			if (cA)
			{
				aSN.Add(new ASN1(1, new byte[1] { 255 }));
			}
			if (cA && pathLenConstraint >= 0)
			{
				aSN.Add(ASN1Convert.FromInt32(pathLenConstraint));
			}
			extnValue = new ASN1(4);
			extnValue.Add(aSN);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Subject Type=");
			stringBuilder.Append((!cA) ? "End Entity" : "CA");
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append("Path Length Constraint=");
			if (pathLenConstraint == -1)
			{
				stringBuilder.Append("None");
			}
			else
			{
				stringBuilder.Append(pathLenConstraint.ToString(CultureInfo.InvariantCulture));
			}
			stringBuilder.Append(Environment.NewLine);
			return stringBuilder.ToString();
		}
	}
}
