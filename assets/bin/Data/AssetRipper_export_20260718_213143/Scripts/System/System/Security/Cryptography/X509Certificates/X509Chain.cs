using System.Collections;
using System.Text;
using Mono.Security.X509;
using Mono.Security.X509.Extensions;

namespace System.Security.Cryptography.X509Certificates
{
	public class X509Chain
	{
		private StoreLocation location;

		private X509ChainElementCollection elements;

		private X509ChainPolicy policy;

		private X509ChainStatus[] status;

		private static X509ChainStatus[] Empty = new X509ChainStatus[0];

		private int max_path_length;

		private X500DistinguishedName working_issuer_name;

		private AsymmetricAlgorithm working_public_key;

		private X509ChainElement bce_restriction;

		private X509Store roots;

		private X509Store cas;

		private X509Certificate2Collection collection;

		[System.MonoTODO("Mono's X509Chain is fully managed. Always returns IntPtr.Zero.")]
		public IntPtr ChainContext
		{
			get
			{
				return IntPtr.Zero;
			}
		}

		public X509ChainElementCollection ChainElements
		{
			get
			{
				return elements;
			}
		}

		public X509ChainPolicy ChainPolicy
		{
			get
			{
				return policy;
			}
			set
			{
				policy = value;
			}
		}

		public X509ChainStatus[] ChainStatus
		{
			get
			{
				if (status == null)
				{
					return Empty;
				}
				return status;
			}
		}

		private X509Store Roots
		{
			get
			{
				if (roots == null)
				{
					roots = new X509Store(StoreName.Root, location);
					roots.Open(OpenFlags.ReadOnly);
				}
				return roots;
			}
		}

		private X509Store CertificateAuthorities
		{
			get
			{
				if (cas == null)
				{
					cas = new X509Store(StoreName.CertificateAuthority, location);
					cas.Open(OpenFlags.ReadOnly);
				}
				return cas;
			}
		}

		private X509Certificate2Collection CertificateCollection
		{
			get
			{
				if (collection == null)
				{
					collection = new X509Certificate2Collection(ChainPolicy.ExtraStore);
					if (Roots.Certificates.Count > 0)
					{
						collection.AddRange(Roots.Certificates);
					}
					if (CertificateAuthorities.Certificates.Count > 0)
					{
						collection.AddRange(CertificateAuthorities.Certificates);
					}
				}
				return collection;
			}
		}

		public X509Chain()
			: this(false)
		{
		}

		public X509Chain(bool useMachineContext)
		{
			location = ((!useMachineContext) ? StoreLocation.CurrentUser : StoreLocation.LocalMachine);
			elements = new X509ChainElementCollection();
			policy = new X509ChainPolicy();
		}

		[System.MonoTODO("Mono's X509Chain is fully managed. All handles are invalid.")]
		public X509Chain(IntPtr chainContext)
		{
			throw new NotSupportedException();
		}

		[System.MonoTODO("Not totally RFC3280 compliant, but neither is MS implementation...")]
		public bool Build(X509Certificate2 certificate)
		{
			if (certificate == null)
			{
				throw new ArgumentException("certificate");
			}
			Reset();
			X509ChainStatusFlags x509ChainStatusFlags;
			try
			{
				x509ChainStatusFlags = BuildChainFrom(certificate);
				ValidateChain(x509ChainStatusFlags);
			}
			catch (CryptographicException innerException)
			{
				throw new ArgumentException("certificate", innerException);
			}
			X509ChainStatusFlags x509ChainStatusFlags2 = X509ChainStatusFlags.NoError;
			ArrayList arrayList = new ArrayList();
			X509ChainElementEnumerator enumerator = elements.GetEnumerator();
			while (enumerator.MoveNext())
			{
				X509ChainElement current = enumerator.Current;
				X509ChainStatus[] chainElementStatus = current.ChainElementStatus;
				for (int i = 0; i < chainElementStatus.Length; i++)
				{
					X509ChainStatus x509ChainStatus = chainElementStatus[i];
					if ((x509ChainStatusFlags2 & x509ChainStatus.Status) != x509ChainStatus.Status)
					{
						arrayList.Add(x509ChainStatus);
						x509ChainStatusFlags2 |= x509ChainStatus.Status;
					}
				}
			}
			if (x509ChainStatusFlags != X509ChainStatusFlags.NoError)
			{
				arrayList.Insert(0, new X509ChainStatus(x509ChainStatusFlags));
			}
			status = (X509ChainStatus[])arrayList.ToArray(typeof(X509ChainStatus));
			if (status.Length == 0 || ChainPolicy.VerificationFlags == X509VerificationFlags.AllFlags)
			{
				return true;
			}
			bool flag = true;
			X509ChainStatus[] array = status;
			for (int j = 0; j < array.Length; j++)
			{
				X509ChainStatus x509ChainStatus2 = array[j];
				switch (x509ChainStatus2.Status)
				{
				case X509ChainStatusFlags.UntrustedRoot:
				case X509ChainStatusFlags.PartialChain:
					flag &= (ChainPolicy.VerificationFlags & X509VerificationFlags.AllowUnknownCertificateAuthority) != 0;
					break;
				case X509ChainStatusFlags.NotTimeValid:
					flag &= (ChainPolicy.VerificationFlags & X509VerificationFlags.IgnoreNotTimeValid) != 0;
					break;
				case X509ChainStatusFlags.NotTimeNested:
					flag &= (ChainPolicy.VerificationFlags & X509VerificationFlags.IgnoreNotTimeNested) != 0;
					break;
				case X509ChainStatusFlags.InvalidBasicConstraints:
					flag &= (ChainPolicy.VerificationFlags & X509VerificationFlags.IgnoreInvalidBasicConstraints) != 0;
					break;
				case X509ChainStatusFlags.InvalidPolicyConstraints:
				case X509ChainStatusFlags.NoIssuanceChainPolicy:
					flag &= (ChainPolicy.VerificationFlags & X509VerificationFlags.IgnoreInvalidPolicy) != 0;
					break;
				case X509ChainStatusFlags.InvalidNameConstraints:
				case X509ChainStatusFlags.HasNotSupportedNameConstraint:
				case X509ChainStatusFlags.HasNotPermittedNameConstraint:
				case X509ChainStatusFlags.HasExcludedNameConstraint:
					flag &= (ChainPolicy.VerificationFlags & X509VerificationFlags.IgnoreInvalidName) != 0;
					break;
				case X509ChainStatusFlags.InvalidExtension:
					flag &= (ChainPolicy.VerificationFlags & X509VerificationFlags.IgnoreWrongUsage) != 0;
					break;
				case X509ChainStatusFlags.CtlNotTimeValid:
					flag &= (ChainPolicy.VerificationFlags & X509VerificationFlags.IgnoreCtlNotTimeValid) != 0;
					break;
				case X509ChainStatusFlags.CtlNotValidForUsage:
					flag &= (ChainPolicy.VerificationFlags & X509VerificationFlags.IgnoreWrongUsage) != 0;
					break;
				default:
					flag = false;
					break;
				case X509ChainStatusFlags.CtlNotSignatureValid:
					break;
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		public void Reset()
		{
			if (status != null && status.Length != 0)
			{
				status = null;
			}
			if (elements.Count > 0)
			{
				elements.Clear();
			}
			if (roots != null)
			{
				roots.Close();
				roots = null;
			}
			if (cas != null)
			{
				cas.Close();
				cas = null;
			}
			collection = null;
			bce_restriction = null;
			working_public_key = null;
		}

		public static X509Chain Create()
		{
			return (X509Chain)CryptoConfig.CreateFromName("X509Chain");
		}

		private X509ChainStatusFlags BuildChainFrom(X509Certificate2 certificate)
		{
			elements.Add(certificate);
			while (!IsChainComplete(certificate))
			{
				certificate = FindParent(certificate);
				if (certificate == null)
				{
					return X509ChainStatusFlags.PartialChain;
				}
				if (elements.Contains(certificate))
				{
					return X509ChainStatusFlags.Cyclic;
				}
				elements.Add(certificate);
			}
			if (!Roots.Certificates.Contains(certificate))
			{
				elements[elements.Count - 1].StatusFlags |= X509ChainStatusFlags.UntrustedRoot;
			}
			return X509ChainStatusFlags.NoError;
		}

		private X509Certificate2 SelectBestFromCollection(X509Certificate2 child, X509Certificate2Collection c)
		{
			switch (c.Count)
			{
			case 0:
				return null;
			case 1:
				return c[0];
			default:
			{
				X509Certificate2Collection x509Certificate2Collection = c.Find(X509FindType.FindByTimeValid, ChainPolicy.VerificationTime, false);
				switch (x509Certificate2Collection.Count)
				{
				case 0:
					x509Certificate2Collection = c;
					break;
				case 1:
					return x509Certificate2Collection[0];
				}
				string authorityKeyIdentifier = GetAuthorityKeyIdentifier(child);
				if (string.IsNullOrEmpty(authorityKeyIdentifier))
				{
					return x509Certificate2Collection[0];
				}
				X509Certificate2Enumerator enumerator = x509Certificate2Collection.GetEnumerator();
				while (enumerator.MoveNext())
				{
					X509Certificate2 current = enumerator.Current;
					string subjectKeyIdentifier = GetSubjectKeyIdentifier(current);
					if (authorityKeyIdentifier == subjectKeyIdentifier)
					{
						return current;
					}
				}
				return x509Certificate2Collection[0];
			}
			}
		}

		private X509Certificate2 FindParent(X509Certificate2 certificate)
		{
			X509Certificate2Collection x509Certificate2Collection = CertificateCollection.Find(X509FindType.FindBySubjectDistinguishedName, certificate.Issuer, false);
			string authorityKeyIdentifier = GetAuthorityKeyIdentifier(certificate);
			if (authorityKeyIdentifier != null && authorityKeyIdentifier.Length > 0)
			{
				x509Certificate2Collection.AddRange(CertificateCollection.Find(X509FindType.FindBySubjectKeyIdentifier, authorityKeyIdentifier, false));
			}
			X509Certificate2 x509Certificate = SelectBestFromCollection(certificate, x509Certificate2Collection);
			return (!certificate.Equals(x509Certificate)) ? x509Certificate : null;
		}

		private bool IsChainComplete(X509Certificate2 certificate)
		{
			if (!IsSelfIssued(certificate))
			{
				return false;
			}
			if (certificate.Version < 3)
			{
				return true;
			}
			string subjectKeyIdentifier = GetSubjectKeyIdentifier(certificate);
			if (string.IsNullOrEmpty(subjectKeyIdentifier))
			{
				return true;
			}
			string authorityKeyIdentifier = GetAuthorityKeyIdentifier(certificate);
			if (string.IsNullOrEmpty(authorityKeyIdentifier))
			{
				return true;
			}
			return authorityKeyIdentifier == subjectKeyIdentifier;
		}

		private bool IsSelfIssued(X509Certificate2 certificate)
		{
			return certificate.Issuer == certificate.Subject;
		}

		private void ValidateChain(X509ChainStatusFlags flag)
		{
			int num = elements.Count - 1;
			X509Certificate2 certificate = elements[num].Certificate;
			if ((flag & X509ChainStatusFlags.PartialChain) == 0)
			{
				Process(num);
				if (num == 0)
				{
					elements[0].UncompressFlags();
					return;
				}
				num--;
			}
			working_public_key = certificate.PublicKey.Key;
			working_issuer_name = certificate.IssuerName;
			max_path_length = num;
			for (int num2 = num; num2 > 0; num2--)
			{
				Process(num2);
				PrepareForNextCertificate(num2);
			}
			Process(0);
			CheckRevocationOnChain(flag);
			WrapUp();
		}

		private void Process(int n)
		{
			X509ChainElement x509ChainElement = elements[n];
			X509Certificate2 certificate = x509ChainElement.Certificate;
			if (n != elements.Count - 1 && certificate.MonoCertificate.KeyAlgorithm == "1.2.840.10040.4.1" && certificate.MonoCertificate.KeyAlgorithmParameters == null)
			{
				X509Certificate2 certificate2 = elements[n + 1].Certificate;
				certificate.MonoCertificate.KeyAlgorithmParameters = certificate2.MonoCertificate.KeyAlgorithmParameters;
			}
			bool flag = working_public_key == null;
			if (!IsSignedWith(certificate, (!flag) ? working_public_key : certificate.PublicKey.Key) && (flag || n != elements.Count - 1 || IsSelfIssued(certificate)))
			{
				x509ChainElement.StatusFlags |= X509ChainStatusFlags.NotSignatureValid;
			}
			if (ChainPolicy.VerificationTime < certificate.NotBefore || ChainPolicy.VerificationTime > certificate.NotAfter)
			{
				x509ChainElement.StatusFlags |= X509ChainStatusFlags.NotTimeValid;
			}
			if (!flag)
			{
				if (!X500DistinguishedName.AreEqual(certificate.IssuerName, working_issuer_name))
				{
					x509ChainElement.StatusFlags |= X509ChainStatusFlags.InvalidNameConstraints;
				}
				if (!IsSelfIssued(certificate) && n == 0)
				{
				}
			}
		}

		private void PrepareForNextCertificate(int n)
		{
			X509ChainElement x509ChainElement = elements[n];
			X509Certificate2 certificate = x509ChainElement.Certificate;
			working_issuer_name = certificate.SubjectName;
			working_public_key = certificate.PublicKey.Key;
			X509BasicConstraintsExtension x509BasicConstraintsExtension = (X509BasicConstraintsExtension)certificate.Extensions["2.5.29.19"];
			if (x509BasicConstraintsExtension != null)
			{
				if (!x509BasicConstraintsExtension.CertificateAuthority)
				{
					x509ChainElement.StatusFlags |= X509ChainStatusFlags.InvalidBasicConstraints;
				}
			}
			else if (certificate.Version >= 3)
			{
				x509ChainElement.StatusFlags |= X509ChainStatusFlags.InvalidBasicConstraints;
			}
			if (!IsSelfIssued(certificate))
			{
				if (max_path_length > 0)
				{
					max_path_length--;
				}
				else if (bce_restriction != null)
				{
					bce_restriction.StatusFlags |= X509ChainStatusFlags.InvalidBasicConstraints;
				}
			}
			if (x509BasicConstraintsExtension != null && x509BasicConstraintsExtension.HasPathLengthConstraint && x509BasicConstraintsExtension.PathLengthConstraint < max_path_length)
			{
				max_path_length = x509BasicConstraintsExtension.PathLengthConstraint;
				bce_restriction = x509ChainElement;
			}
			X509KeyUsageExtension x509KeyUsageExtension = (X509KeyUsageExtension)certificate.Extensions["2.5.29.15"];
			if (x509KeyUsageExtension != null)
			{
				X509KeyUsageFlags x509KeyUsageFlags = X509KeyUsageFlags.KeyCertSign;
				if ((x509KeyUsageExtension.KeyUsages & x509KeyUsageFlags) != x509KeyUsageFlags)
				{
					x509ChainElement.StatusFlags |= X509ChainStatusFlags.NotValidForUsage;
				}
			}
			ProcessCertificateExtensions(x509ChainElement);
		}

		private void WrapUp()
		{
			X509ChainElement x509ChainElement = elements[0];
			X509Certificate2 certificate = x509ChainElement.Certificate;
			if (IsSelfIssued(certificate))
			{
			}
			ProcessCertificateExtensions(x509ChainElement);
			for (int num = elements.Count - 1; num >= 0; num--)
			{
				elements[num].UncompressFlags();
			}
		}

		private void ProcessCertificateExtensions(X509ChainElement element)
		{
			X509ExtensionEnumerator enumerator = element.Certificate.Extensions.GetEnumerator();
			while (enumerator.MoveNext())
			{
				X509Extension current = enumerator.Current;
				if (current.Critical)
				{
					switch (current.Oid.Value)
					{
					case "2.5.29.15":
					case "2.5.29.19":
						continue;
					}
					element.StatusFlags |= X509ChainStatusFlags.InvalidExtension;
				}
			}
		}

		private bool IsSignedWith(X509Certificate2 signed, AsymmetricAlgorithm pubkey)
		{
			if (pubkey == null)
			{
				return false;
			}
			Mono.Security.X509.X509Certificate monoCertificate = signed.MonoCertificate;
			return monoCertificate.VerifySignature(pubkey);
		}

		private string GetSubjectKeyIdentifier(X509Certificate2 certificate)
		{
			X509SubjectKeyIdentifierExtension x509SubjectKeyIdentifierExtension = (X509SubjectKeyIdentifierExtension)certificate.Extensions["2.5.29.14"];
			return (x509SubjectKeyIdentifierExtension != null) ? x509SubjectKeyIdentifierExtension.SubjectKeyIdentifier : string.Empty;
		}

		private string GetAuthorityKeyIdentifier(X509Certificate2 certificate)
		{
			return GetAuthorityKeyIdentifier(certificate.MonoCertificate.Extensions["2.5.29.35"]);
		}

		private string GetAuthorityKeyIdentifier(X509Crl crl)
		{
			return GetAuthorityKeyIdentifier(crl.Extensions["2.5.29.35"]);
		}

		private string GetAuthorityKeyIdentifier(Mono.Security.X509.X509Extension ext)
		{
			if (ext == null)
			{
				return string.Empty;
			}
			AuthorityKeyIdentifierExtension authorityKeyIdentifierExtension = new AuthorityKeyIdentifierExtension(ext);
			byte[] identifier = authorityKeyIdentifierExtension.Identifier;
			if (identifier == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			byte[] array = identifier;
			foreach (byte b in array)
			{
				stringBuilder.Append(b.ToString("X02"));
			}
			return stringBuilder.ToString();
		}

		private void CheckRevocationOnChain(X509ChainStatusFlags flag)
		{
			bool flag2 = (flag & X509ChainStatusFlags.PartialChain) != 0;
			bool online;
			switch (ChainPolicy.RevocationMode)
			{
			case X509RevocationMode.Online:
				online = true;
				break;
			case X509RevocationMode.Offline:
				online = false;
				break;
			case X509RevocationMode.NoCheck:
				return;
			default:
				throw new InvalidOperationException(Locale.GetText("Invalid revocation mode."));
			}
			bool flag3 = flag2;
			for (int num = elements.Count - 1; num >= 0; num--)
			{
				bool flag4 = true;
				switch (ChainPolicy.RevocationFlag)
				{
				case X509RevocationFlag.EndCertificateOnly:
					flag4 = num == 0;
					break;
				case X509RevocationFlag.EntireChain:
					flag4 = true;
					break;
				case X509RevocationFlag.ExcludeRoot:
					flag4 = num != elements.Count - 1;
					break;
				}
				X509ChainElement x509ChainElement = elements[num];
				if (!flag3)
				{
					flag3 |= (x509ChainElement.StatusFlags & X509ChainStatusFlags.NotSignatureValid) != 0;
				}
				if (flag3)
				{
					x509ChainElement.StatusFlags |= X509ChainStatusFlags.RevocationStatusUnknown;
					x509ChainElement.StatusFlags |= X509ChainStatusFlags.OfflineRevocation;
				}
				else if (flag4 && !flag2 && !IsSelfIssued(x509ChainElement.Certificate))
				{
					x509ChainElement.StatusFlags |= CheckRevocation(x509ChainElement.Certificate, num + 1, online);
					flag3 |= (x509ChainElement.StatusFlags & X509ChainStatusFlags.Revoked) != 0;
				}
			}
		}

		private X509ChainStatusFlags CheckRevocation(X509Certificate2 certificate, int ca, bool online)
		{
			X509ChainStatusFlags x509ChainStatusFlags = X509ChainStatusFlags.RevocationStatusUnknown;
			X509ChainElement x509ChainElement = elements[ca];
			X509Certificate2 certificate2 = x509ChainElement.Certificate;
			while (IsSelfIssued(certificate2) && ca < elements.Count - 1)
			{
				x509ChainStatusFlags = CheckRevocation(certificate, certificate2, online);
				if (x509ChainStatusFlags != X509ChainStatusFlags.RevocationStatusUnknown)
				{
					break;
				}
				ca++;
				x509ChainElement = elements[ca];
				certificate2 = x509ChainElement.Certificate;
			}
			if (x509ChainStatusFlags == X509ChainStatusFlags.RevocationStatusUnknown)
			{
				x509ChainStatusFlags = CheckRevocation(certificate, certificate2, online);
			}
			return x509ChainStatusFlags;
		}

		private X509ChainStatusFlags CheckRevocation(X509Certificate2 certificate, X509Certificate2 ca_cert, bool online)
		{
			X509KeyUsageExtension x509KeyUsageExtension = (X509KeyUsageExtension)ca_cert.Extensions["2.5.29.15"];
			if (x509KeyUsageExtension != null)
			{
				X509KeyUsageFlags x509KeyUsageFlags = X509KeyUsageFlags.CrlSign;
				if ((x509KeyUsageExtension.KeyUsages & x509KeyUsageFlags) != x509KeyUsageFlags)
				{
					return X509ChainStatusFlags.RevocationStatusUnknown;
				}
			}
			X509Crl x509Crl = FindCrl(ca_cert);
			if (x509Crl != null || online)
			{
			}
			if (x509Crl != null)
			{
				if (!x509Crl.VerifySignature(ca_cert.PublicKey.Key))
				{
					return X509ChainStatusFlags.RevocationStatusUnknown;
				}
				X509Crl.X509CrlEntry crlEntry = x509Crl.GetCrlEntry(certificate.MonoCertificate);
				if (crlEntry != null)
				{
					if (!ProcessCrlEntryExtensions(crlEntry))
					{
						return X509ChainStatusFlags.Revoked;
					}
					if (crlEntry.RevocationDate <= ChainPolicy.VerificationTime)
					{
						return X509ChainStatusFlags.Revoked;
					}
				}
				if (x509Crl.NextUpdate < ChainPolicy.VerificationTime)
				{
					return X509ChainStatusFlags.RevocationStatusUnknown | X509ChainStatusFlags.OfflineRevocation;
				}
				if (!ProcessCrlExtensions(x509Crl))
				{
					return X509ChainStatusFlags.RevocationStatusUnknown;
				}
				return X509ChainStatusFlags.NoError;
			}
			return X509ChainStatusFlags.RevocationStatusUnknown;
		}

		private X509Crl FindCrl(X509Certificate2 caCertificate)
		{
			string text = caCertificate.SubjectName.Decode(X500DistinguishedNameFlags.None);
			string subjectKeyIdentifier = GetSubjectKeyIdentifier(caCertificate);
			foreach (X509Crl crl in CertificateAuthorities.Store.Crls)
			{
				if (crl.IssuerName == text && (subjectKeyIdentifier.Length == 0 || subjectKeyIdentifier == GetAuthorityKeyIdentifier(crl)))
				{
					return crl;
				}
			}
			foreach (X509Crl crl2 in Roots.Store.Crls)
			{
				if (crl2.IssuerName == text && (subjectKeyIdentifier.Length == 0 || subjectKeyIdentifier == GetAuthorityKeyIdentifier(crl2)))
				{
					return crl2;
				}
			}
			return null;
		}

		private bool ProcessCrlExtensions(X509Crl crl)
		{
			foreach (Mono.Security.X509.X509Extension extension in crl.Extensions)
			{
				if (extension.Critical)
				{
					switch (extension.Oid)
					{
					case "2.5.29.20":
					case "2.5.29.35":
						continue;
					}
					return false;
				}
			}
			return true;
		}

		private bool ProcessCrlEntryExtensions(X509Crl.X509CrlEntry entry)
		{
			foreach (Mono.Security.X509.X509Extension extension in entry.Extensions)
			{
				if (extension.Critical)
				{
					switch (extension.Oid)
					{
					case "2.5.29.21":
						continue;
					}
					return false;
				}
			}
			return true;
		}
	}
}
