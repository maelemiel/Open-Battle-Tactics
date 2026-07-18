using System;

namespace Mono.Security.Protocol.Tls.Handshake.Client
{
	internal class TlsServerHello : HandshakeMessage
	{
		private SecurityCompressionType compressionMethod;

		private byte[] random;

		private byte[] sessionId;

		private CipherSuite cipherSuite;

		public TlsServerHello(Context context, byte[] buffer)
			: base(context, HandshakeType.ServerHello, buffer)
		{
		}

		public override void Update()
		{
			base.Update();
			base.Context.SessionId = sessionId;
			base.Context.ServerRandom = random;
			base.Context.Negotiating.Cipher = cipherSuite;
			base.Context.CompressionMethod = compressionMethod;
			base.Context.ProtocolNegotiated = true;
			int num = base.Context.ClientRandom.Length;
			int num2 = base.Context.ServerRandom.Length;
			int num3 = num + num2;
			byte[] array = new byte[num3];
			Buffer.BlockCopy(base.Context.ClientRandom, 0, array, 0, num);
			Buffer.BlockCopy(base.Context.ServerRandom, 0, array, num, num2);
			base.Context.RandomCS = array;
			byte[] array2 = new byte[num3];
			Buffer.BlockCopy(base.Context.ServerRandom, 0, array2, 0, num2);
			Buffer.BlockCopy(base.Context.ClientRandom, 0, array2, num2, num);
			base.Context.RandomSC = array2;
		}

		protected override void ProcessAsSsl3()
		{
			ProcessAsTls1();
		}

		protected override void ProcessAsTls1()
		{
			processProtocol(ReadInt16());
			random = ReadBytes(32);
			int num = ReadByte();
			if (num > 0)
			{
				sessionId = ReadBytes(num);
				ClientSessionCache.Add(base.Context.ClientSettings.TargetHost, sessionId);
				base.Context.AbbreviatedHandshake = HandshakeMessage.Compare(sessionId, base.Context.SessionId);
			}
			else
			{
				base.Context.AbbreviatedHandshake = false;
			}
			short code = ReadInt16();
			if (base.Context.SupportedCiphers.IndexOf(code) == -1)
			{
				throw new TlsException(AlertDescription.InsuficientSecurity, "Invalid cipher suite received from server");
			}
			cipherSuite = base.Context.SupportedCiphers[code];
			compressionMethod = (SecurityCompressionType)ReadByte();
		}

		private void processProtocol(short protocol)
		{
			SecurityProtocolType securityProtocolType = base.Context.DecodeProtocolCode(protocol);
			if ((securityProtocolType & base.Context.SecurityProtocolFlags) == securityProtocolType || (base.Context.SecurityProtocolFlags & SecurityProtocolType.Default) == SecurityProtocolType.Default)
			{
				base.Context.SecurityProtocol = securityProtocolType;
				base.Context.SupportedCiphers.Clear();
				base.Context.SupportedCiphers = null;
				base.Context.SupportedCiphers = CipherSuiteFactory.GetSupportedCiphers(securityProtocolType);
				return;
			}
			throw new TlsException(AlertDescription.ProtocolVersion, "Incorrect protocol version received from server");
		}
	}
}
