using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Mono.Security.Protocol.Tls.Handshake;
using Mono.Security.X509;

namespace Mono.Security.Protocol.Tls
{
	public class SslClientStream : SslStreamBase
	{
		internal Stream InputBuffer
		{
			get
			{
				return inputBuffer;
			}
		}

		public System.Security.Cryptography.X509Certificates.X509CertificateCollection ClientCertificates
		{
			get
			{
				return context.ClientSettings.Certificates;
			}
		}

		public System.Security.Cryptography.X509Certificates.X509Certificate SelectedClientCertificate
		{
			get
			{
				return context.ClientSettings.ClientCertificate;
			}
		}

		public CertificateValidationCallback ServerCertValidationDelegate
		{
			get
			{
				return this.ServerCertValidation;
			}
			set
			{
				this.ServerCertValidation = value;
			}
		}

		public CertificateSelectionCallback ClientCertSelectionDelegate
		{
			get
			{
				return this.ClientCertSelection;
			}
			set
			{
				this.ClientCertSelection = value;
			}
		}

		public PrivateKeySelectionCallback PrivateKeyCertSelectionDelegate
		{
			get
			{
				return this.PrivateKeySelection;
			}
			set
			{
				this.PrivateKeySelection = value;
			}
		}

		internal override bool HaveRemoteValidation2Callback
		{
			get
			{
				return this.ServerCertValidation2 != null;
			}
		}

		internal event CertificateValidationCallback ServerCertValidation;

		internal event CertificateSelectionCallback ClientCertSelection;

		internal event PrivateKeySelectionCallback PrivateKeySelection;

		public event CertificateValidationCallback2 ServerCertValidation2;

		public SslClientStream(Stream stream, string targetHost, bool ownsStream)
			: this(stream, targetHost, ownsStream, SecurityProtocolType.Default, null)
		{
		}

		public SslClientStream(Stream stream, string targetHost, System.Security.Cryptography.X509Certificates.X509Certificate clientCertificate)
			: this(stream, targetHost, false, SecurityProtocolType.Default, new System.Security.Cryptography.X509Certificates.X509CertificateCollection(new System.Security.Cryptography.X509Certificates.X509Certificate[1] { clientCertificate }))
		{
		}

		public SslClientStream(Stream stream, string targetHost, System.Security.Cryptography.X509Certificates.X509CertificateCollection clientCertificates)
			: this(stream, targetHost, false, SecurityProtocolType.Default, clientCertificates)
		{
		}

		public SslClientStream(Stream stream, string targetHost, bool ownsStream, SecurityProtocolType securityProtocolType)
			: this(stream, targetHost, ownsStream, securityProtocolType, new System.Security.Cryptography.X509Certificates.X509CertificateCollection())
		{
		}

		public SslClientStream(Stream stream, string targetHost, bool ownsStream, SecurityProtocolType securityProtocolType, System.Security.Cryptography.X509Certificates.X509CertificateCollection clientCertificates)
			: base(stream, ownsStream)
		{
			if (targetHost == null || targetHost.Length == 0)
			{
				throw new ArgumentNullException("targetHost is null or an empty string.");
			}
			context = new ClientContext(this, securityProtocolType, targetHost, clientCertificates);
			protocol = new ClientRecordProtocol(innerStream, (ClientContext)context);
		}

		~SslClientStream()
		{
			base.Dispose(false);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				this.ServerCertValidation = null;
				this.ClientCertSelection = null;
				this.PrivateKeySelection = null;
				this.ServerCertValidation2 = null;
			}
		}

		internal override IAsyncResult OnBeginNegotiateHandshake(AsyncCallback callback, object state)
		{
			try
			{
				if (context.HandshakeState != HandshakeState.None)
				{
					context.Clear();
				}
				context.SupportedCiphers = CipherSuiteFactory.GetSupportedCiphers(context.SecurityProtocol);
				context.HandshakeState = HandshakeState.Started;
				return protocol.BeginSendRecord(HandshakeType.ClientHello, callback, state);
			}
			catch (TlsException ex)
			{
				protocol.SendAlert(ex.Alert);
				throw new IOException("The authentication or decryption has failed.", ex);
			}
			catch (Exception innerException)
			{
				protocol.SendAlert(AlertDescription.InternalError);
				throw new IOException("The authentication or decryption has failed.", innerException);
			}
		}

		private void SafeReceiveRecord(Stream s)
		{
			byte[] array = protocol.ReceiveRecord(s);
			if (array == null || array.Length == 0)
			{
				throw new TlsException(AlertDescription.HandshakeFailiure, "The server stopped the handshake.");
			}
		}

		internal override void OnNegotiateHandshakeCallback(IAsyncResult asyncResult)
		{
			protocol.EndSendRecord(asyncResult);
			while (context.LastHandshakeMsg != HandshakeType.ServerHelloDone)
			{
				SafeReceiveRecord(innerStream);
				if (context.AbbreviatedHandshake && context.LastHandshakeMsg == HandshakeType.ServerHello)
				{
					break;
				}
			}
			if (context.AbbreviatedHandshake)
			{
				ClientSessionCache.SetContextFromCache(context);
				context.Negotiating.Cipher.ComputeKeys();
				context.Negotiating.Cipher.InitializeCipher();
				protocol.SendChangeCipherSpec();
				while (context.HandshakeState != HandshakeState.Finished)
				{
					SafeReceiveRecord(innerStream);
				}
				protocol.SendRecord(HandshakeType.Finished);
			}
			else
			{
				bool flag = context.ServerSettings.CertificateRequest;
				if (context.SecurityProtocol == SecurityProtocolType.Ssl3)
				{
					flag = context.ClientSettings.Certificates != null && context.ClientSettings.Certificates.Count > 0;
				}
				if (flag)
				{
					protocol.SendRecord(HandshakeType.Certificate);
				}
				protocol.SendRecord(HandshakeType.ClientKeyExchange);
				context.Negotiating.Cipher.InitializeCipher();
				if (flag && context.ClientSettings.ClientCertificate != null)
				{
					protocol.SendRecord(HandshakeType.CertificateVerify);
				}
				protocol.SendChangeCipherSpec();
				protocol.SendRecord(HandshakeType.Finished);
				while (context.HandshakeState != HandshakeState.Finished)
				{
					SafeReceiveRecord(innerStream);
				}
			}
			context.HandshakeMessages.Reset();
			context.ClearKeyInfo();
		}

		internal override System.Security.Cryptography.X509Certificates.X509Certificate OnLocalCertificateSelection(System.Security.Cryptography.X509Certificates.X509CertificateCollection clientCertificates, System.Security.Cryptography.X509Certificates.X509Certificate serverCertificate, string targetHost, System.Security.Cryptography.X509Certificates.X509CertificateCollection serverRequestedCertificates)
		{
			if (this.ClientCertSelection != null)
			{
				return this.ClientCertSelection(clientCertificates, serverCertificate, targetHost, serverRequestedCertificates);
			}
			return null;
		}

		internal override ValidationResult OnRemoteCertificateValidation2(Mono.Security.X509.X509CertificateCollection collection)
		{
			CertificateValidationCallback2 serverCertValidation = this.ServerCertValidation2;
			if (serverCertValidation != null)
			{
				return serverCertValidation(collection);
			}
			return null;
		}

		internal override bool OnRemoteCertificateValidation(System.Security.Cryptography.X509Certificates.X509Certificate certificate, int[] errors)
		{
			if (this.ServerCertValidation != null)
			{
				return this.ServerCertValidation(certificate, errors);
			}
			return errors != null && errors.Length == 0;
		}

		internal virtual bool RaiseServerCertificateValidation(System.Security.Cryptography.X509Certificates.X509Certificate certificate, int[] certificateErrors)
		{
			return RaiseRemoteCertificateValidation(certificate, certificateErrors);
		}

		internal virtual ValidationResult RaiseServerCertificateValidation2(Mono.Security.X509.X509CertificateCollection collection)
		{
			return RaiseRemoteCertificateValidation2(collection);
		}

		internal System.Security.Cryptography.X509Certificates.X509Certificate RaiseClientCertificateSelection(System.Security.Cryptography.X509Certificates.X509CertificateCollection clientCertificates, System.Security.Cryptography.X509Certificates.X509Certificate serverCertificate, string targetHost, System.Security.Cryptography.X509Certificates.X509CertificateCollection serverRequestedCertificates)
		{
			return RaiseLocalCertificateSelection(clientCertificates, serverCertificate, targetHost, serverRequestedCertificates);
		}

		internal override AsymmetricAlgorithm OnLocalPrivateKeySelection(System.Security.Cryptography.X509Certificates.X509Certificate certificate, string targetHost)
		{
			if (this.PrivateKeySelection != null)
			{
				return this.PrivateKeySelection(certificate, targetHost);
			}
			return null;
		}

		internal AsymmetricAlgorithm RaisePrivateKeySelection(System.Security.Cryptography.X509Certificates.X509Certificate certificate, string targetHost)
		{
			return RaiseLocalPrivateKeySelection(certificate, targetHost);
		}
	}
}
