using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Mono.Security.X509;

namespace Mono.Security.Authenticode
{
	public class AuthenticodeFormatter : AuthenticodeBase
	{
		private const string signedData = "1.2.840.113549.1.7.2";

		private const string countersignature = "1.2.840.113549.1.9.6";

		private const string spcStatementType = "1.3.6.1.4.1.311.2.1.11";

		private const string spcSpOpusInfo = "1.3.6.1.4.1.311.2.1.12";

		private const string spcPelmageData = "1.3.6.1.4.1.311.2.1.15";

		private const string commercialCodeSigning = "1.3.6.1.4.1.311.2.1.22";

		private const string timestampCountersignature = "1.3.6.1.4.1.311.3.2.1";

		private Authority authority;

		private X509CertificateCollection certs;

		private ArrayList crls;

		private string hash;

		private RSA rsa;

		private Uri timestamp;

		private ASN1 authenticode;

		private PKCS7.SignedData pkcs7;

		private string description;

		private Uri url;

		private static byte[] obsolete = new byte[37]
		{
			3, 1, 0, 160, 32, 162, 30, 128, 28, 0,
			60, 0, 60, 0, 60, 0, 79, 0, 98, 0,
			115, 0, 111, 0, 108, 0, 101, 0, 116, 0,
			101, 0, 62, 0, 62, 0, 62
		};

		public Authority Authority
		{
			get
			{
				return authority;
			}
			set
			{
				authority = value;
			}
		}

		public X509CertificateCollection Certificates
		{
			get
			{
				return certs;
			}
		}

		public ArrayList Crl
		{
			get
			{
				return crls;
			}
		}

		public string Hash
		{
			get
			{
				if (hash == null)
				{
					hash = "MD5";
				}
				return hash;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Hash");
				}
				string text = value.ToUpper(CultureInfo.InvariantCulture);
				switch (text)
				{
				case "MD5":
				case "SHA1":
					hash = text;
					break;
				default:
					throw new ArgumentException("Invalid Authenticode hash algorithm");
				}
			}
		}

		public RSA RSA
		{
			get
			{
				return rsa;
			}
			set
			{
				rsa = value;
			}
		}

		public Uri TimestampUrl
		{
			get
			{
				return timestamp;
			}
			set
			{
				timestamp = value;
			}
		}

		public string Description
		{
			get
			{
				return description;
			}
			set
			{
				description = value;
			}
		}

		public Uri Url
		{
			get
			{
				return url;
			}
			set
			{
				url = value;
			}
		}

		public AuthenticodeFormatter()
		{
			certs = new X509CertificateCollection();
			crls = new ArrayList();
			authority = Authority.Maximum;
			pkcs7 = new PKCS7.SignedData();
		}

		private ASN1 AlgorithmIdentifier(string oid)
		{
			ASN1 aSN = new ASN1(48);
			aSN.Add(ASN1Convert.FromOid(oid));
			aSN.Add(new ASN1(5));
			return aSN;
		}

		private ASN1 Attribute(string oid, ASN1 value)
		{
			ASN1 aSN = new ASN1(48);
			aSN.Add(ASN1Convert.FromOid(oid));
			ASN1 aSN2 = aSN.Add(new ASN1(49));
			aSN2.Add(value);
			return aSN;
		}

		private ASN1 Opus(string description, string url)
		{
			ASN1 aSN = new ASN1(48);
			if (description != null)
			{
				ASN1 aSN2 = aSN.Add(new ASN1(160));
				aSN2.Add(new ASN1(128, Encoding.BigEndianUnicode.GetBytes(description)));
			}
			if (url != null)
			{
				ASN1 aSN3 = aSN.Add(new ASN1(161));
				aSN3.Add(new ASN1(128, Encoding.ASCII.GetBytes(url)));
			}
			return aSN;
		}

		private byte[] Header(byte[] fileHash, string hashAlgorithm)
		{
			string oid = CryptoConfig.MapNameToOID(hashAlgorithm);
			ASN1 aSN = new ASN1(48);
			ASN1 aSN2 = aSN.Add(new ASN1(48));
			aSN2.Add(ASN1Convert.FromOid("1.3.6.1.4.1.311.2.1.15"));
			aSN2.Add(new ASN1(48, obsolete));
			ASN1 aSN3 = aSN.Add(new ASN1(48));
			aSN3.Add(AlgorithmIdentifier(oid));
			aSN3.Add(new ASN1(4, fileHash));
			pkcs7.HashName = hashAlgorithm;
			pkcs7.Certificates.AddRange(certs);
			pkcs7.ContentInfo.ContentType = "1.3.6.1.4.1.311.2.1.4";
			pkcs7.ContentInfo.Content.Add(aSN);
			pkcs7.SignerInfo.Certificate = certs[0];
			pkcs7.SignerInfo.Key = rsa;
			ASN1 aSN4 = null;
			aSN4 = ((!(url == null)) ? Attribute("1.3.6.1.4.1.311.2.1.12", Opus(description, url.ToString())) : Attribute("1.3.6.1.4.1.311.2.1.12", Opus(description, null)));
			pkcs7.SignerInfo.AuthenticatedAttributes.Add(aSN4);
			pkcs7.GetASN1();
			return pkcs7.SignerInfo.Signature;
		}

		public ASN1 TimestampRequest(byte[] signature)
		{
			PKCS7.ContentInfo contentInfo = new PKCS7.ContentInfo("1.2.840.113549.1.7.1");
			contentInfo.Content.Add(new ASN1(4, signature));
			return PKCS7.AlgorithmIdentifier("1.3.6.1.4.1.311.3.2.1", contentInfo.ASN1);
		}

		public void ProcessTimestamp(byte[] response)
		{
			ASN1 aSN = new ASN1(Convert.FromBase64String(Encoding.ASCII.GetString(response)));
			for (int i = 0; i < aSN[1][0][3].Count; i++)
			{
				pkcs7.Certificates.Add(new X509Certificate(aSN[1][0][3][i].GetBytes()));
			}
			pkcs7.SignerInfo.UnauthenticatedAttributes.Add(Attribute("1.2.840.113549.1.9.6", aSN[1][0][4][0]));
		}

		private byte[] Timestamp(byte[] signature)
		{
			ASN1 aSN = TimestampRequest(signature);
			WebClient webClient = new WebClient();
			webClient.Headers.Add("Content-Type", "application/octet-stream");
			webClient.Headers.Add("Accept", "application/octet-stream");
			byte[] bytes = Encoding.ASCII.GetBytes(Convert.ToBase64String(aSN.GetBytes()));
			return webClient.UploadData(timestamp.ToString(), bytes);
		}

		private bool Save(string fileName, byte[] asn)
		{
			File.Copy(fileName, fileName + ".bak", true);
			using (FileStream fileStream = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite))
			{
				int num;
				if (base.SecurityOffset > 0)
				{
					num = base.SecurityOffset;
				}
				else if (base.CoffSymbolTableOffset > 0)
				{
					fileStream.Seek(base.PEOffset + 12, SeekOrigin.Begin);
					for (int i = 0; i < 8; i++)
					{
						fileStream.WriteByte(0);
					}
					num = base.CoffSymbolTableOffset;
				}
				else
				{
					num = (int)fileStream.Length;
				}
				int num2 = num & 7;
				if (num2 > 0)
				{
					num2 = 8 - num2;
				}
				byte[] bytes = Mono.Security.BitConverterLE.GetBytes(num + num2);
				fileStream.Seek(base.PEOffset + 152, SeekOrigin.Begin);
				fileStream.Write(bytes, 0, 4);
				int num3 = asn.Length + 8;
				int num4 = num3 & 7;
				if (num4 > 0)
				{
					num4 = 8 - num4;
				}
				bytes = Mono.Security.BitConverterLE.GetBytes(num3 + num4);
				fileStream.Seek(base.PEOffset + 156, SeekOrigin.Begin);
				fileStream.Write(bytes, 0, 4);
				fileStream.Seek(num, SeekOrigin.Begin);
				if (num2 > 0)
				{
					byte[] array = new byte[num2];
					fileStream.Write(array, 0, array.Length);
				}
				fileStream.Write(bytes, 0, bytes.Length);
				bytes = Mono.Security.BitConverterLE.GetBytes(131584);
				fileStream.Write(bytes, 0, bytes.Length);
				fileStream.Write(asn, 0, asn.Length);
				if (num4 > 0)
				{
					byte[] array2 = new byte[num4];
					fileStream.Write(array2, 0, array2.Length);
				}
				fileStream.Close();
			}
			return true;
		}

		public bool Sign(string fileName)
		{
			try
			{
				Open(fileName);
				HashAlgorithm hashAlgorithm = HashAlgorithm.Create(Hash);
				byte[] fileHash = GetHash(hashAlgorithm);
				byte[] signature = Header(fileHash, Hash);
				if (timestamp != null)
				{
					byte[] response = Timestamp(signature);
					ProcessTimestamp(response);
				}
				PKCS7.ContentInfo contentInfo = new PKCS7.ContentInfo("1.2.840.113549.1.7.2");
				contentInfo.Content.Add(pkcs7.ASN1);
				authenticode = contentInfo.ASN1;
				Close();
				return Save(fileName, authenticode.GetBytes());
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
			return false;
		}

		public bool Timestamp(string fileName)
		{
			try
			{
				AuthenticodeDeformatter authenticodeDeformatter = new AuthenticodeDeformatter(fileName);
				byte[] signature = authenticodeDeformatter.Signature;
				if (signature != null)
				{
					Open(fileName);
					PKCS7.ContentInfo contentInfo = new PKCS7.ContentInfo(signature);
					pkcs7 = new PKCS7.SignedData(contentInfo.Content);
					byte[] bytes = Timestamp(pkcs7.SignerInfo.Signature);
					ASN1 aSN = new ASN1(Convert.FromBase64String(Encoding.ASCII.GetString(bytes)));
					ASN1 aSN2 = new ASN1(signature);
					ASN1 aSN3 = aSN2.Element(1, 160);
					if (aSN3 == null)
					{
						return false;
					}
					ASN1 aSN4 = aSN3.Element(0, 48);
					if (aSN4 == null)
					{
						return false;
					}
					ASN1 aSN5 = aSN4.Element(3, 160);
					if (aSN5 == null)
					{
						aSN5 = new ASN1(160);
						aSN4.Add(aSN5);
					}
					for (int i = 0; i < aSN[1][0][3].Count; i++)
					{
						aSN5.Add(aSN[1][0][3][i]);
					}
					ASN1 aSN6 = aSN4[aSN4.Count - 1];
					ASN1 aSN7 = aSN6[0];
					ASN1 aSN8 = aSN7[aSN7.Count - 1];
					if (aSN8.Tag != 161)
					{
						aSN8 = new ASN1(161);
						aSN7.Add(aSN8);
					}
					aSN8.Add(Attribute("1.2.840.113549.1.9.6", aSN[1][0][4][0]));
					return Save(fileName, aSN2.GetBytes());
				}
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
			return false;
		}
	}
}
