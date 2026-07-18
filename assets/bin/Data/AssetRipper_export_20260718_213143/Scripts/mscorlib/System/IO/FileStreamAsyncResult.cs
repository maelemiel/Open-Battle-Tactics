using System.Threading;

namespace System.IO
{
	internal class FileStreamAsyncResult : IAsyncResult
	{
		private object state;

		private bool completed;

		private bool done;

		private Exception exc;

		private ManualResetEvent wh;

		private AsyncCallback cb;

		private bool completedSynch;

		public byte[] Buffer;

		public int Offset;

		public int Count;

		public int OriginalCount;

		public int BytesRead;

		private AsyncCallback realcb;

		public object AsyncState
		{
			get
			{
				return state;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				return completedSynch;
			}
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				return wh;
			}
		}

		public bool IsCompleted
		{
			get
			{
				return completed;
			}
		}

		public Exception Exception
		{
			get
			{
				return exc;
			}
		}

		public bool Done
		{
			get
			{
				return done;
			}
			set
			{
				done = value;
			}
		}

		public FileStreamAsyncResult(AsyncCallback cb, object state)
		{
			this.state = state;
			realcb = cb;
			if (realcb != null)
			{
				this.cb = CBWrapper;
			}
			wh = new ManualResetEvent(false);
		}

		private static void CBWrapper(IAsyncResult ares)
		{
			FileStreamAsyncResult fileStreamAsyncResult = (FileStreamAsyncResult)ares;
			fileStreamAsyncResult.realcb.BeginInvoke(ares, null, null);
		}

		public void SetComplete(Exception e)
		{
			exc = e;
			completed = true;
			wh.Set();
			if (cb != null)
			{
				cb(this);
			}
		}

		public void SetComplete(Exception e, int nbytes)
		{
			BytesRead = nbytes;
			SetComplete(e);
		}

		public void SetComplete(Exception e, int nbytes, bool synch)
		{
			completedSynch = synch;
			SetComplete(e, nbytes);
		}
	}
}
