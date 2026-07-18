using System.Threading;

namespace System.IO
{
	internal class StreamAsyncResult : IAsyncResult
	{
		private object state;

		private bool completed;

		private bool done;

		private Exception exc;

		private int nbytes = -1;

		private ManualResetEvent wh;

		public object AsyncState
		{
			get
			{
				return state;
			}
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				lock (this)
				{
					if (wh == null)
					{
						wh = new ManualResetEvent(completed);
					}
					return wh;
				}
			}
		}

		public virtual bool CompletedSynchronously
		{
			get
			{
				return true;
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

		public int NBytes
		{
			get
			{
				return nbytes;
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

		public StreamAsyncResult(object state)
		{
			this.state = state;
		}

		public void SetComplete(Exception e)
		{
			exc = e;
			completed = true;
			lock (this)
			{
				if (wh != null)
				{
					wh.Set();
				}
			}
		}

		public void SetComplete(Exception e, int nbytes)
		{
			this.nbytes = nbytes;
			SetComplete(e);
		}
	}
}
