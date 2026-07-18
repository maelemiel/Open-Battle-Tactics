using System.ComponentModel;

namespace System.Net
{
	public class UploadProgressChangedEventArgs : ProgressChangedEventArgs
	{
		private long received;

		private long sent;

		private long total_recv;

		private long total_send;

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
				return total_recv;
			}
		}

		public long BytesSent
		{
			get
			{
				return sent;
			}
		}

		public long TotalBytesToSend
		{
			get
			{
				return total_send;
			}
		}

		internal UploadProgressChangedEventArgs(long bytesReceived, long totalBytesToReceive, long bytesSent, long totalBytesToSend, int progressPercentage, object userState)
			: base(progressPercentage, userState)
		{
			received = bytesReceived;
			total_recv = totalBytesToReceive;
			sent = bytesSent;
			total_send = totalBytesToSend;
		}
	}
}
