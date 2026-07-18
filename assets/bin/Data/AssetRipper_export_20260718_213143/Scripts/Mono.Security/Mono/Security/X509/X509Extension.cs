using System;
using System.Globalization;
using System.Text;

namespace Mono.Security.X509
{
	public class X509Extension
	{
		protected string extnOid;

		protected bool extnCritical;

		protected ASN1 extnValue;

		public ASN1 ASN1
		{
			get
			{
				ASN1 aSN = new ASN1(48);
				aSN.Add(ASN1Convert.FromOid(extnOid));
				if (extnCritical)
				{
					aSN.Add(new ASN1(1, new byte[1] { 255 }));
				}
				Encode();
				aSN.Add(extnValue);
				return aSN;
			}
		}

		public string Oid
		{
			get
			{
				return extnOid;
			}
		}

		public bool Critical
		{
			get
			{
				return extnCritical;
			}
			set
			{
				extnCritical = value;
			}
		}

		public virtual string Name
		{
			get
			{
				return extnOid;
			}
		}

		public ASN1 Value
		{
			get
			{
				if (extnValue == null)
				{
					Encode();
				}
				return extnValue;
			}
		}

		protected X509Extension()
		{
			extnCritical = false;
		}

		public X509Extension(ASN1 asn1)
		{
			if (asn1.Tag != 48 || asn1.Count < 2)
			{
				throw new ArgumentException(global::Locale.GetText("Invalid X.509 extension."));
			}
			if (asn1[0].Tag != 6)
			{
				throw new ArgumentException(global::Locale.GetText("Invalid X.509 extension."));
			}
			extnOid = ASN1Convert.ToOid(asn1[0]);
			extnCritical = asn1[1].Tag == 1 && asn1[1].Value[0] == byte.MaxValue;
			extnValue = asn1[asn1.Count - 1];
			if (extnValue.Tag == 4 && extnValue.Length > 0 && extnValue.Count == 0)
			{
				try
				{
					ASN1 asn2 = new ASN1(extnValue.Value);
					extnValue.Value = null;
					extnValue.Add(asn2);
				}
				catch
				{
				}
			}
			Decode();
		}

		public X509Extension(X509Extension extension)
		{
			if (extension == null)
			{
				throw new ArgumentNullException("extension");
			}
			if (extension.Value == null || extension.Value.Tag != 4 || extension.Value.Count != 1)
			{
				throw new ArgumentException(global::Locale.GetText("Invalid X.509 extension."));
			}
			extnOid = extension.Oid;
			extnCritical = extension.Critical;
			extnValue = extension.Value;
			Decode();
		}

		protected virtual void Decode()
		{
		}

		protected virtual void Encode()
		{
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			X509Extension x509Extension = obj as X509Extension;
			if (x509Extension == null)
			{
				return false;
			}
			if (extnCritical != x509Extension.extnCritical)
			{
				return false;
			}
			if (extnOid != x509Extension.extnOid)
			{
				return false;
			}
			if (extnValue.Length != x509Extension.extnValue.Length)
			{
				return false;
			}
			for (int i = 0; i < extnValue.Length; i++)
			{
				if (extnValue[i] != x509Extension.extnValue[i])
				{
					return false;
				}
			}
			return true;
		}

		public byte[] GetBytes()
		{
			return ASN1.GetBytes();
		}

		public override int GetHashCode()
		{
			return extnOid.GetHashCode();
		}

		private void WriteLine(StringBuilder sb, int n, int pos)
		{
			byte[] value = extnValue.Value;
			int num = pos;
			for (int i = 0; i < 8; i++)
			{
				if (i < n)
				{
					sb.Append(value[num++].ToString("X2", CultureInfo.InvariantCulture));
					sb.Append(" ");
				}
				else
				{
					sb.Append("   ");
				}
			}
			sb.Append("  ");
			num = pos;
			for (int j = 0; j < n; j++)
			{
				byte b = value[num++];
				if (b < 32)
				{
					sb.Append(".");
				}
				else
				{
					sb.Append(Convert.ToChar(b));
				}
			}
			sb.Append(Environment.NewLine);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = extnValue.Length >> 3;
			int n = extnValue.Length - (num << 3);
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				WriteLine(stringBuilder, 8, num2);
				num2 += 8;
			}
			WriteLine(stringBuilder, n, num2);
			return stringBuilder.ToString();
		}
	}
}
