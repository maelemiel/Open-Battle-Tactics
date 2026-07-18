using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Mono.Security.Cryptography;

namespace Mono.Security.Protocol.Tls.Handshake.Server
{
	internal class TlsServerKeyExchange : HandshakeMessage
	{
		public TlsServerKeyExchange(Context context)
			: base(context, HandshakeType.ServerKeyExchange)
		{
		}

		public override void Update()
		{
			throw new NotSupportedException();
		}

		protected override void ProcessAsSsl3()
		{
			ProcessAsTls1();
		}

		protected override void ProcessAsTls1()
		{
			ServerContext serverContext = (ServerContext)base.Context;
			RSA rSA = (RSA)serverContext.SslStream.PrivateKeyCertSelectionDelegate(new X509Certificate(serverContext.ServerSettings.Certificates[0].RawData), null);
			RSAParameters rSAParameters = rSA.ExportParameters(false);
			WriteInt24(rSAParameters.Modulus.Length);
			Write(rSAParameters.Modulus, 0, rSAParameters.Modulus.Length);
			WriteInt24(rSAParameters.Exponent.Length);
			Write(rSAParameters.Exponent, 0, rSAParameters.Exponent.Length);
			byte[] array = createSignature(rSA, ToArray());
			WriteInt24(array.Length);
			Write(array);
		}

		private byte[] createSignature(RSA rsa, byte[] buffer)
		{
			MD5SHA1 mD5SHA = new MD5SHA1();
			TlsStream tlsStream = new TlsStream();
			tlsStream.Write(base.Context.RandomCS);
			tlsStream.Write(buffer, 0, buffer.Length);
			mD5SHA.ComputeHash(tlsStream.ToArray());
			tlsStream.Reset();
			return mD5SHA.CreateSignature(rsa);
		}
	}
}
