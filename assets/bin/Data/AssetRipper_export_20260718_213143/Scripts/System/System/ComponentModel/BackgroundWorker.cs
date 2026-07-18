using System.Threading;

namespace System.ComponentModel
{
	public class BackgroundWorker
	{
		private delegate void ProcessWorkerEventHandler(object argument, AsyncOperation async, SendOrPostCallback callback);

		private AsyncOperation async;

		private bool cancel_pending;

		private bool report_progress;

		private bool support_cancel;

		public bool CancellationPending
		{
			get
			{
				return cancel_pending;
			}
		}

		public bool IsBusy
		{
			get
			{
				return async != null;
			}
		}

		[DefaultValue(false)]
		public bool WorkerReportsProgress
		{
			get
			{
				return report_progress;
			}
			set
			{
				report_progress = value;
			}
		}

		[DefaultValue(false)]
		public bool WorkerSupportsCancellation
		{
			get
			{
				return support_cancel;
			}
			set
			{
				support_cancel = value;
			}
		}

		public event DoWorkEventHandler DoWork;

		public event ProgressChangedEventHandler ProgressChanged;

		public event RunWorkerCompletedEventHandler RunWorkerCompleted;

		public void CancelAsync()
		{
			if (!support_cancel)
			{
				throw new InvalidOperationException("This background worker does not support cancellation.");
			}
			if (IsBusy)
			{
				cancel_pending = true;
			}
		}

		public void ReportProgress(int percentProgress)
		{
			ReportProgress(percentProgress, null);
		}

		public void ReportProgress(int percentProgress, object userState)
		{
			if (!WorkerReportsProgress)
			{
				throw new InvalidOperationException("This background worker does not report progress.");
			}
			if (IsBusy)
			{
				async.Post(delegate(object o)
				{
					ProgressChangedEventArgs e = o as ProgressChangedEventArgs;
					OnProgressChanged(e);
				}, new ProgressChangedEventArgs(percentProgress, userState));
			}
		}

		public void RunWorkerAsync()
		{
			RunWorkerAsync(null);
		}

		private void ProcessWorker(object argument, AsyncOperation async, SendOrPostCallback callback)
		{
			Exception error = null;
			DoWorkEventArgs e = new DoWorkEventArgs(argument);
			try
			{
				OnDoWork(e);
			}
			catch (Exception ex)
			{
				error = ex;
				e.Cancel = false;
			}
			callback(new object[2]
			{
				new RunWorkerCompletedEventArgs(e.Result, error, e.Cancel),
				async
			});
		}

		private void CompleteWorker(object state)
		{
			object[] array = (object[])state;
			RunWorkerCompletedEventArgs arg = array[0] as RunWorkerCompletedEventArgs;
			AsyncOperation asyncOperation = array[1] as AsyncOperation;
			SendOrPostCallback d = delegate(object darg)
			{
				async = null;
				OnRunWorkerCompleted(darg as RunWorkerCompletedEventArgs);
			};
			asyncOperation.PostOperationCompleted(d, arg);
			cancel_pending = false;
		}

		public void RunWorkerAsync(object argument)
		{
			if (IsBusy)
			{
				throw new InvalidOperationException("The background worker is busy.");
			}
			async = AsyncOperationManager.CreateOperation(this);
			ProcessWorkerEventHandler processWorkerEventHandler = ProcessWorker;
			processWorkerEventHandler.BeginInvoke(argument, async, CompleteWorker, null, null);
		}

		protected virtual void OnDoWork(DoWorkEventArgs e)
		{
			if (this.DoWork != null)
			{
				this.DoWork(this, e);
			}
		}

		protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
		{
			if (this.ProgressChanged != null)
			{
				this.ProgressChanged(this, e);
			}
		}

		protected virtual void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
		{
			if (this.RunWorkerCompleted != null)
			{
				this.RunWorkerCompleted(this, e);
			}
		}
	}
}
