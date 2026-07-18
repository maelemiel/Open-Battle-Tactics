using System.Security.Cryptography;
using Mono.Security.Cryptography;

namespace Mono.Security.Protocol.Tls.Handshake.Client
{
	internal class TlsClientKeyExchange : HandshakeMessage
	{
		public TlsClientKeyExchange(Context context)
			: base(context, HandshakeType.ClientKeyExchange)
		{
		}

		protected override void ProcessAsSsl3()
		{
			ProcessCommon(false);
		}

		protected override void ProcessAsTls1()
		{
			ProcessCommon(true);
		}

		public void ProcessCommon(bool sendLength)
		{
			byte[] array = base.Context.Negotiating.Cipher.CreatePremasterSecret();
			RSA rSA = null;
			if (base.Context.ServerSettings.ServerKeyExchange)
			{
				rSA = new RSAManaged();
				rSA.ImportParameters(base.Context.ServerSettings.RsaParameters);
			}
			else
			{
				rSA = base.Context.ServerSettings.CertificateRSA;
			}
			RSAPKCS1KeyExchangeFormatter rSAPKCS1KeyExchangeFormatter = new RSAPKCS1KeyExchangeFormatter(rSA);
			byte[] array2 = rSAPKCS1KeyExchangeFormatter.CreateKeyExchange(array);
			if (sendLength)
			{
				Write((short)array2.Length);
			}
			Write(array2);
			base.Context.Negotiating.Cipher.ComputeMasterSecret(array);
			base.Context.Negotiating.Cipher.ComputeKeys();
			rSA.Clear();
		}
	}
}
