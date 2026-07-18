using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using Mono.Security.X509.Extensions;

namespace Mono.Security.X509
{
	internal class X509Store
	{
		private string _storePath;

		private X509CertificateCollection _certificates;

		private ArrayList _crls;

		private bool _crl;

		private string _name;

		public X509CertificateCollection Certificates
		{
			get
			{
				if (_certificates == null)
				{
					_certificates = BuildCertificatesCollection(_storePath);
				}
				return _certificates;
			}
		}

		public ArrayList Crls
		{
			get
			{
				if (!_crl)
				{
					_crls = new ArrayList();
				}
				if (_crls == null)
				{
					_crls = BuildCrlsCollection(_storePath);
				}
				return _crls;
			}
		}

		public string Name
		{
			get
			{
				if (_name == null)
				{
					int num = _storePath.LastIndexOf(Path.DirectorySeparatorChar);
					_name = _storePath.Substring(num + 1);
				}
				return _name;
			}
		}

		internal X509Store(string path, bool crl)
		{
			_storePath = path;
			_crl = crl;
		}

		public void Clear()
		{
			if (_certificates != null)
			{
				_certificates.Clear();
			}
			_certificates = null;
			if (_crls != null)
			{
				_crls.Clear();
			}
			_crls = null;
		}

		public void Import(X509Certificate certificate)
		{
			CheckStore(_storePath, true);
			string path = Path.Combine(_storePath, GetUniqueName(certificate));
			if (!File.Exists(path))
			{
				using (FileStream fileStream = File.Create(path))
				{
					byte[] rawData = certificate.RawData;
					fileStream.Write(rawData, 0, rawData.Length);
					fileStream.Close();
				}
			}
		}

		public void Import(X509Crl crl)
		{
			CheckStore(_storePath, true);
			string path = Path.Combine(_storePath, GetUniqueName(crl));
			if (!File.Exists(path))
			{
				using (FileStream fileStream = File.Create(path))
				{
					byte[] rawData = crl.RawData;
					fileStream.Write(rawData, 0, rawData.Length);
				}
			}
		}

		public void Remove(X509Certificate certificate)
		{
			string path = Path.Combine(_storePath, GetUniqueName(certificate));
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}

		public void Remove(X509Crl crl)
		{
			string path = Path.Combine(_storePath, GetUniqueName(crl));
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}

		private string GetUniqueName(X509Certificate certificate)
		{
			byte[] array = GetUniqueName(certificate.Extensions);
			string method;
			if (array == null)
			{
				method = "tbp";
				array = certificate.Hash;
			}
			else
			{
				method = "ski";
			}
			return GetUniqueName(method, array, ".cer");
		}

		private string GetUniqueName(X509Crl crl)
		{
			byte[] array = GetUniqueName(crl.Extensions);
			string method;
			if (array == null)
			{
				method = "tbp";
				array = crl.Hash;
			}
			else
			{
				method = "ski";
			}
			return GetUniqueName(method, array, ".crl");
		}

		private byte[] GetUniqueName(X509ExtensionCollection extensions)
		{
			X509Extension x509Extension = extensions["2.5.29.14"];
			if (x509Extension == null)
			{
				return null;
			}
			SubjectKeyIdentifierExtension subjectKeyIdentifierExtension = new SubjectKeyIdentifierExtension(x509Extension);
			return subjectKeyIdentifierExtension.Identifier;
		}

		private string GetUniqueName(string method, byte[] name, string fileExtension)
		{
			StringBuilder stringBuilder = new StringBuilder(method);
			stringBuilder.Append("-");
			foreach (byte b in name)
			{
				stringBuilder.Append(b.ToString("X2", CultureInfo.InvariantCulture));
			}
			stringBuilder.Append(fileExtension);
			return stringBuilder.ToString();
		}

		private byte[] Load(string filename)
		{
			byte[] array = null;
			using (FileStream fileStream = File.OpenRead(filename))
			{
				array = new byte[fileStream.Length];
				fileStream.Read(array, 0, array.Length);
				fileStream.Close();
				return array;
			}
		}

		private X509Certificate LoadCertificate(string filename)
		{
			byte[] data = Load(filename);
			return new X509Certificate(data);
		}

		private X509Crl LoadCrl(string filename)
		{
			byte[] crl = Load(filename);
			return new X509Crl(crl);
		}

		private bool CheckStore(string path, bool throwException)
		{
			try
			{
				if (Directory.Exists(path))
				{
					return true;
				}
				Directory.CreateDirectory(path);
				return Directory.Exists(path);
			}
			catch
			{
				if (throwException)
				{
					throw;
				}
				return false;
			}
		}

		private X509CertificateCollection BuildCertificatesCollection(string storeName)
		{
			X509CertificateCollection x509CertificateCollection = new X509CertificateCollection();
			string path = Path.Combine(_storePath, storeName);
			if (!CheckStore(path, false))
			{
				return x509CertificateCollection;
			}
			string[] files = Directory.GetFiles(path, "*.cer");
			if (files != null && files.Length > 0)
			{
				string[] array = files;
				foreach (string filename in array)
				{
					try
					{
						X509Certificate value = LoadCertificate(filename);
						x509CertificateCollection.Add(value);
					}
					catch
					{
					}
				}
			}
			return x509CertificateCollection;
		}

		private ArrayList BuildCrlsCollection(string storeName)
		{
			ArrayList arrayList = new ArrayList();
			string path = Path.Combine(_storePath, storeName);
			if (!CheckStore(path, false))
			{
				return arrayList;
			}
			string[] files = Directory.GetFiles(path, "*.crl");
			if (files != null && files.Length > 0)
			{
				string[] array = files;
				foreach (string filename in array)
				{
					try
					{
						X509Crl value = LoadCrl(filename);
						arrayList.Add(value);
					}
					catch
					{
					}
				}
			}
			return arrayList;
		}
	}
}
