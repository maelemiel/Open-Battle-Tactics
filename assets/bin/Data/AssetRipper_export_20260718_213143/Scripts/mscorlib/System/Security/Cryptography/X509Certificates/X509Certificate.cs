using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using Mono.Security.Authenticode;
using Mono.Security.X509;

namespace System.Security.Cryptography.X509Certificates
{
	[Serializable]
	[ComVisible(true)]
	[MonoTODO("X509ContentType.SerializedCert isn't supported (anywhere in the class)")]
	public class X509Certificate : ISerializable, IDeserializationCallback
	{
		internal struct CertificateContext
		{
			public uint dwCertEncodingType;

			public IntPtr pbCertEncoded;

			public uint cbCertEncoded;

			public IntPtr pCertInfo;

			public IntPtr hCertStore;
		}

		private Mono.Security.X509.X509Certificate x509;

		private bool hideDates;

		private byte[] cachedCertificateHash;

		private string issuer_name;

		private string subject_name;

		public string Issuer
		{
			get
			{
				if (x509 == null)
				{
					throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
				}
				if (issuer_name == null)
				{
					issuer_name = X501.ToString(x509.GetIssuerName(), true, ", ", true);
				}
				return issuer_name;
			}
		}

		public string Subject
		{
			get
			{
				if (x509 == null)
				{
					throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
				}
				if (subject_name == null)
				{
					subject_name = X501.ToString(x509.GetSubjectName(), true, ", ", true);
				}
				return subject_name;
			}
		}

		[ComVisible(false)]
		public IntPtr Handle
		{
			get
			{
				return IntPtr.Zero;
			}
		}

		internal X509Certificate(byte[] data, bool dates)
		{
			if (data != null)
			{
				Import(data, (string)null, X509KeyStorageFlags.DefaultKeySet);
				hideDates = !dates;
			}
		}

		public X509Certificate(byte[] data)
			: this(data, true)
		{
		}

		public X509Certificate(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentException("Invalid handle.");
			}
			throw new NotSupportedException();
		}

		public X509Certificate(X509Certificate cert)
		{
			if (cert == null)
			{
				throw new ArgumentNullException("cert");
			}
			if (cert != null)
			{
				byte[] rawCertData = cert.GetRawCertData();
				if (rawCertData != null)
				{
					x509 = new Mono.Security.X509.X509Certificate(rawCertData);
				}
				hideDates = false;
			}
		}

		public X509Certificate()
		{
		}

		public X509Certificate(byte[] rawData, string password)
		{
			Import(rawData, password, X509KeyStorageFlags.DefaultKeySet);
		}

		[MonoTODO("SecureString support is incomplete")]
		public X509Certificate(byte[] rawData, SecureString password)
		{
			Import(rawData, password, X509KeyStorageFlags.DefaultKeySet);
		}

		public X509Certificate(byte[] rawData, string password, X509KeyStorageFlags keyStorageFlags)
		{
			Import(rawData, password, keyStorageFlags);
		}

		[MonoTODO("SecureString support is incomplete")]
		public X509Certificate(byte[] rawData, SecureString password, X509KeyStorageFlags keyStorageFlags)
		{
			Import(rawData, password, keyStorageFlags);
		}

		public X509Certificate(string fileName)
		{
			Import(fileName, (string)null, X509KeyStorageFlags.DefaultKeySet);
		}

		public X509Certificate(string fileName, string password)
		{
			Import(fileName, password, X509KeyStorageFlags.DefaultKeySet);
		}

		[MonoTODO("SecureString support is incomplete")]
		public X509Certificate(string fileName, SecureString password)
		{
			Import(fileName, password, X509KeyStorageFlags.DefaultKeySet);
		}

		public X509Certificate(string fileName, string password, X509KeyStorageFlags keyStorageFlags)
		{
			Import(fileName, password, keyStorageFlags);
		}

		[MonoTODO("SecureString support is incomplete")]
		public X509Certificate(string fileName, SecureString password, X509KeyStorageFlags keyStorageFlags)
		{
			Import(fileName, password, keyStorageFlags);
		}

		public X509Certificate(SerializationInfo info, StreamingContext context)
		{
			byte[] rawData = (byte[])info.GetValue("RawData", typeof(byte[]));
			Import(rawData, (string)null, X509KeyStorageFlags.DefaultKeySet);
		}

		void IDeserializationCallback.OnDeserialization(object sender)
		{
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("RawData", x509.RawData);
		}

		private string tostr(byte[] data)
		{
			if (data != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < data.Length; i++)
				{
					stringBuilder.Append(data[i].ToString("X2"));
				}
				return stringBuilder.ToString();
			}
			return null;
		}

		public static X509Certificate CreateFromCertFile(string filename)
		{
			byte[] data = Load(filename);
			return new X509Certificate(data);
		}

		[MonoTODO("Incomplete - minimal validation in this version")]
		public static X509Certificate CreateFromSignedFile(string filename)
		{
			try
			{
				AuthenticodeDeformatter authenticodeDeformatter = new AuthenticodeDeformatter(filename);
				if (authenticodeDeformatter.SigningCertificate != null)
				{
					return new X509Certificate(authenticodeDeformatter.SigningCertificate.RawData);
				}
			}
			catch (SecurityException)
			{
				throw;
			}
			catch (Exception inner)
			{
				string text = Locale.GetText("Couldn't extract digital signature from {0}.", filename);
				throw new COMException(text, inner);
			}
			throw new CryptographicException(Locale.GetText("{0} isn't signed.", filename));
		}

		private void InitFromHandle(IntPtr handle)
		{
			if (handle != IntPtr.Zero)
			{
				CertificateContext certificateContext = (CertificateContext)Marshal.PtrToStructure(handle, typeof(CertificateContext));
				byte[] array = new byte[certificateContext.cbCertEncoded];
				Marshal.Copy(certificateContext.pbCertEncoded, array, 0, (int)certificateContext.cbCertEncoded);
				x509 = new Mono.Security.X509.X509Certificate(array);
			}
		}

		public virtual bool Equals(X509Certificate other)
		{
			if (other == null)
			{
				return false;
			}
			if (other.x509 == null)
			{
				if (x509 == null)
				{
					return true;
				}
				throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
			}
			byte[] rawData = other.x509.RawData;
			if (rawData != null)
			{
				if (x509 == null)
				{
					return false;
				}
				if (x509.RawData == null)
				{
					return false;
				}
				if (rawData.Length == x509.RawData.Length)
				{
					for (int i = 0; i < rawData.Length; i++)
					{
						if (rawData[i] != x509.RawData[i])
						{
							return false;
						}
					}
					return true;
				}
				return false;
			}
			return x509 == null || x509.RawData == null;
		}

		public virtual byte[] GetCertHash()
		{
			if (x509 == null)
			{
				throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
			}
			if (cachedCertificateHash == null && x509 != null)
			{
				SHA1 sHA = SHA1.Create();
				cachedCertificateHash = sHA.ComputeHash(x509.RawData);
			}
			return cachedCertificateHash;
		}

		public virtual string GetCertHashString()
		{
			return tostr(GetCertHash());
		}

		public virtual string GetEffectiveDateString()
		{
			if (hideDates)
			{
				return null;
			}
			if (x509 == null)
			{
				throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
			}
			return x509.ValidFrom.ToLocalTime().ToString();
		}

		public virtual string GetExpirationDateString()
		{
			if (hideDates)
			{
				return null;
			}
			if (x509 == null)
			{
				throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
			}
			return x509.ValidUntil.ToLocalTime().ToString();
		}

		public virtual string GetFormat()
		{
			return "X509";
		}

		public override int GetHashCode()
		{
			if (x509 == null)
			{
				return 0;
			}
			if (cachedCertificateHash == null)
			{
				GetCertHash();
			}
			if (cachedCertificateHash != null && cachedCertificateHash.Length >= 4)
			{
				return (cachedCertificateHash[0] << 24) | (cachedCertificateHash[1] << 16) | (cachedCertificateHash[2] << 8) | cachedCertificateHash[3];
			}
			return 0;
		}

		[Obsolete("Use the Issuer property.")]
		public virtual string GetIssuerName()
		{
			if (x509 == null)
			{
				throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
			}
			return x509.IssuerName;
		}

		public virtual string GetKeyAlgorithm()
		{
			if (x509 == null)
			{
				throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
			}
			return x509.KeyAlgorithm;
		}

		public virtual byte[] GetKeyAlgorithmParameters()
		{
			if (x509 == null)
			{
				throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
			}
			byte[] keyAlgorithmParameters = x509.KeyAlgorithmParameters;
			if (keyAlgorithmParameters == null)
			{
				throw new CryptographicException(Locale.GetText("Parameters not part of the certificate"));
			}
			return keyAlgorithmParameters;
		}

		public virtual string GetKeyAlgorithmParametersString()
		{
			return tostr(GetKeyAlgorithmParameters());
		}

		[Obsolete("Use the Subject property.")]
		public virtual string GetName()
		{
			if (x509 == null)
			{
				throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
			}
			return x509.SubjectName;
		}

		public virtual byte[] GetPublicKey()
		{
			if (x509 == null)
			{
				throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
			}
			return x509.PublicKey;
		}

		public virtual string GetPublicKeyString()
		{
			return tostr(GetPublicKey());
		}

		public virtual byte[] GetRawCertData()
		{
			if (x509 == null)
			{
				throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
			}
			return x509.RawData;
		}

		public virtual string GetRawCertDataString()
		{
			if (x509 == null)
			{
				throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
			}
			return tostr(x509.RawData);
		}

		public virtual byte[] GetSerialNumber()
		{
			if (x509 == null)
			{
				throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
			}
			return x509.SerialNumber;
		}

		public virtual string GetSerialNumberString()
		{
			byte[] serialNumber = GetSerialNumber();
			Array.Reverse(serialNumber);
			return tostr(serialNumber);
		}

		public override string ToString()
		{
			return base.ToString();
		}

		public virtual string ToString(bool fVerbose)
		{
			if (!fVerbose || x509 == null)
			{
				return base.ToString();
			}
			string newLine = Environment.NewLine;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("[Subject]{0}  {1}{0}{0}", newLine, Subject);
			stringBuilder.AppendFormat("[Issuer]{0}  {1}{0}{0}", newLine, Issuer);
			stringBuilder.AppendFormat("[Not Before]{0}  {1}{0}{0}", newLine, GetEffectiveDateString());
			stringBuilder.AppendFormat("[Not After]{0}  {1}{0}{0}", newLine, GetExpirationDateString());
			stringBuilder.AppendFormat("[Thumbprint]{0}  {1}{0}", newLine, GetCertHashString());
			stringBuilder.Append(newLine);
			return stringBuilder.ToString();
		}

		private static byte[] Load(string fileName)
		{
			byte[] array = null;
			using (FileStream fileStream = File.OpenRead(fileName))
			{
				array = new byte[fileStream.Length];
				fileStream.Read(array, 0, array.Length);
				fileStream.Close();
				return array;
			}
		}

		[ComVisible(false)]
		public override bool Equals(object obj)
		{
			X509Certificate x509Certificate = obj as X509Certificate;
			if (x509Certificate != null)
			{
				return Equals(x509Certificate);
			}
			return false;
		}

		[ComVisible(false)]
		[MonoTODO("X509ContentType.Pfx/Pkcs12 and SerializedCert are not supported")]
		public virtual byte[] Export(X509ContentType contentType)
		{
			return Export(contentType, (byte[])null);
		}

		[ComVisible(false)]
		[MonoTODO("X509ContentType.Pfx/Pkcs12 and SerializedCert are not supported")]
		public virtual byte[] Export(X509ContentType contentType, string password)
		{
			byte[] password2 = ((password != null) ? Encoding.UTF8.GetBytes(password) : null);
			return Export(contentType, password2);
		}

		[MonoTODO("X509ContentType.Pfx/Pkcs12 and SerializedCert are not supported. SecureString support is incomplete.")]
		public virtual byte[] Export(X509ContentType contentType, SecureString password)
		{
			byte[] password2 = ((password != null) ? password.GetBuffer() : null);
			return Export(contentType, password2);
		}

		internal byte[] Export(X509ContentType contentType, byte[] password)
		{
			if (x509 == null)
			{
				throw new CryptographicException(Locale.GetText("Certificate instance is empty."));
			}
			try
			{
				switch (contentType)
				{
				case X509ContentType.Cert:
					return x509.RawData;
				case X509ContentType.Pfx:
					throw new NotSupportedException();
				case X509ContentType.SerializedCert:
					throw new NotSupportedException();
				default:
				{
					string text = Locale.GetText("This certificate format '{0}' cannot be exported.", contentType);
					throw new CryptographicException(text);
				}
				}
			}
			finally
			{
				if (password != null)
				{
					Array.Clear(password, 0, password.Length);
				}
			}
		}

		[ComVisible(false)]
		public virtual void Import(byte[] rawData)
		{
			Import(rawData, (string)null, X509KeyStorageFlags.DefaultKeySet);
		}

		[MonoTODO("missing KeyStorageFlags support")]
		[ComVisible(false)]
		public virtual void Import(byte[] rawData, string password, X509KeyStorageFlags keyStorageFlags)
		{
			Reset();
			if (password == null)
			{
				try
				{
					x509 = new Mono.Security.X509.X509Certificate(rawData);
					return;
				}
				catch (Exception inner)
				{
					try
					{
						PKCS12 pKCS = new PKCS12(rawData);
						if (pKCS.Certificates.Count > 0)
						{
							x509 = pKCS.Certificates[0];
						}
						else
						{
							x509 = null;
						}
						return;
					}
					catch
					{
						string text = Locale.GetText("Unable to decode certificate.");
						throw new CryptographicException(text, inner);
					}
				}
			}
			try
			{
				PKCS12 pKCS2 = new PKCS12(rawData, password);
				if (pKCS2.Certificates.Count > 0)
				{
					x509 = pKCS2.Certificates[0];
				}
				else
				{
					x509 = null;
				}
			}
			catch
			{
				x509 = new Mono.Security.X509.X509Certificate(rawData);
			}
		}

		[MonoTODO("SecureString support is incomplete")]
		public virtual void Import(byte[] rawData, SecureString password, X509KeyStorageFlags keyStorageFlags)
		{
			Import(rawData, (string)null, keyStorageFlags);
		}

		[ComVisible(false)]
		public virtual void Import(string fileName)
		{
			byte[] rawData = Load(fileName);
			Import(rawData, (string)null, X509KeyStorageFlags.DefaultKeySet);
		}

		[MonoTODO("missing KeyStorageFlags support")]
		[ComVisible(false)]
		public virtual void Import(string fileName, string password, X509KeyStorageFlags keyStorageFlags)
		{
			byte[] rawData = Load(fileName);
			Import(rawData, password, keyStorageFlags);
		}

		[MonoTODO("SecureString support is incomplete, missing KeyStorageFlags support")]
		public virtual void Import(string fileName, SecureString password, X509KeyStorageFlags keyStorageFlags)
		{
			byte[] rawData = Load(fileName);
			Import(rawData, (string)null, keyStorageFlags);
		}

		[ComVisible(false)]
		public virtual void Reset()
		{
			x509 = null;
			issuer_name = null;
			subject_name = null;
			hideDates = false;
			cachedCertificateHash = null;
		}
	}
}
