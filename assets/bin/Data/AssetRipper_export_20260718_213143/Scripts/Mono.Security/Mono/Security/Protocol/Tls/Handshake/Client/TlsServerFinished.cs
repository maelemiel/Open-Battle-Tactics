using System.Security.Cryptography;
using Mono.Security.Cryptography;

namespace Mono.Security.Protocol.Tls.Handshake.Client
{
	internal class TlsServerFinished : HandshakeMessage
	{
		private static byte[] Ssl3Marker = new byte[4] { 83, 82, 86, 82 };

		public TlsServerFinished(Context context, byte[] buffer)
			: base(context, HandshakeType.Finished, buffer)
		{
		}

		public override void Update()
		{
			base.Update();
			base.Context.HandshakeState = HandshakeState.Finished;
		}

		protected override void ProcessAsSsl3()
		{
			HashAlgorithm hashAlgorithm = new SslHandshakeHash(base.Context.MasterSecret);
			byte[] array = base.Context.HandshakeMessages.ToArray();
			hashAlgorithm.TransformBlock(array, 0, array.Length, array, 0);
			hashAlgorithm.TransformBlock(Ssl3Marker, 0, Ssl3Marker.Length, Ssl3Marker, 0);
			hashAlgorithm.TransformFinalBlock(CipherSuite.EmptyArray, 0, 0);
			byte[] buffer = ReadBytes((int)Length);
			byte[] hash = hashAlgorithm.Hash;
			if (!HandshakeMessage.Compare(hash, buffer))
			{
				throw new TlsException(AlertDescription.InsuficientSecurity, "Invalid ServerFinished message received.");
			}
		}

		protected override void ProcessAsTls1()
		{
			byte[] buffer = ReadBytes((int)Length);
			HashAlgorithm hashAlgorithm = new MD5SHA1();
			byte[] array = base.Context.HandshakeMessages.ToArray();
			byte[] data = hashAlgorithm.ComputeHash(array, 0, array.Length);
			byte[] buffer2 = base.Context.Current.Cipher.PRF(base.Context.MasterSecret, "server finished", data, 12);
			if (!HandshakeMessage.Compare(buffer2, buffer))
			{
				throw new TlsException("Invalid ServerFinished message received.");
			}
		}
	}
}
