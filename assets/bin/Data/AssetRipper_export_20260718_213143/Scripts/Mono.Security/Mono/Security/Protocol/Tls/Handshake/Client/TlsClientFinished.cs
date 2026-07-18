using System.Security.Cryptography;
using Mono.Security.Cryptography;

namespace Mono.Security.Protocol.Tls.Handshake.Client
{
	internal class TlsClientFinished : HandshakeMessage
	{
		private static byte[] Ssl3Marker = new byte[4] { 67, 76, 78, 84 };

		public TlsClientFinished(Context context)
			: base(context, HandshakeType.Finished)
		{
		}

		public override void Update()
		{
			base.Update();
			Reset();
		}

		protected override void ProcessAsSsl3()
		{
			HashAlgorithm hashAlgorithm = new SslHandshakeHash(base.Context.MasterSecret);
			byte[] array = base.Context.HandshakeMessages.ToArray();
			hashAlgorithm.TransformBlock(array, 0, array.Length, array, 0);
			hashAlgorithm.TransformBlock(Ssl3Marker, 0, Ssl3Marker.Length, Ssl3Marker, 0);
			hashAlgorithm.TransformFinalBlock(CipherSuite.EmptyArray, 0, 0);
			Write(hashAlgorithm.Hash);
		}

		protected override void ProcessAsTls1()
		{
			HashAlgorithm hashAlgorithm = new MD5SHA1();
			byte[] array = base.Context.HandshakeMessages.ToArray();
			byte[] data = hashAlgorithm.ComputeHash(array, 0, array.Length);
			Write(base.Context.Write.Cipher.PRF(base.Context.MasterSecret, "client finished", data, 12));
		}
	}
}
