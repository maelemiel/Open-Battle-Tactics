using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;

namespace System.Security.Policy
{
	[Serializable]
	[ComVisible(true)]
	public sealed class Publisher : IBuiltInEvidence, IIdentityPermissionFactory
	{
		private X509Certificate m_cert;

		public X509Certificate Certificate
		{
			get
			{
				if (m_cert.GetHashCode() == 0)
				{
					throw new ArgumentException("m_cert");
				}
				return m_cert;
			}
		}

		public Publisher(X509Certificate cert)
		{
			if (cert == null)
			{
				throw new ArgumentNullException("cert");
			}
			if (cert.GetHashCode() == 0)
			{
				throw new ArgumentException("cert");
			}
			m_cert = cert;
		}

		int IBuiltInEvidence.GetRequiredSize(bool verbose)
		{
			return ((!verbose) ? 1 : 3) + m_cert.GetRawCertData().Length;
		}

		[MonoTODO("IBuiltInEvidence")]
		int IBuiltInEvidence.InitFromBuffer(char[] buffer, int position)
		{
			return 0;
		}

		[MonoTODO("IBuiltInEvidence")]
		int IBuiltInEvidence.OutputToBuffer(char[] buffer, int position, bool verbose)
		{
			return 0;
		}

		public object Copy()
		{
			return new Publisher(m_cert);
		}

		public IPermission CreateIdentityPermission(Evidence evidence)
		{
			return new PublisherIdentityPermission(m_cert);
		}

		public override bool Equals(object o)
		{
			Publisher publisher = o as Publisher;
			if (publisher == null)
			{
				throw new ArgumentException("o", Locale.GetText("not a Publisher instance."));
			}
			return m_cert.Equals(publisher.Certificate);
		}

		public override int GetHashCode()
		{
			return m_cert.GetHashCode();
		}

		public override string ToString()
		{
			SecurityElement securityElement = new SecurityElement("System.Security.Policy.Publisher");
			securityElement.AddAttribute("version", "1");
			SecurityElement securityElement2 = new SecurityElement("X509v3Certificate");
			string rawCertDataString = m_cert.GetRawCertDataString();
			if (rawCertDataString != null)
			{
				securityElement2.Text = rawCertDataString;
			}
			securityElement.AddChild(securityElement2);
			return securityElement.ToString();
		}
	}
}
