using System.IO;

namespace System.Net.Security
{
	public abstract class AuthenticatedStream : Stream
	{
		private Stream innerStream;

		private bool leaveStreamOpen;

		protected Stream InnerStream
		{
			get
			{
				return innerStream;
			}
		}

		public abstract bool IsAuthenticated { get; }

		public abstract bool IsEncrypted { get; }

		public abstract bool IsMutuallyAuthenticated { get; }

		public abstract bool IsServer { get; }

		public abstract bool IsSigned { get; }

		public bool LeaveInnerStreamOpen
		{
			get
			{
				return leaveStreamOpen;
			}
		}

		protected AuthenticatedStream(Stream innerStream, bool leaveInnerStreamOpen)
		{
			this.innerStream = innerStream;
			leaveStreamOpen = leaveInnerStreamOpen;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && innerStream != null)
			{
				if (!leaveStreamOpen)
				{
					innerStream.Close();
				}
				innerStream = null;
			}
		}
	}
}
