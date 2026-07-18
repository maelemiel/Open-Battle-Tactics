namespace System.Security.Cryptography
{
	public sealed class Oid
	{
		internal const string oidRSA = "1.2.840.113549.1.1.1";

		internal const string nameRSA = "RSA";

		internal const string oidPkcs7Data = "1.2.840.113549.1.7.1";

		internal const string namePkcs7Data = "PKCS 7 Data";

		internal const string oidPkcs9ContentType = "1.2.840.113549.1.9.3";

		internal const string namePkcs9ContentType = "Content Type";

		internal const string oidPkcs9MessageDigest = "1.2.840.113549.1.9.4";

		internal const string namePkcs9MessageDigest = "Message Digest";

		internal const string oidPkcs9SigningTime = "1.2.840.113549.1.9.5";

		internal const string namePkcs9SigningTime = "Signing Time";

		internal const string oidMd5 = "1.2.840.113549.2.5";

		internal const string nameMd5 = "md5";

		internal const string oid3Des = "1.2.840.113549.3.7";

		internal const string name3Des = "3des";

		internal const string oidSha1 = "1.3.14.3.2.26";

		internal const string nameSha1 = "sha1";

		internal const string oidSubjectAltName = "2.5.29.17";

		internal const string nameSubjectAltName = "Subject Alternative Name";

		internal const string oidNetscapeCertType = "2.16.840.1.113730.1.1";

		internal const string nameNetscapeCertType = "Netscape Cert Type";

		private string _value;

		private string _name;

		public string FriendlyName
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
				_value = GetValue(_name);
			}
		}

		public string Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				_name = GetName(_value);
			}
		}

		public Oid()
		{
		}

		public Oid(string oid)
		{
			if (oid == null)
			{
				throw new ArgumentNullException("oid");
			}
			_value = oid;
			_name = GetName(oid);
		}

		public Oid(string value, string friendlyName)
		{
			_value = value;
			_name = friendlyName;
		}

		public Oid(Oid oid)
		{
			if (oid == null)
			{
				throw new ArgumentNullException("oid");
			}
			_value = oid.Value;
			_name = oid.FriendlyName;
		}

		private string GetName(string oid)
		{
			switch (oid)
			{
			case "1.2.840.113549.1.1.1":
				return "RSA";
			case "1.2.840.113549.1.7.1":
				return "PKCS 7 Data";
			case "1.2.840.113549.1.9.3":
				return "Content Type";
			case "1.2.840.113549.1.9.4":
				return "Message Digest";
			case "1.2.840.113549.1.9.5":
				return "Signing Time";
			case "1.2.840.113549.3.7":
				return "3des";
			case "2.5.29.19":
				return "Basic Constraints";
			case "2.5.29.15":
				return "Key Usage";
			case "2.5.29.37":
				return "Enhanced Key Usage";
			case "2.5.29.14":
				return "Subject Key Identifier";
			case "2.5.29.17":
				return "Subject Alternative Name";
			case "2.16.840.1.113730.1.1":
				return "Netscape Cert Type";
			case "1.2.840.113549.2.5":
				return "md5";
			case "1.3.14.3.2.26":
				return "sha1";
			default:
				return _name;
			}
		}

		private string GetValue(string name)
		{
			switch (name)
			{
			case "RSA":
				return "1.2.840.113549.1.1.1";
			case "PKCS 7 Data":
				return "1.2.840.113549.1.7.1";
			case "Content Type":
				return "1.2.840.113549.1.9.3";
			case "Message Digest":
				return "1.2.840.113549.1.9.4";
			case "Signing Time":
				return "1.2.840.113549.1.9.5";
			case "3des":
				return "1.2.840.113549.3.7";
			case "Basic Constraints":
				return "2.5.29.19";
			case "Key Usage":
				return "2.5.29.15";
			case "Enhanced Key Usage":
				return "2.5.29.37";
			case "Subject Key Identifier":
				return "2.5.29.14";
			case "Subject Alternative Name":
				return "2.5.29.17";
			case "Netscape Cert Type":
				return "2.16.840.1.113730.1.1";
			case "md5":
				return "1.2.840.113549.2.5";
			case "sha1":
				return "1.3.14.3.2.26";
			default:
				return _value;
			}
		}
	}
}
