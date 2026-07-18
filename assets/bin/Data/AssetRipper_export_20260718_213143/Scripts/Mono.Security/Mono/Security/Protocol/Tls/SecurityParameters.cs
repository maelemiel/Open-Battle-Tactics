namespace Mono.Security.Protocol.Tls
{
	internal class SecurityParameters
	{
		private CipherSuite cipher;

		private byte[] clientWriteMAC;

		private byte[] serverWriteMAC;

		public CipherSuite Cipher
		{
			get
			{
				return cipher;
			}
			set
			{
				cipher = value;
			}
		}

		public byte[] ClientWriteMAC
		{
			get
			{
				return clientWriteMAC;
			}
			set
			{
				clientWriteMAC = value;
			}
		}

		public byte[] ServerWriteMAC
		{
			get
			{
				return serverWriteMAC;
			}
			set
			{
				serverWriteMAC = value;
			}
		}

		public void Clear()
		{
			cipher = null;
		}
	}
}
