using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Mono.Security.Protocol.Tls
{
	internal class HttpsClientStream : SslClientStream
	{
		private HttpWebRequest _request;

		private int _status;

		public bool TrustFailure
		{
			get
			{
				int status = _status;
				if (status == -2146762487 || status == -2146762486)
				{
					return true;
				}
				return false;
			}
		}

		public HttpsClientStream(Stream stream, X509CertificateCollection clientCertificates, HttpWebRequest request, byte[] buffer)
			: base(stream, request.Address.Host, false, (SecurityProtocolType)ServicePointManager.SecurityProtocol, clientCertificates)
		{
			_request = request;
			_status = 0;
			if (buffer != null)
			{
				base.InputBuffer.Write(buffer, 0, buffer.Length);
			}
			base.CheckCertRevocationStatus = ServicePointManager.CheckCertificateRevocationList;
			base.ClientCertSelection += (X509CertificateCollection clientCerts, X509Certificate serverCertificate, string targetHost, X509CertificateCollection serverRequestedCertificates) => (clientCerts != null && clientCerts.Count != 0) ? clientCerts[0] : null;
			base.PrivateKeySelection += delegate(X509Certificate certificate, string targetHost)
			{
				X509Certificate2 x509Certificate = certificate as X509Certificate2;
				return (x509Certificate != null) ? x509Certificate.PrivateKey : null;
			};
		}

		internal override bool RaiseServerCertificateValidation(X509Certificate certificate, int[] certificateErrors)
		{
			bool flag = certificateErrors.Length > 0;
			_status = (flag ? certificateErrors[0] : 0);
			if (ServicePointManager.CertificatePolicy != null)
			{
				ServicePoint servicePoint = _request.ServicePoint;
				if (!ServicePointManager.CertificatePolicy.CheckValidationResult(servicePoint, certificate, _request, _status))
				{
					return false;
				}
				flag = true;
			}
			if (HaveRemoteValidation2Callback)
			{
				return flag;
			}
			RemoteCertificateValidationCallback serverCertificateValidationCallback = ServicePointManager.ServerCertificateValidationCallback;
			if (serverCertificateValidationCallback != null)
			{
				SslPolicyErrors sslPolicyErrors = SslPolicyErrors.None;
				for (int i = 0; i < certificateErrors.Length; i++)
				{
					switch (certificateErrors[i])
					{
					case -2146762490:
						sslPolicyErrors |= SslPolicyErrors.RemoteCertificateNotAvailable;
						break;
					case -2146762481:
						sslPolicyErrors |= SslPolicyErrors.RemoteCertificateNameMismatch;
						break;
					default:
						sslPolicyErrors |= SslPolicyErrors.RemoteCertificateChainErrors;
						break;
					}
				}
				X509Certificate2 certificate2 = new X509Certificate2(certificate.GetRawCertData());
				X509Chain x509Chain = new X509Chain();
				if (!x509Chain.Build(certificate2))
				{
					sslPolicyErrors |= SslPolicyErrors.RemoteCertificateChainErrors;
				}
				return serverCertificateValidationCallback(_request, certificate2, x509Chain, sslPolicyErrors);
			}
			return flag;
		}
	}
}
