using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using Mono.Security.X509;
using Mono.Security.X509.Extensions;

namespace Mono.Security.Protocol.Tls.Handshake.Server
{
	internal class TlsClientCertificate : HandshakeMessage
	{
		private Mono.Security.X509.X509CertificateCollection clientCertificates;

		public TlsClientCertificate(Context context, byte[] buffer)
			: base(context, HandshakeType.Certificate, buffer)
		{
		}

		public override void Update()
		{
			foreach (Mono.Security.X509.X509Certificate clientCertificate in clientCertificates)
			{
				base.Context.ClientSettings.Certificates.Add(new System.Security.Cryptography.X509Certificates.X509Certificate(clientCertificate.RawData));
			}
		}

		protected override void ProcessAsSsl3()
		{
			ProcessAsTls1();
		}

		protected override void ProcessAsTls1()
		{
			int num = 0;
			int num2 = ReadInt24();
			clientCertificates = new Mono.Security.X509.X509CertificateCollection();
			while (num2 > num)
			{
				int num3 = ReadInt24();
				num += num3 + 3;
				byte[] data = ReadBytes(num3);
				clientCertificates.Add(new Mono.Security.X509.X509Certificate(data));
			}
			if (clientCertificates.Count > 0)
			{
				validateCertificates(clientCertificates);
			}
			else if ((base.Context as ServerContext).ClientCertificateRequired)
			{
				throw new TlsException(AlertDescription.NoCertificate);
			}
		}

		private bool checkCertificateUsage(Mono.Security.X509.X509Certificate cert)
		{
			ServerContext serverContext = (ServerContext)base.Context;
			if (cert.Version < 3)
			{
				return true;
			}
			KeyUsages usage = KeyUsages.none;
			switch (serverContext.Negotiating.Cipher.ExchangeAlgorithmType)
			{
			case ExchangeAlgorithmType.RsaKeyX:
			case ExchangeAlgorithmType.RsaSign:
				usage = KeyUsages.digitalSignature;
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
				return keyUsageExtension.Support(usage) && extendedKeyUsageExtension.KeyPurpose.Contains("1.3.6.1.5.5.7.3.2");
			}
			if (keyUsageExtension != null)
			{
				return keyUsageExtension.Support(usage);
			}
			if (extendedKeyUsageExtension != null)
			{
				return extendedKeyUsageExtension.KeyPurpose.Contains("1.3.6.1.5.5.7.3.2");
			}
			x509Extension = cert.Extensions["2.16.840.1.113730.1.1"];
			if (x509Extension != null)
			{
				NetscapeCertTypeExtension netscapeCertTypeExtension = new NetscapeCertTypeExtension(x509Extension);
				return netscapeCertTypeExtension.Support(NetscapeCertTypeExtension.CertTypes.SslClient);
			}
			return false;
		}

		private void validateCertificates(Mono.Security.X509.X509CertificateCollection certificates)
		{
			ServerContext serverContext = (ServerContext)base.Context;
			AlertDescription description = AlertDescription.BadCertificate;
			System.Security.Cryptography.X509Certificates.X509Certificate x509Certificate = null;
			int[] array = null;
			if (certificates.Count > 0)
			{
				Mono.Security.X509.X509Certificate x509Certificate2 = certificates[0];
				ArrayList arrayList = new ArrayList();
				if (!checkCertificateUsage(x509Certificate2))
				{
					arrayList.Add(-2146762490);
				}
				Mono.Security.X509.X509Chain x509Chain;
				if (certificates.Count > 1)
				{
					Mono.Security.X509.X509CertificateCollection x509CertificateCollection = new Mono.Security.X509.X509CertificateCollection(certificates);
					x509CertificateCollection.Remove(x509Certificate2);
					x509Chain = new Mono.Security.X509.X509Chain(x509CertificateCollection);
				}
				else
				{
					x509Chain = new Mono.Security.X509.X509Chain();
				}
				bool flag = false;
				try
				{
					flag = x509Chain.Build(x509Certificate2);
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
				x509Certificate = new System.Security.Cryptography.X509Certificates.X509Certificate(x509Certificate2.RawData);
				array = (int[])arrayList.ToArray(typeof(int));
			}
			else
			{
				array = new int[0];
			}
			System.Security.Cryptography.X509Certificates.X509CertificateCollection x509CertificateCollection2 = new System.Security.Cryptography.X509Certificates.X509CertificateCollection();
			foreach (Mono.Security.X509.X509Certificate certificate in certificates)
			{
				x509CertificateCollection2.Add(new System.Security.Cryptography.X509Certificates.X509Certificate(certificate.RawData));
			}
			if (!serverContext.SslStream.RaiseClientCertificateValidation(x509Certificate, array))
			{
				throw new TlsException(description, "Invalid certificate received from client.");
			}
			base.Context.ClientSettings.ClientCertificate = x509Certificate;
		}
	}
}
