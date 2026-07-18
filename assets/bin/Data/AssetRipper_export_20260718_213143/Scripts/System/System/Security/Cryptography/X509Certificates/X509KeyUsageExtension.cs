using System.Text;
using Mono.Security;

namespace System.Security.Cryptography.X509Certificates
{
	public sealed class X509KeyUsageExtension : X509Extension
	{
		internal const string oid = "2.5.29.15";

		internal const string friendlyName = "Key Usage";

		internal const X509KeyUsageFlags all = X509KeyUsageFlags.EncipherOnly | X509KeyUsageFlags.CrlSign | X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.KeyAgreement | X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.NonRepudiation | X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.DecipherOnly;

		private X509KeyUsageFlags _keyUsages;

		private System.Security.Cryptography.AsnDecodeStatus _status;

		public X509KeyUsageFlags KeyUsages
		{
			get
			{
				System.Security.Cryptography.AsnDecodeStatus status = _status;
				if (status == System.Security.Cryptography.AsnDecodeStatus.Ok || status == System.Security.Cryptography.AsnDecodeStatus.InformationNotAvailable)
				{
					return _keyUsages;
				}
				throw new CryptographicException("Badly encoded extension.");
			}
		}

		public X509KeyUsageExtension()
		{
			_oid = new Oid("2.5.29.15", "Key Usage");
		}

		public X509KeyUsageExtension(AsnEncodedData encodedKeyUsage, bool critical)
		{
			_oid = new Oid("2.5.29.15", "Key Usage");
			_raw = encodedKeyUsage.RawData;
			base.Critical = critical;
			_status = Decode(base.RawData);
		}

		public X509KeyUsageExtension(X509KeyUsageFlags keyUsages, bool critical)
		{
			_oid = new Oid("2.5.29.15", "Key Usage");
			base.Critical = critical;
			_keyUsages = GetValidFlags(keyUsages);
			base.RawData = Encode();
		}

		public override void CopyFrom(AsnEncodedData encodedData)
		{
			if (encodedData == null)
			{
				throw new ArgumentNullException("encodedData");
			}
			X509Extension x509Extension = encodedData as X509Extension;
			if (x509Extension == null)
			{
				throw new ArgumentException(Locale.GetText("Wrong type."), "encodedData");
			}
			if (x509Extension._oid == null)
			{
				_oid = new Oid("2.5.29.15", "Key Usage");
			}
			else
			{
				_oid = new Oid(x509Extension._oid);
			}
			base.RawData = x509Extension.RawData;
			base.Critical = x509Extension.Critical;
			_status = Decode(base.RawData);
		}

		internal X509KeyUsageFlags GetValidFlags(X509KeyUsageFlags flags)
		{
			if ((flags & (X509KeyUsageFlags.EncipherOnly | X509KeyUsageFlags.CrlSign | X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.KeyAgreement | X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.NonRepudiation | X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.DecipherOnly)) != flags)
			{
				return X509KeyUsageFlags.None;
			}
			return flags;
		}

		internal System.Security.Cryptography.AsnDecodeStatus Decode(byte[] extension)
		{
			if (extension == null || extension.Length == 0)
			{
				return System.Security.Cryptography.AsnDecodeStatus.BadAsn;
			}
			if (extension[0] != 3)
			{
				return System.Security.Cryptography.AsnDecodeStatus.BadTag;
			}
			if (extension.Length < 3)
			{
				return System.Security.Cryptography.AsnDecodeStatus.BadLength;
			}
			if (extension.Length < 4)
			{
				return System.Security.Cryptography.AsnDecodeStatus.InformationNotAvailable;
			}
			try
			{
				ASN1 aSN = new ASN1(extension);
				int num = 0;
				int num2 = 1;
				while (num2 < aSN.Value.Length)
				{
					num = (num << 8) + aSN.Value[num2++];
				}
				_keyUsages = GetValidFlags((X509KeyUsageFlags)num);
			}
			catch
			{
				return System.Security.Cryptography.AsnDecodeStatus.BadAsn;
			}
			return System.Security.Cryptography.AsnDecodeStatus.Ok;
		}

		internal byte[] Encode()
		{
			ASN1 aSN = null;
			int keyUsages = (int)_keyUsages;
			byte b = 0;
			if (keyUsages == 0)
			{
				aSN = new ASN1(3, new byte[1] { b });
			}
			else
			{
				int num = ((keyUsages >= 255) ? (keyUsages >> 8) : keyUsages);
				while ((num & 1) == 0 && b < 8)
				{
					b++;
					num >>= 1;
				}
				aSN = ((keyUsages > 255) ? new ASN1(3, new byte[3]
				{
					b,
					(byte)keyUsages,
					(byte)(keyUsages >> 8)
				}) : new ASN1(3, new byte[2]
				{
					b,
					(byte)keyUsages
				}));
			}
			return aSN.GetBytes();
		}

		internal override string ToString(bool multiLine)
		{
			switch (_status)
			{
			case System.Security.Cryptography.AsnDecodeStatus.BadAsn:
				return string.Empty;
			case System.Security.Cryptography.AsnDecodeStatus.BadTag:
			case System.Security.Cryptography.AsnDecodeStatus.BadLength:
				return FormatUnkownData(_raw);
			case System.Security.Cryptography.AsnDecodeStatus.InformationNotAvailable:
				return "Information Not Available";
			default:
			{
				if (_oid.Value != "2.5.29.15")
				{
					return string.Format("Unknown Key Usage ({0})", _oid.Value);
				}
				if (_keyUsages == X509KeyUsageFlags.None)
				{
					return "Information Not Available";
				}
				StringBuilder stringBuilder = new StringBuilder();
				if ((_keyUsages & X509KeyUsageFlags.DigitalSignature) != X509KeyUsageFlags.None)
				{
					stringBuilder.Append("Digital Signature");
				}
				if ((_keyUsages & X509KeyUsageFlags.NonRepudiation) != X509KeyUsageFlags.None)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append("Non-Repudiation");
				}
				if ((_keyUsages & X509KeyUsageFlags.KeyEncipherment) != X509KeyUsageFlags.None)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append("Key Encipherment");
				}
				if ((_keyUsages & X509KeyUsageFlags.DataEncipherment) != X509KeyUsageFlags.None)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append("Data Encipherment");
				}
				if ((_keyUsages & X509KeyUsageFlags.KeyAgreement) != X509KeyUsageFlags.None)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append("Key Agreement");
				}
				if ((_keyUsages & X509KeyUsageFlags.KeyCertSign) != X509KeyUsageFlags.None)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append("Certificate Signing");
				}
				if ((_keyUsages & X509KeyUsageFlags.CrlSign) != X509KeyUsageFlags.None)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append("Off-line CRL Signing, CRL Signing");
				}
				if ((_keyUsages & X509KeyUsageFlags.EncipherOnly) != X509KeyUsageFlags.None)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append("Encipher Only");
				}
				if ((_keyUsages & X509KeyUsageFlags.DecipherOnly) != X509KeyUsageFlags.None)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append("Decipher Only");
				}
				int keyUsages = (int)_keyUsages;
				stringBuilder.Append(" (");
				stringBuilder.Append(((byte)keyUsages).ToString("x2"));
				if (keyUsages > 255)
				{
					stringBuilder.Append(" ");
					stringBuilder.Append(((byte)(keyUsages >> 8)).ToString("x2"));
				}
				stringBuilder.Append(")");
				if (multiLine)
				{
					stringBuilder.Append(Environment.NewLine);
				}
				return stringBuilder.ToString();
			}
			}
		}
	}
}
