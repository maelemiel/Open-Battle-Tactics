using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Mono.Security.Protocol.Tls.Handshake;
using Mono.Security.X509;

namespace Mono.Security.Protocol.Tls
{
	public class SslServerStream : SslStreamBase
	{
		public System.Security.Cryptography.X509Certificates.X509Certificate ClientCertificate
		{
			get
			{
				if (context.HandshakeState == HandshakeState.Finished)
				{
					return context.ClientSettings.ClientCertificate;
				}
				return null;
			}
		}

		public CertificateValidationCallback ClientCertValidationDelegate
		{
			get
			{
				return this.ClientCertValidation;
			}
			set
			{
				this.ClientCertValidation = value;
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
				return this.ClientCertValidation2 != null;
			}
		}

		internal event CertificateValidationCallback ClientCertValidation;

		internal event PrivateKeySelectionCallback PrivateKeySelection;

		public event CertificateValidationCallback2 ClientCertValidation2;

		public SslServerStream(Stream stream, System.Security.Cryptography.X509Certificates.X509Certificate serverCertificate)
			: this(stream, serverCertificate, false, false, SecurityProtocolType.Default)
		{
		}

		public SslServerStream(Stream stream, System.Security.Cryptography.X509Certificates.X509Certificate serverCertificate, bool clientCertificateRequired, bool ownsStream)
			: this(stream, serverCertificate, clientCertificateRequired, ownsStream, SecurityProtocolType.Default)
		{
		}

		public SslServerStream(Stream stream, System.Security.Cryptography.X509Certificates.X509Certificate serverCertificate, bool clientCertificateRequired, bool requestClientCertificate, bool ownsStream)
			: this(stream, serverCertificate, clientCertificateRequired, requestClientCertificate, ownsStream, SecurityProtocolType.Default)
		{
		}

		public SslServerStream(Stream stream, System.Security.Cryptography.X509Certificates.X509Certificate serverCertificate, bool clientCertificateRequired, bool ownsStream, SecurityProtocolType securityProtocolType)
			: this(stream, serverCertificate, clientCertificateRequired, false, ownsStream, securityProtocolType)
		{
		}

		public SslServerStream(Stream stream, System.Security.Cryptography.X509Certificates.X509Certificate serverCertificate, bool clientCertificateRequired, bool requestClientCertificate, bool ownsStream, SecurityProtocolType securityProtocolType)
			: base(stream, ownsStream)
		{
			context = new ServerContext(this, securityProtocolType, serverCertificate, clientCertificateRequired, requestClientCertificate);
			protocol = new ServerRecordProtocol(innerStream, (ServerContext)context);
		}

		~SslServerStream()
		{
			Dispose(false);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				this.ClientCertValidation = null;
				this.PrivateKeySelection = null;
			}
		}

		internal override IAsyncResult OnBeginNegotiateHandshake(AsyncCallback callback, object state)
		{
			if (context.HandshakeState != HandshakeState.None)
			{
				context.Clear();
			}
			context.SupportedCiphers = CipherSuiteFactory.GetSupportedCiphers(context.SecurityProtocol);
			context.HandshakeState = HandshakeState.Started;
			return protocol.BeginReceiveRecord(innerStream, callback, state);
		}

		internal override void OnNegotiateHandshakeCallback(IAsyncResult asyncResult)
		{
			protocol.EndReceiveRecord(asyncResult);
			if (context.LastHandshakeMsg != HandshakeType.ClientHello)
			{
				protocol.SendAlert(AlertDescription.UnexpectedMessage);
			}
			protocol.SendRecord(HandshakeType.ServerHello);
			protocol.SendRecord(HandshakeType.Certificate);
			if (context.Negotiating.Cipher.IsExportable)
			{
				protocol.SendRecord(HandshakeType.ServerKeyExchange);
			}
			bool flag = false;
			if (context.Negotiating.Cipher.IsExportable || ((ServerContext)context).ClientCertificateRequired || ((ServerContext)context).RequestClientCertificate)
			{
				protocol.SendRecord(HandshakeType.CertificateRequest);
				flag = true;
			}
			protocol.SendRecord(HandshakeType.ServerHelloDone);
			while (context.LastHandshakeMsg != HandshakeType.Finished)
			{
				byte[] array = protocol.ReceiveRecord(innerStream);
				if (array == null || array.Length == 0)
				{
					throw new TlsException(AlertDescription.HandshakeFailiure, "The client stopped the handshake.");
				}
			}
			if (flag)
			{
				System.Security.Cryptography.X509Certificates.X509Certificate clientCertificate = context.ClientSettings.ClientCertificate;
				if (clientCertificate == null && ((ServerContext)context).ClientCertificateRequired)
				{
					throw new TlsException(AlertDescription.BadCertificate, "No certificate received from client.");
				}
				if (!RaiseClientCertificateValidation(clientCertificate, new int[0]))
				{
					throw new TlsException(AlertDescription.BadCertificate, "Client certificate not accepted.");
				}
			}
			protocol.SendChangeCipherSpec();
			protocol.SendRecord(HandshakeType.Finished);
			context.HandshakeState = HandshakeState.Finished;
			context.HandshakeMessages.Reset();
			context.ClearKeyInfo();
		}

		internal override System.Security.Cryptography.X509Certificates.X509Certificate OnLocalCertificateSelection(System.Security.Cryptography.X509Certificates.X509CertificateCollection clientCertificates, System.Security.Cryptography.X509Certificates.X509Certificate serverCertificate, string targetHost, System.Security.Cryptography.X509Certificates.X509CertificateCollection serverRequestedCertificates)
		{
			throw new NotSupportedException();
		}

		internal override bool OnRemoteCertificateValidation(System.Security.Cryptography.X509Certificates.X509Certificate certificate, int[] errors)
		{
			if (this.ClientCertValidation != null)
			{
				return this.ClientCertValidation(certificate, errors);
			}
			return errors != null && errors.Length == 0;
		}

		internal override ValidationResult OnRemoteCertificateValidation2(Mono.Security.X509.X509CertificateCollection collection)
		{
			CertificateValidationCallback2 clientCertValidation = this.ClientCertValidation2;
			if (clientCertValidation != null)
			{
				return clientCertValidation(collection);
			}
			return null;
		}

		internal bool RaiseClientCertificateValidation(System.Security.Cryptography.X509Certificates.X509Certificate certificate, int[] certificateErrors)
		{
			return RaiseRemoteCertificateValidation(certificate, certificateErrors);
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
