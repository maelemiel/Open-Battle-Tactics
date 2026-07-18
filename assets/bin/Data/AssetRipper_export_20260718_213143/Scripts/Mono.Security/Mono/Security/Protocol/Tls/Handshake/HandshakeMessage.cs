using System;

namespace Mono.Security.Protocol.Tls.Handshake
{
	internal abstract class HandshakeMessage : TlsStream
	{
		private Context context;

		private HandshakeType handshakeType;

		private ContentType contentType;

		private byte[] cache;

		public Context Context
		{
			get
			{
				return context;
			}
		}

		public HandshakeType HandshakeType
		{
			get
			{
				return handshakeType;
			}
		}

		public ContentType ContentType
		{
			get
			{
				return contentType;
			}
		}

		public HandshakeMessage(Context context, HandshakeType handshakeType)
			: this(context, handshakeType, ContentType.Handshake)
		{
		}

		public HandshakeMessage(Context context, HandshakeType handshakeType, ContentType contentType)
		{
			this.context = context;
			this.handshakeType = handshakeType;
			this.contentType = contentType;
		}

		public HandshakeMessage(Context context, HandshakeType handshakeType, byte[] data)
			: base(data)
		{
			this.context = context;
			this.handshakeType = handshakeType;
		}

		protected abstract void ProcessAsTls1();

		protected abstract void ProcessAsSsl3();

		public void Process()
		{
			switch (Context.SecurityProtocol)
			{
			case SecurityProtocolType.Default:
			case SecurityProtocolType.Tls:
				ProcessAsTls1();
				break;
			case SecurityProtocolType.Ssl3:
				ProcessAsSsl3();
				break;
			default:
				throw new NotSupportedException("Unsupported security protocol type");
			}
		}

		public virtual void Update()
		{
			if (CanWrite)
			{
				if (cache == null)
				{
					cache = EncodeMessage();
				}
				context.HandshakeMessages.Write(cache);
				Reset();
				cache = null;
			}
		}

		public virtual byte[] EncodeMessage()
		{
			cache = null;
			if (CanWrite)
			{
				byte[] array = ToArray();
				int num = array.Length;
				cache = new byte[4 + num];
				cache[0] = (byte)HandshakeType;
				cache[1] = (byte)(num >> 16);
				cache[2] = (byte)(num >> 8);
				cache[3] = (byte)num;
				Buffer.BlockCopy(array, 0, cache, 4, num);
			}
			return cache;
		}

		public static bool Compare(byte[] buffer1, byte[] buffer2)
		{
			if (buffer1 == null || buffer2 == null)
			{
				return false;
			}
			if (buffer1.Length != buffer2.Length)
			{
				return false;
			}
			for (int i = 0; i < buffer1.Length; i++)
			{
				if (buffer1[i] != buffer2[i])
				{
					return false;
				}
			}
			return true;
		}
	}
}
