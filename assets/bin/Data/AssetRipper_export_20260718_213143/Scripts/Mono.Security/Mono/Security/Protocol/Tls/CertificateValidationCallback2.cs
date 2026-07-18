using Mono.Security.X509;

namespace Mono.Security.Protocol.Tls
{
	public delegate ValidationResult CertificateValidationCallback2(X509CertificateCollection collection);
}
