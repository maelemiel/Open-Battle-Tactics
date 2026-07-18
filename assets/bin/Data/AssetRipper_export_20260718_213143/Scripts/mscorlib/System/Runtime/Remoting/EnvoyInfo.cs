using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Remoting
{
	[Serializable]
	internal class EnvoyInfo : IEnvoyInfo
	{
		private IMessageSink envoySinks;

		public IMessageSink EnvoySinks
		{
			get
			{
				return envoySinks;
			}
			set
			{
				envoySinks = value;
			}
		}

		public EnvoyInfo(IMessageSink sinks)
		{
			envoySinks = sinks;
		}
	}
}
