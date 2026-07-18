using System.Runtime.InteropServices;

namespace System.Security.Cryptography.X509Certificates
{
	[ComVisible(true)]
	public enum X509ContentType
	{
		Unknown = 0,
		Cert = 1,
		SerializedCert = 2,
		Pfx = 3,
		SerializedStore = 4,
		Pkcs7 = 5,
		Authenticode = 6,
		Pkcs12 = 3
	}
}
