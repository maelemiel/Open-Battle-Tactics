using Mono.Security.X509.Extensions;

namespace Mono.Security.X509
{
	internal class X509Chain
	{
		private X509CertificateCollection roots;

		private X509CertificateCollection certs;

		private X509Certificate _root;

		private X509CertificateCollection _chain;

		private X509ChainStatusFlags _status;

		public X509CertificateCollection Chain
		{
			get
			{
				return _chain;
			}
		}

		public X509Certificate Root
		{
			get
			{
				return _root;
			}
		}

		public X509ChainStatusFlags Status
		{
			get
			{
				return _status;
			}
		}

		public X509CertificateCollection TrustAnchors
		{
			get
			{
				if (roots == null)
				{
					roots = new X509CertificateCollection();
					roots.AddRange(X509StoreManager.TrustedRootCertificates);
					return roots;
				}
				return roots;
			}
			set
			{
				roots = value;
			}
		}

		public X509Chain()
		{
			certs = new X509CertificateCollection();
		}

		public X509Chain(X509CertificateCollection chain)
			: this()
		{
			_chain = new X509CertificateCollection();
			_chain.AddRange(chain);
		}

		public void LoadCertificate(X509Certificate x509)
		{
			certs.Add(x509);
		}

		public void LoadCertificates(X509CertificateCollection collection)
		{
			certs.AddRange(collection);
		}

		public X509Certificate FindByIssuerName(string issuerName)
		{
			foreach (X509Certificate cert in certs)
			{
				if (cert.IssuerName == issuerName)
				{
					return cert;
				}
			}
			return null;
		}

		public bool Build(X509Certificate leaf)
		{
			_status = X509ChainStatusFlags.NoError;
			if (_chain == null)
			{
				_chain = new X509CertificateCollection();
				X509Certificate x509Certificate = leaf;
				X509Certificate potentialRoot = x509Certificate;
				while (x509Certificate != null && !x509Certificate.IsSelfSigned)
				{
					potentialRoot = x509Certificate;
					_chain.Add(x509Certificate);
					x509Certificate = FindCertificateParent(x509Certificate);
				}
				_root = FindCertificateRoot(potentialRoot);
			}
			else
			{
				int count = _chain.Count;
				if (count > 0)
				{
					if (IsParent(leaf, _chain[0]))
					{
						int i;
						for (i = 1; i < count && IsParent(_chain[i - 1], _chain[i]); i++)
						{
						}
						if (i == count)
						{
							_root = FindCertificateRoot(_chain[count - 1]);
						}
					}
				}
				else
				{
					_root = FindCertificateRoot(leaf);
				}
			}
			if (_chain != null && _status == X509ChainStatusFlags.NoError)
			{
				foreach (X509Certificate item in _chain)
				{
					if (!IsValid(item))
					{
						return false;
					}
				}
				if (!IsValid(leaf))
				{
					if (_status == X509ChainStatusFlags.NotTimeNested)
					{
						_status = X509ChainStatusFlags.NotTimeValid;
					}
					return false;
				}
				if (_root != null && !IsValid(_root))
				{
					return false;
				}
			}
			return _status == X509ChainStatusFlags.NoError;
		}

		public void Reset()
		{
			_status = X509ChainStatusFlags.NoError;
			roots = null;
			certs.Clear();
			if (_chain != null)
			{
				_chain.Clear();
			}
		}

		private bool IsValid(X509Certificate cert)
		{
			if (!cert.IsCurrent)
			{
				_status = X509ChainStatusFlags.NotTimeNested;
				return false;
			}
			return true;
		}

		private X509Certificate FindCertificateParent(X509Certificate child)
		{
			foreach (X509Certificate cert in certs)
			{
				if (IsParent(child, cert))
				{
					return cert;
				}
			}
			return null;
		}

		private X509Certificate FindCertificateRoot(X509Certificate potentialRoot)
		{
			if (potentialRoot == null)
			{
				_status = X509ChainStatusFlags.PartialChain;
				return null;
			}
			if (IsTrusted(potentialRoot))
			{
				return potentialRoot;
			}
			foreach (X509Certificate trustAnchor in TrustAnchors)
			{
				if (IsParent(potentialRoot, trustAnchor))
				{
					return trustAnchor;
				}
			}
			if (potentialRoot.IsSelfSigned)
			{
				_status = X509ChainStatusFlags.UntrustedRoot;
				return potentialRoot;
			}
			_status = X509ChainStatusFlags.PartialChain;
			return null;
		}

		private bool IsTrusted(X509Certificate potentialTrusted)
		{
			return TrustAnchors.Contains(potentialTrusted);
		}

		private bool IsParent(X509Certificate child, X509Certificate parent)
		{
			if (child.IssuerName != parent.SubjectName)
			{
				return false;
			}
			if (parent.Version > 2 && !IsTrusted(parent))
			{
				X509Extension x509Extension = parent.Extensions["2.5.29.19"];
				if (x509Extension != null)
				{
					BasicConstraintsExtension basicConstraintsExtension = new BasicConstraintsExtension(x509Extension);
					if (!basicConstraintsExtension.CertificateAuthority)
					{
						_status = X509ChainStatusFlags.InvalidBasicConstraints;
					}
				}
				else
				{
					_status = X509ChainStatusFlags.InvalidBasicConstraints;
				}
			}
			if (!child.VerifySignature(parent.RSA))
			{
				_status = X509ChainStatusFlags.NotSignatureValid;
				return false;
			}
			return true;
		}
	}
}
