using System.Security.Cryptography.X509Certificates;

namespace Mono.Security.Protocol.Tls.Handshake.Client
{
	internal class TlsClientCertificate : HandshakeMessage
	{
		private bool clientCertSelected;

		private X509Certificate clientCert;

		public X509Certificate ClientCertificate
		{
			get
			{
				if (!clientCertSelected)
				{
					GetClientCertificate();
					clientCertSelected = true;
				}
				return clientCert;
			}
		}

		public TlsClientCertificate(Context context)
			: base(context, HandshakeType.Certificate)
		{
		}

		public override void Update()
		{
			base.Update();
			Reset();
		}

		private void GetClientCertificate()
		{
			ClientContext clientContext = (ClientContext)base.Context;
			if (clientContext.ClientSettings.Certificates != null && clientContext.ClientSettings.Certificates.Count > 0)
			{
				clientCert = clientContext.SslStream.RaiseClientCertificateSelection(base.Context.ClientSettings.Certificates, new X509Certificate(base.Context.ServerSettings.Certificates[0].RawData), base.Context.ClientSettings.TargetHost, null);
			}
			clientContext.ClientSettings.ClientCertificate = clientCert;
		}

		private void SendCertificates()
		{
			TlsStream tlsStream = new TlsStream();
			for (X509Certificate x509Certificate = ClientCertificate; x509Certificate != null; x509Certificate = FindParentCertificate(x509Certificate))
			{
				byte[] rawCertData = x509Certificate.GetRawCertData();
				tlsStream.WriteInt24(rawCertData.Length);
				tlsStream.Write(rawCertData);
			}
			WriteInt24((int)tlsStream.Length);
			Write(tlsStream.ToArray());
		}

		protected override void ProcessAsSsl3()
		{
			if (ClientCertificate != null)
			{
				SendCertificates();
			}
		}

		protected override void ProcessAsTls1()
		{
			if (ClientCertificate != null)
			{
				SendCertificates();
			}
			else
			{
				WriteInt24(0);
			}
		}

		private X509Certificate FindParentCertificate(X509Certificate cert)
		{
			if (cert.GetName() == cert.GetIssuerName())
			{
				return null;
			}
			foreach (X509Certificate certificate in base.Context.ClientSettings.Certificates)
			{
				if (cert.GetName() == cert.GetIssuerName())
				{
					return certificate;
				}
			}
			return null;
		}
	}
}
