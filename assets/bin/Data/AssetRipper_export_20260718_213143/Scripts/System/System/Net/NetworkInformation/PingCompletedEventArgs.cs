using System.ComponentModel;

namespace System.Net.NetworkInformation
{
	public class PingCompletedEventArgs : AsyncCompletedEventArgs
	{
		private PingReply reply;

		public PingReply Reply
		{
			get
			{
				return reply;
			}
		}

		internal PingCompletedEventArgs(Exception ex, bool cancelled, object userState, PingReply reply)
			: base(ex, cancelled, userState)
		{
			this.reply = reply;
		}
	}
}
