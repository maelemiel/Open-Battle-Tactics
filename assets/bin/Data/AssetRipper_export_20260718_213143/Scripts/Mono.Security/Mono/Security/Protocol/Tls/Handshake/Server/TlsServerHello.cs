namespace Mono.Security.Protocol.Tls.Handshake.Server
{
	internal class TlsServerHello : HandshakeMessage
	{
		private int unixTime;

		private byte[] random;

		public TlsServerHello(Context context)
			: base(context, HandshakeType.ServerHello)
		{
		}

		public override void Update()
		{
			base.Update();
			TlsStream tlsStream = new TlsStream();
			tlsStream.Write(unixTime);
			tlsStream.Write(random);
			base.Context.ServerRandom = tlsStream.ToArray();
			tlsStream.Reset();
			tlsStream.Write(base.Context.ClientRandom);
			tlsStream.Write(base.Context.ServerRandom);
			base.Context.RandomCS = tlsStream.ToArray();
			tlsStream.Reset();
			tlsStream.Write(base.Context.ServerRandom);
			tlsStream.Write(base.Context.ClientRandom);
			base.Context.RandomSC = tlsStream.ToArray();
			tlsStream.Reset();
		}

		protected override void ProcessAsSsl3()
		{
			ProcessAsTls1();
		}

		protected override void ProcessAsTls1()
		{
			Write(base.Context.Protocol);
			unixTime = base.Context.GetUnixTime();
			Write(unixTime);
			random = base.Context.GetSecureRandomBytes(28);
			Write(random);
			if (base.Context.SessionId == null)
			{
				WriteByte(0);
			}
			else
			{
				WriteByte((byte)base.Context.SessionId.Length);
				Write(base.Context.SessionId);
			}
			Write(base.Context.Negotiating.Cipher.Code);
			WriteByte((byte)base.Context.CompressionMethod);
		}
	}
}
