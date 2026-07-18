using System.ComponentModel;

namespace System.Net
{
	public class DownloadProgressChangedEventArgs : ProgressChangedEventArgs
	{
		private long received;

		private long total;

		public long BytesReceived
		{
			get
			{
				return received;
			}
		}

		public long TotalBytesToReceive
		{
			get
			{
				return total;
			}
		}

		internal DownloadProgressChangedEventArgs(long bytesReceived, long totalBytesToReceive, object userState)
			: base((int)((totalBytesToReceive != -1) ? (bytesReceived * 100 / totalBytesToReceive) : 0), userState)
		{
			received = bytesReceived;
			total = totalBytesToReceive;
		}
	}
}
