using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace System.Runtime.Remoting.Channels
{
	[Serializable]
	internal class CrossAppDomainChannel : IChannel, IChannelReceiver, IChannelSender
	{
		private const string _strName = "MONOCAD";

		private const string _strBaseURI = "MONOCADURI";

		private static object s_lock = new object();

		public virtual string ChannelName
		{
			get
			{
				return "MONOCAD";
			}
		}

		public virtual int ChannelPriority
		{
			get
			{
				return 100;
			}
		}

		public virtual object ChannelData
		{
			get
			{
				return new CrossAppDomainData(Thread.GetDomainID());
			}
		}

		internal static void RegisterCrossAppDomainChannel()
		{
			lock (s_lock)
			{
				CrossAppDomainChannel chnl = new CrossAppDomainChannel();
				ChannelServices.RegisterChannel(chnl);
			}
		}

		public string Parse(string url, out string objectURI)
		{
			objectURI = url;
			return null;
		}

		public virtual string[] GetUrlsForUri(string objectURI)
		{
			throw new NotSupportedException("CrossAppdomain channel dont support UrlsForUri");
		}

		public virtual void StartListening(object data)
		{
		}

		public virtual void StopListening(object data)
		{
		}

		public virtual IMessageSink CreateMessageSink(string url, object data, out string uri)
		{
			uri = null;
			if (data != null)
			{
				CrossAppDomainData crossAppDomainData = data as CrossAppDomainData;
				if (crossAppDomainData != null && crossAppDomainData.ProcessID == RemotingConfiguration.ProcessId)
				{
					return CrossAppDomainSink.GetSink(crossAppDomainData.DomainID);
				}
			}
			if (url != null && url.StartsWith("MONOCAD"))
			{
				throw new NotSupportedException("Can't create a named channel via crossappdomain");
			}
			return null;
		}
	}
}
