namespace Mono.Security.Protocol.Tls.Handshake.Client
{
	internal class TlsClientHello : HandshakeMessage
	{
		private byte[] random;

		public TlsClientHello(Context context)
			: base(context, HandshakeType.ClientHello)
		{
		}

		public override void Update()
		{
			ClientContext clientContext = (ClientContext)base.Context;
			base.Update();
			clientContext.ClientRandom = random;
			clientContext.ClientHelloProtocol = base.Context.Protocol;
			random = null;
		}

		protected override void ProcessAsSsl3()
		{
			ProcessAsTls1();
		}

		protected override void ProcessAsTls1()
		{
			Write(base.Context.Protocol);
			TlsStream tlsStream = new TlsStream();
			tlsStream.Write(base.Context.GetUnixTime());
			tlsStream.Write(base.Context.GetSecureRandomBytes(28));
			random = tlsStream.ToArray();
			tlsStream.Reset();
			Write(random);
			base.Context.SessionId = ClientSessionCache.FromHost(base.Context.ClientSettings.TargetHost);
			if (base.Context.SessionId != null)
			{
				Write((byte)base.Context.SessionId.Length);
				if (base.Context.SessionId.Length > 0)
				{
					Write(base.Context.SessionId);
				}
			}
			else
			{
				Write((byte)0);
			}
			Write((short)(base.Context.SupportedCiphers.Count * 2));
			for (int i = 0; i < base.Context.SupportedCiphers.Count; i++)
			{
				Write(base.Context.SupportedCiphers[i].Code);
			}
			Write((byte)1);
			Write((byte)base.Context.CompressionMethod);
		}
	}
}
