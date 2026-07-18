using System.Security.Cryptography;
using Mono.Security.Cryptography;

namespace Mono.Security.Protocol.Tls.Handshake.Client
{
	internal class TlsServerKeyExchange : HandshakeMessage
	{
		private RSAParameters rsaParams;

		private byte[] signedParams;

		public TlsServerKeyExchange(Context context, byte[] buffer)
			: base(context, HandshakeType.ServerKeyExchange, buffer)
		{
			verifySignature();
		}

		public override void Update()
		{
			base.Update();
			base.Context.ServerSettings.ServerKeyExchange = true;
			base.Context.ServerSettings.RsaParameters = rsaParams;
			base.Context.ServerSettings.SignedParams = signedParams;
		}

		protected override void ProcessAsSsl3()
		{
			ProcessAsTls1();
		}

		protected override void ProcessAsTls1()
		{
			rsaParams = default(RSAParameters);
			rsaParams.Modulus = ReadBytes(ReadInt16());
			rsaParams.Exponent = ReadBytes(ReadInt16());
			signedParams = ReadBytes(ReadInt16());
		}

		private void verifySignature()
		{
			MD5SHA1 mD5SHA = new MD5SHA1();
			int count = rsaParams.Modulus.Length + rsaParams.Exponent.Length + 4;
			TlsStream tlsStream = new TlsStream();
			tlsStream.Write(base.Context.RandomCS);
			tlsStream.Write(ToArray(), 0, count);
			mD5SHA.ComputeHash(tlsStream.ToArray());
			tlsStream.Reset();
			if (!mD5SHA.VerifySignature(base.Context.ServerSettings.CertificateRSA, signedParams))
			{
				throw new TlsException(AlertDescription.DecodeError, "Data was not signed with the server certificate.");
			}
		}
	}
}
