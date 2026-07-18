using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Mono.Security.Protocol.Tls.Handshake.Server
{
	internal class TlsClientKeyExchange : HandshakeMessage
	{
		public TlsClientKeyExchange(Context context, byte[] buffer)
			: base(context, HandshakeType.ClientKeyExchange, buffer)
		{
		}

		protected override void ProcessAsSsl3()
		{
			AsymmetricAlgorithm asymmetricAlgorithm = null;
			ServerContext serverContext = (ServerContext)base.Context;
			asymmetricAlgorithm = serverContext.SslStream.RaisePrivateKeySelection(new X509Certificate(serverContext.ServerSettings.Certificates[0].RawData), null);
			if (asymmetricAlgorithm == null)
			{
				throw new TlsException(AlertDescription.UserCancelled, "Server certificate Private Key unavailable.");
			}
			byte[] rgbIn = ReadBytes((int)Length);
			RSAPKCS1KeyExchangeDeformatter rSAPKCS1KeyExchangeDeformatter = new RSAPKCS1KeyExchangeDeformatter(asymmetricAlgorithm);
			byte[] preMasterSecret = rSAPKCS1KeyExchangeDeformatter.DecryptKeyExchange(rgbIn);
			base.Context.Negotiating.Cipher.ComputeMasterSecret(preMasterSecret);
			base.Context.Negotiating.Cipher.ComputeKeys();
			base.Context.Negotiating.Cipher.InitializeCipher();
		}

		protected override void ProcessAsTls1()
		{
			AsymmetricAlgorithm asymmetricAlgorithm = null;
			ServerContext serverContext = (ServerContext)base.Context;
			asymmetricAlgorithm = serverContext.SslStream.RaisePrivateKeySelection(new X509Certificate(serverContext.ServerSettings.Certificates[0].RawData), null);
			if (asymmetricAlgorithm == null)
			{
				throw new TlsException(AlertDescription.UserCancelled, "Server certificate Private Key unavailable.");
			}
			byte[] rgbIn = ReadBytes(ReadInt16());
			RSAPKCS1KeyExchangeDeformatter rSAPKCS1KeyExchangeDeformatter = new RSAPKCS1KeyExchangeDeformatter(asymmetricAlgorithm);
			byte[] preMasterSecret = rSAPKCS1KeyExchangeDeformatter.DecryptKeyExchange(rgbIn);
			base.Context.Negotiating.Cipher.ComputeMasterSecret(preMasterSecret);
			base.Context.Negotiating.Cipher.ComputeKeys();
			base.Context.Negotiating.Cipher.InitializeCipher();
		}
	}
}
