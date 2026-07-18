using Mono.Security;
using Mono.Security.Cryptography;
using Mono.Security.X509;

namespace System.Security.Cryptography.X509Certificates
{
	public sealed class PublicKey
	{
		private const string rsaOid = "1.2.840.113549.1.1.1";

		private const string dsaOid = "1.2.840.10040.4.1";

		private AsymmetricAlgorithm _key;

		private AsnEncodedData _keyValue;

		private AsnEncodedData _params;

		private Oid _oid;

		public AsnEncodedData EncodedKeyValue
		{
			get
			{
				return _keyValue;
			}
		}

		public AsnEncodedData EncodedParameters
		{
			get
			{
				return _params;
			}
		}

		public AsymmetricAlgorithm Key
		{
			get
			{
				if (_key == null)
				{
					switch (_oid.Value)
					{
					default:
					{
						int num;
						if (num == 1)
						{
							_key = DecodeDSA(_keyValue.RawData, _params.RawData);
							break;
						}
						string text = Locale.GetText("Cannot decode public key from unknown OID '{0}'.", _oid.Value);
						throw new NotSupportedException(text);
					}
					case "1.2.840.113549.1.1.1":
						_key = DecodeRSA(_keyValue.RawData);
						break;
					}
				}
				return _key;
			}
		}

		public Oid Oid
		{
			get
			{
				return _oid;
			}
		}

		public PublicKey(Oid oid, AsnEncodedData parameters, AsnEncodedData keyValue)
		{
			if (oid == null)
			{
				throw new ArgumentNullException("oid");
			}
			if (parameters == null)
			{
				throw new ArgumentNullException("parameters");
			}
			if (keyValue == null)
			{
				throw new ArgumentNullException("keyValue");
			}
			_oid = new Oid(oid);
			_params = new AsnEncodedData(parameters);
			_keyValue = new AsnEncodedData(keyValue);
		}

		internal PublicKey(Mono.Security.X509.X509Certificate certificate)
		{
			bool flag = true;
			if (certificate.KeyAlgorithm == "1.2.840.113549.1.1.1")
			{
				RSACryptoServiceProvider rSACryptoServiceProvider = certificate.RSA as RSACryptoServiceProvider;
				if (rSACryptoServiceProvider != null && rSACryptoServiceProvider.PublicOnly)
				{
					_key = certificate.RSA;
					flag = false;
				}
				else
				{
					RSAManaged rSAManaged = certificate.RSA as RSAManaged;
					if (rSAManaged != null && rSAManaged.PublicOnly)
					{
						_key = certificate.RSA;
						flag = false;
					}
				}
				if (flag)
				{
					RSAParameters parameters = certificate.RSA.ExportParameters(false);
					_key = RSA.Create();
					(_key as RSA).ImportParameters(parameters);
				}
			}
			else
			{
				DSACryptoServiceProvider dSACryptoServiceProvider = certificate.DSA as DSACryptoServiceProvider;
				if (dSACryptoServiceProvider != null && dSACryptoServiceProvider.PublicOnly)
				{
					_key = certificate.DSA;
					flag = false;
				}
				if (flag)
				{
					DSAParameters parameters2 = certificate.DSA.ExportParameters(false);
					_key = DSA.Create();
					(_key as DSA).ImportParameters(parameters2);
				}
			}
			_oid = new Oid(certificate.KeyAlgorithm);
			_keyValue = new AsnEncodedData(_oid, certificate.PublicKey);
			_params = new AsnEncodedData(_oid, certificate.KeyAlgorithmParameters);
		}

		private static byte[] GetUnsignedBigInteger(byte[] integer)
		{
			if (integer[0] != 0)
			{
				return integer;
			}
			int num = integer.Length - 1;
			byte[] array = new byte[num];
			Buffer.BlockCopy(integer, 1, array, 0, num);
			return array;
		}

		internal static DSA DecodeDSA(byte[] rawPublicKey, byte[] rawParameters)
		{
			DSAParameters parameters = default(DSAParameters);
			try
			{
				ASN1 aSN = new ASN1(rawPublicKey);
				if (aSN.Tag != 2)
				{
					throw new CryptographicException(Locale.GetText("Missing DSA Y integer."));
				}
				parameters.Y = GetUnsignedBigInteger(aSN.Value);
				ASN1 aSN2 = new ASN1(rawParameters);
				if (aSN2 == null || aSN2.Tag != 48 || aSN2.Count < 3)
				{
					throw new CryptographicException(Locale.GetText("Missing DSA parameters."));
				}
				if (aSN2[0].Tag != 2 || aSN2[1].Tag != 2 || aSN2[2].Tag != 2)
				{
					throw new CryptographicException(Locale.GetText("Invalid DSA parameters."));
				}
				parameters.P = GetUnsignedBigInteger(aSN2[0].Value);
				parameters.Q = GetUnsignedBigInteger(aSN2[1].Value);
				parameters.G = GetUnsignedBigInteger(aSN2[2].Value);
			}
			catch (Exception inner)
			{
				string text = Locale.GetText("Error decoding the ASN.1 structure.");
				throw new CryptographicException(text, inner);
			}
			DSA dSA = new DSACryptoServiceProvider(parameters.Y.Length << 3);
			dSA.ImportParameters(parameters);
			return dSA;
		}

		internal static RSA DecodeRSA(byte[] rawPublicKey)
		{
			RSAParameters parameters = default(RSAParameters);
			try
			{
				ASN1 aSN = new ASN1(rawPublicKey);
				if (aSN.Count == 0)
				{
					throw new CryptographicException(Locale.GetText("Missing RSA modulus and exponent."));
				}
				ASN1 aSN2 = aSN[0];
				if (aSN2 == null || aSN2.Tag != 2)
				{
					throw new CryptographicException(Locale.GetText("Missing RSA modulus."));
				}
				ASN1 aSN3 = aSN[1];
				if (aSN3.Tag != 2)
				{
					throw new CryptographicException(Locale.GetText("Missing RSA public exponent."));
				}
				parameters.Modulus = GetUnsignedBigInteger(aSN2.Value);
				parameters.Exponent = aSN3.Value;
			}
			catch (Exception inner)
			{
				string text = Locale.GetText("Error decoding the ASN.1 structure.");
				throw new CryptographicException(text, inner);
			}
			int dwKeySize = parameters.Modulus.Length << 3;
			RSA rSA = new RSACryptoServiceProvider(dwKeySize);
			rSA.ImportParameters(parameters);
			return rSA;
		}
	}
}
