using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Mono.Security.Cryptography;

namespace System.Security.Policy
{
	[Serializable]
	[ComVisible(true)]
	public sealed class PublisherMembershipCondition : IConstantMembershipCondition, IMembershipCondition, ISecurityEncodable, ISecurityPolicyEncodable
	{
		private readonly int version = 1;

		private X509Certificate x509;

		public X509Certificate Certificate
		{
			get
			{
				return x509;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				x509 = value;
			}
		}

		internal PublisherMembershipCondition()
		{
		}

		public PublisherMembershipCondition(X509Certificate certificate)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			if (certificate.GetHashCode() == 0)
			{
				throw new ArgumentException("certificate");
			}
			x509 = certificate;
		}

		public bool Check(Evidence evidence)
		{
			if (evidence == null)
			{
				return false;
			}
			IEnumerator hostEnumerator = evidence.GetHostEnumerator();
			while (hostEnumerator.MoveNext())
			{
				if (hostEnumerator.Current is Publisher && x509.Equals((hostEnumerator.Current as Publisher).Certificate))
				{
					return true;
				}
			}
			return false;
		}

		public IMembershipCondition Copy()
		{
			return new PublisherMembershipCondition(x509);
		}

		public override bool Equals(object o)
		{
			PublisherMembershipCondition publisherMembershipCondition = o as PublisherMembershipCondition;
			if (publisherMembershipCondition == null)
			{
				return false;
			}
			return x509.Equals(publisherMembershipCondition.Certificate);
		}

		public void FromXml(SecurityElement e)
		{
			FromXml(e, null);
		}

		public void FromXml(SecurityElement e, PolicyLevel level)
		{
			MembershipConditionHelper.CheckSecurityElement(e, "e", version, version);
			string text = e.Attribute("X509Certificate");
			if (text != null)
			{
				byte[] data = CryptoConvert.FromHex(text);
				x509 = new X509Certificate(data);
			}
		}

		public override int GetHashCode()
		{
			return x509.GetHashCode();
		}

		public override string ToString()
		{
			return "Publisher - " + x509.GetPublicKeyString();
		}

		public SecurityElement ToXml()
		{
			return ToXml(null);
		}

		public SecurityElement ToXml(PolicyLevel level)
		{
			SecurityElement securityElement = MembershipConditionHelper.Element(typeof(PublisherMembershipCondition), version);
			securityElement.AddAttribute("X509Certificate", x509.GetRawCertDataString());
			return securityElement;
		}
	}
}
