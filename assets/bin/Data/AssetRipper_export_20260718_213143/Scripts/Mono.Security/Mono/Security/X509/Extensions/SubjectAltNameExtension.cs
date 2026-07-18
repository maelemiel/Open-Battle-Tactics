using System;

namespace Mono.Security.X509.Extensions
{
	public class SubjectAltNameExtension : X509Extension
	{
		private GeneralNames _names;

		public override string Name
		{
			get
			{
				return "Subject Alternative Name";
			}
		}

		public string[] RFC822
		{
			get
			{
				return _names.RFC822;
			}
		}

		public string[] DNSNames
		{
			get
			{
				return _names.DNSNames;
			}
		}

		public string[] IPAddresses
		{
			get
			{
				return _names.IPAddresses;
			}
		}

		public string[] UniformResourceIdentifiers
		{
			get
			{
				return _names.UniformResourceIdentifiers;
			}
		}

		public SubjectAltNameExtension()
		{
			extnOid = "2.5.29.17";
			_names = new GeneralNames();
		}

		public SubjectAltNameExtension(ASN1 asn1)
			: base(asn1)
		{
		}

		public SubjectAltNameExtension(X509Extension extension)
			: base(extension)
		{
		}

		public SubjectAltNameExtension(string[] rfc822, string[] dnsNames, string[] ipAddresses, string[] uris)
		{
			_names = new GeneralNames(rfc822, dnsNames, ipAddresses, uris);
			extnValue = new ASN1(4, _names.GetBytes());
			extnOid = "2.5.29.17";
		}

		protected override void Decode()
		{
			ASN1 aSN = new ASN1(extnValue.Value);
			if (aSN.Tag != 48)
			{
				throw new ArgumentException("Invalid SubjectAltName extension");
			}
			_names = new GeneralNames(aSN);
		}

		public override string ToString()
		{
			return _names.ToString();
		}
	}
}
