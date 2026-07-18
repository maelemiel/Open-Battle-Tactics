using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Mono.Security.Cryptography;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	public sealed class PublisherIdentityPermission : CodeAccessPermission, IBuiltInPermission
	{
		private const int version = 1;

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
					throw new ArgumentNullException("X509Certificate");
				}
				x509 = value;
			}
		}

		public PublisherIdentityPermission(PermissionState state)
		{
			CodeAccessPermission.CheckPermissionState(state, false);
		}

		public PublisherIdentityPermission(X509Certificate certificate)
		{
			Certificate = certificate;
		}

		int IBuiltInPermission.GetTokenIndex()
		{
			return 10;
		}

		public override IPermission Copy()
		{
			PublisherIdentityPermission publisherIdentityPermission = new PublisherIdentityPermission(PermissionState.None);
			if (x509 != null)
			{
				publisherIdentityPermission.Certificate = x509;
			}
			return publisherIdentityPermission;
		}

		public override void FromXml(SecurityElement esd)
		{
			CodeAccessPermission.CheckSecurityElement(esd, "esd", 1, 1);
			string text = esd.Attributes["X509v3Certificate"] as string;
			if (text != null)
			{
				byte[] data = CryptoConvert.FromHex(text);
				x509 = new X509Certificate(data);
			}
		}

		public override IPermission Intersect(IPermission target)
		{
			PublisherIdentityPermission publisherIdentityPermission = Cast(target);
			if (publisherIdentityPermission == null)
			{
				return null;
			}
			if (x509 != null && publisherIdentityPermission.x509 != null && x509.GetRawCertDataString() == publisherIdentityPermission.x509.GetRawCertDataString())
			{
				return new PublisherIdentityPermission(publisherIdentityPermission.x509);
			}
			return null;
		}

		public override bool IsSubsetOf(IPermission target)
		{
			PublisherIdentityPermission publisherIdentityPermission = Cast(target);
			if (publisherIdentityPermission == null)
			{
				return false;
			}
			if (x509 == null)
			{
				return true;
			}
			if (publisherIdentityPermission.x509 == null)
			{
				return false;
			}
			return x509.GetRawCertDataString() == publisherIdentityPermission.x509.GetRawCertDataString();
		}

		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = Element(1);
			if (x509 != null)
			{
				securityElement.AddAttribute("X509v3Certificate", x509.GetRawCertDataString());
			}
			return securityElement;
		}

		public override IPermission Union(IPermission target)
		{
			PublisherIdentityPermission publisherIdentityPermission = Cast(target);
			if (publisherIdentityPermission == null)
			{
				return Copy();
			}
			if (x509 != null && publisherIdentityPermission.x509 != null)
			{
				if (x509.GetRawCertDataString() == publisherIdentityPermission.x509.GetRawCertDataString())
				{
					return new PublisherIdentityPermission(x509);
				}
			}
			else
			{
				if (x509 == null && publisherIdentityPermission.x509 != null)
				{
					return new PublisherIdentityPermission(publisherIdentityPermission.x509);
				}
				if (x509 != null && publisherIdentityPermission.x509 == null)
				{
					return new PublisherIdentityPermission(x509);
				}
			}
			return null;
		}

		private PublisherIdentityPermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			PublisherIdentityPermission publisherIdentityPermission = target as PublisherIdentityPermission;
			if (publisherIdentityPermission == null)
			{
				CodeAccessPermission.ThrowInvalidPermission(target, typeof(PublisherIdentityPermission));
			}
			return publisherIdentityPermission;
		}
	}
}
