namespace Mono.Security.Protocol.Tls.Handshake.Server
{
	internal class TlsClientHello : HandshakeMessage
	{
		private byte[] random;

		private byte[] sessionId;

		private short[] cipherSuites;

		private byte[] compressionMethods;

		public TlsClientHello(Context context, byte[] buffer)
			: base(context, HandshakeType.ClientHello, buffer)
		{
		}

		public override void Update()
		{
			base.Update();
			selectCipherSuite();
			selectCompressionMethod();
			base.Context.SessionId = sessionId;
			base.Context.ClientRandom = random;
			base.Context.ProtocolNegotiated = true;
		}

		protected override void ProcessAsSsl3()
		{
			ProcessAsTls1();
		}

		protected override void ProcessAsTls1()
		{
			processProtocol(ReadInt16());
			random = ReadBytes(32);
			sessionId = ReadBytes(ReadByte());
			cipherSuites = new short[ReadInt16() / 2];
			for (int i = 0; i < cipherSuites.Length; i++)
			{
				cipherSuites[i] = ReadInt16();
			}
			compressionMethods = new byte[ReadByte()];
			for (int j = 0; j < compressionMethods.Length; j++)
			{
				compressionMethods[j] = ReadByte();
			}
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

		private void selectCipherSuite()
		{
			int num = 0;
			for (int i = 0; i < cipherSuites.Length; i++)
			{
				if ((num = base.Context.SupportedCiphers.IndexOf(cipherSuites[i])) != -1)
				{
					base.Context.Negotiating.Cipher = base.Context.SupportedCiphers[num];
					break;
				}
			}
			if (base.Context.Negotiating.Cipher == null)
			{
				throw new TlsException(AlertDescription.InsuficientSecurity, "Insuficient Security");
			}
		}

		private void selectCompressionMethod()
		{
			base.Context.CompressionMethod = SecurityCompressionType.None;
		}
	}
}
