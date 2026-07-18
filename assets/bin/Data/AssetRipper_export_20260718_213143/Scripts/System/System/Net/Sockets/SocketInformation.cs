namespace System.Net.Sockets
{
	[Serializable]
	public struct SocketInformation
	{
		private SocketInformationOptions options;

		private byte[] protocol_info;

		public SocketInformationOptions Options
		{
			get
			{
				return options;
			}
			set
			{
				options = value;
			}
		}

		public byte[] ProtocolInformation
		{
			get
			{
				return protocol_info;
			}
			set
			{
				protocol_info = value;
			}
		}
	}
}
