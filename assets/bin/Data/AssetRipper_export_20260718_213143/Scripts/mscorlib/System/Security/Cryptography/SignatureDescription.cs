using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class SignatureDescription
	{
		private string _DeformatterAlgorithm;

		private string _DigestAlgorithm;

		private string _FormatterAlgorithm;

		private string _KeyAlgorithm;

		public string DeformatterAlgorithm
		{
			get
			{
				return _DeformatterAlgorithm;
			}
			set
			{
				_DeformatterAlgorithm = value;
			}
		}

		public string DigestAlgorithm
		{
			get
			{
				return _DigestAlgorithm;
			}
			set
			{
				_DigestAlgorithm = value;
			}
		}

		public string FormatterAlgorithm
		{
			get
			{
				return _FormatterAlgorithm;
			}
			set
			{
				_FormatterAlgorithm = value;
			}
		}

		public string KeyAlgorithm
		{
			get
			{
				return _KeyAlgorithm;
			}
			set
			{
				_KeyAlgorithm = value;
			}
		}

		public SignatureDescription()
		{
		}

		public SignatureDescription(SecurityElement el)
		{
			if (el == null)
			{
				throw new ArgumentNullException("el");
			}
			SecurityElement securityElement = el.SearchForChildByTag("Deformatter");
			_DeformatterAlgorithm = ((securityElement != null) ? securityElement.Text : null);
			securityElement = el.SearchForChildByTag("Digest");
			_DigestAlgorithm = ((securityElement != null) ? securityElement.Text : null);
			securityElement = el.SearchForChildByTag("Formatter");
			_FormatterAlgorithm = ((securityElement != null) ? securityElement.Text : null);
			securityElement = el.SearchForChildByTag("Key");
			_KeyAlgorithm = ((securityElement != null) ? securityElement.Text : null);
		}

		public virtual AsymmetricSignatureDeformatter CreateDeformatter(AsymmetricAlgorithm key)
		{
			if (_DeformatterAlgorithm == null)
			{
				throw new ArgumentNullException("DeformatterAlgorithm");
			}
			AsymmetricSignatureDeformatter asymmetricSignatureDeformatter = (AsymmetricSignatureDeformatter)CryptoConfig.CreateFromName(_DeformatterAlgorithm);
			if (_KeyAlgorithm == null)
			{
				throw new NullReferenceException("KeyAlgorithm");
			}
			asymmetricSignatureDeformatter.SetKey(key);
			return asymmetricSignatureDeformatter;
		}

		public virtual HashAlgorithm CreateDigest()
		{
			if (_DigestAlgorithm == null)
			{
				throw new ArgumentNullException("DigestAlgorithm");
			}
			return (HashAlgorithm)CryptoConfig.CreateFromName(_DigestAlgorithm);
		}

		public virtual AsymmetricSignatureFormatter CreateFormatter(AsymmetricAlgorithm key)
		{
			if (_FormatterAlgorithm == null)
			{
				throw new ArgumentNullException("FormatterAlgorithm");
			}
			AsymmetricSignatureFormatter asymmetricSignatureFormatter = (AsymmetricSignatureFormatter)CryptoConfig.CreateFromName(_FormatterAlgorithm);
			if (_KeyAlgorithm == null)
			{
				throw new NullReferenceException("KeyAlgorithm");
			}
			asymmetricSignatureFormatter.SetKey(key);
			return asymmetricSignatureFormatter;
		}
	}
}
