using System.Runtime.Remoting.Channels;

namespace System.Runtime.Remoting
{
	[Serializable]
	internal class ChannelInfo : IChannelInfo
	{
		private object[] channelData;

		public object[] ChannelData
		{
			get
			{
				return channelData;
			}
			set
			{
				channelData = value;
			}
		}

		public ChannelInfo()
		{
			channelData = ChannelServices.GetCurrentChannelInfo();
		}

		public ChannelInfo(object remoteChannelData)
		{
			channelData = new object[1] { remoteChannelData };
		}
	}
}
