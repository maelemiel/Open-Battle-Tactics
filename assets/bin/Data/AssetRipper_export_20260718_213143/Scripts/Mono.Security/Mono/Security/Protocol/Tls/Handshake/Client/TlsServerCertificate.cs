using System;
using System.Collections;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Mono.Security.X509;
using Mono.Security.X509.Extensions;

namespace Mono.Security.Protocol.Tls.Handshake.Client
{
	internal class TlsServerCertificate : HandshakeMessage
	{
		private Mono.Security.X509.X509CertificateCollection certificates;

		public TlsServerCertificate(Context context, byte[] buffer)
			: base(context, HandshakeType.Certificate, buffer)
		{
		}

		public override void Update()
		{
			base.Update();
			base.Context.ServerSettings.Certificates = certificates;
			base.Context.ServerSettings.UpdateCertificateRSA();
		}

		protected override void ProcessAsSsl3()
		{
			ProcessAsTls1();
		}

		protected override void ProcessAsTls1()
		{
			certificates = new Mono.Security.X509.X509CertificateCollection();
			int num = 0;
			int num2 = ReadInt24();
			while (num < num2)
			{
				int num3 = ReadInt24();
				num += 3;
				if (num3 > 0)
				{
					byte[] data = ReadBytes(num3);
					Mono.Security.X509.X509Certificate value = new Mono.Security.X509.X509Certificate(data);
					certificates.Add(value);
					num += num3;
				}
			}
			validateCertificates(certificates);
		}

		private bool checkCertificateUsage(Mono.Security.X509.X509Certificate cert)
		{
			ClientContext clientContext = (ClientContext)base.Context;
			if (cert.Version < 3)
			{
				return true;
			}
			KeyUsages usage = KeyUsages.none;
			switch (clientContext.Negotiating.Cipher.ExchangeAlgorithmType)
			{
			case ExchangeAlgorithmType.RsaSign:
				usage = KeyUsages.digitalSignature;
				break;
			case ExchangeAlgorithmType.RsaKeyX:
				usage = KeyUsages.keyEncipherment;
				break;
			case ExchangeAlgorithmType.DiffieHellman:
				usage = KeyUsages.keyAgreement;
				break;
			case ExchangeAlgorithmType.Fortezza:
				return false;
			}
			KeyUsageExtension keyUsageExtension = null;
			ExtendedKeyUsageExtension extendedKeyUsageExtension = null;
			Mono.Security.X509.X509Extension x509Extension = cert.Extensions["2.5.29.15"];
			if (x509Extension != null)
			{
				keyUsageExtension = new KeyUsageExtension(x509Extension);
			}
			x509Extension = cert.Extensions["2.5.29.37"];
			if (x509Extension != null)
			{
				extendedKeyUsageExtension = new ExtendedKeyUsageExtension(x509Extension);
			}
			if (keyUsageExtension != null && extendedKeyUsageExtension != null)
			{
				if (!keyUsageExtension.Support(usage))
				{
					return false;
				}
				return extendedKeyUsageExtension.KeyPurpose.Contains("1.3.6.1.5.5.7.3.1") || extendedKeyUsageExtension.KeyPurpose.Contains("2.16.840.1.113730.4.1");
			}
			if (keyUsageExtension != null)
			{
				return keyUsageExtension.Support(usage);
			}
			if (extendedKeyUsageExtension != null)
			{
				return extendedKeyUsageExtension.KeyPurpose.Contains("1.3.6.1.5.5.7.3.1") || extendedKeyUsageExtension.KeyPurpose.Contains("2.16.840.1.113730.4.1");
			}
			x509Extension = cert.Extensions["2.16.840.1.113730.1.1"];
			if (x509Extension != null)
			{
				NetscapeCertTypeExtension netscapeCertTypeExtension = new NetscapeCertTypeExtension(x509Extension);
				return netscapeCertTypeExtension.Support(NetscapeCertTypeExtension.CertTypes.SslServer);
			}
			return true;
		}

		private static void VerifyOSX(Mono.Security.X509.X509CertificateCollection certificates)
		{
		}

		private void validateCertificates(Mono.Security.X509.X509CertificateCollection certificates)
		{
			ClientContext clientContext = (ClientContext)base.Context;
			AlertDescription description = AlertDescription.BadCertificate;
			if (clientContext.SslStream.HaveRemoteValidation2Callback)
			{
				ValidationResult validationResult = clientContext.SslStream.RaiseServerCertificateValidation2(certificates);
				if (!validationResult.Trusted)
				{
					long num = validationResult.ErrorCode;
					switch (num)
					{
					case 2148204801L:
						description = AlertDescription.CertificateExpired;
						break;
					case 2148204810L:
						description = AlertDescription.UnknownCA;
						break;
					case 2148204809L:
						description = AlertDescription.UnknownCA;
						break;
					default:
						description = AlertDescription.CertificateUnknown;
						break;
					}
					string text = string.Format("0x{0:x}", num);
					throw new TlsException(description, "Invalid certificate received from server. Error code: " + text);
				}
				return;
			}
			Mono.Security.X509.X509Certificate x509Certificate = certificates[0];
			System.Security.Cryptography.X509Certificates.X509Certificate certificate = new System.Security.Cryptography.X509Certificates.X509Certificate(x509Certificate.RawData);
			ArrayList arrayList = new ArrayList();
			if (!checkCertificateUsage(x509Certificate))
			{
				arrayList.Add(-2146762490);
			}
			if (!checkServerIdentity(x509Certificate))
			{
				arrayList.Add(-2146762481);
			}
			Mono.Security.X509.X509CertificateCollection x509CertificateCollection = new Mono.Security.X509.X509CertificateCollection(certificates);
			x509CertificateCollection.Remove(x509Certificate);
			Mono.Security.X509.X509Chain x509Chain = new Mono.Security.X509.X509Chain(x509CertificateCollection);
			bool flag = false;
			try
			{
				flag = x509Chain.Build(x509Certificate);
			}
			catch (Exception)
			{
				flag = false;
			}
			if (!flag)
			{
				switch (x509Chain.Status)
				{
				case Mono.Security.X509.X509ChainStatusFlags.InvalidBasicConstraints:
					arrayList.Add(-2146869223);
					break;
				case Mono.Security.X509.X509ChainStatusFlags.NotSignatureValid:
					arrayList.Add(-2146869232);
					break;
				case Mono.Security.X509.X509ChainStatusFlags.NotTimeNested:
					arrayList.Add(-2146762494);
					break;
				case Mono.Security.X509.X509ChainStatusFlags.NotTimeValid:
					description = AlertDescription.CertificateExpired;
					arrayList.Add(-2146762495);
					break;
				case Mono.Security.X509.X509ChainStatusFlags.PartialChain:
					description = AlertDescription.UnknownCA;
					arrayList.Add(-2146762486);
					break;
				case Mono.Security.X509.X509ChainStatusFlags.UntrustedRoot:
					description = AlertDescription.UnknownCA;
					arrayList.Add(-2146762487);
					break;
				default:
					description = AlertDescription.CertificateUnknown;
					arrayList.Add((int)x509Chain.Status);
					break;
				}
			}
			int[] certificateErrors = (int[])arrayList.ToArray(typeof(int));
			if (!clientContext.SslStream.RaiseServerCertificateValidation(certificate, certificateErrors))
			{
				throw new TlsException(description, "Invalid certificate received from server.");
			}
		}

		private bool checkServerIdentity(Mono.Security.X509.X509Certificate cert)
		{
			ClientContext clientContext = (ClientContext)base.Context;
			string targetHost = clientContext.ClientSettings.TargetHost;
			Mono.Security.X509.X509Extension x509Extension = cert.Extensions["2.5.29.17"];
			if (x509Extension != null)
			{
				SubjectAltNameExtension subjectAltNameExtension = new SubjectAltNameExtension(x509Extension);
				string[] dNSNames = subjectAltNameExtension.DNSNames;
				foreach (string pattern in dNSNames)
				{
					if (Match(targetHost, pattern))
					{
						return true;
					}
				}
				string[] iPAddresses = subjectAltNameExtension.IPAddresses;
				foreach (string text in iPAddresses)
				{
					if (text == targetHost)
					{
						return true;
					}
				}
			}
			return checkDomainName(cert.SubjectName);
		}

		private bool checkDomainName(string subjectName)
		{
			ClientContext clientContext = (ClientContext)base.Context;
			string pattern = string.Empty;
			Regex regex = new Regex("CN\\s*=\\s*([^,]*)");
			MatchCollection matchCollection = regex.Matches(subjectName);
			if (matchCollection.Count == 1 && matchCollection[0].Success)
			{
				pattern = matchCollection[0].Groups[1].Value.ToString();
			}
			return Match(clientContext.ClientSettings.TargetHost, pattern);
		}

		private static bool Match(string hostname, string pattern)
		{
			int num = pattern.IndexOf('*');
			if (num == -1)
			{
				return string.Compare(hostname, pattern, true, CultureInfo.InvariantCulture) == 0;
			}
			if (num != pattern.Length - 1 && pattern[num + 1] != '.')
			{
				return false;
			}
			int num2 = pattern.IndexOf('*', num + 1);
			if (num2 != -1)
			{
				return false;
			}
			string text = pattern.Substring(num + 1);
			int num3 = hostname.Length - text.Length;
			if (num3 <= 0)
			{
				return false;
			}
			if (string.Compare(hostname, num3, text, 0, text.Length, true, CultureInfo.InvariantCulture) != 0)
			{
				return false;
			}
			if (num == 0)
			{
				int num4 = hostname.IndexOf('.');
				return num4 == -1 || num4 >= hostname.Length - text.Length;
			}
			string text2 = pattern.Substring(0, num);
			return string.Compare(hostname, 0, text2, 0, text2.Length, true, CultureInfo.InvariantCulture) == 0;
		}
	}
}
