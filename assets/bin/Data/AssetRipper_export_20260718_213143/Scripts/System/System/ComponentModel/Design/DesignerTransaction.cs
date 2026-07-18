namespace System.ComponentModel.Design
{
	public abstract class DesignerTransaction : IDisposable
	{
		private string description;

		private bool committed;

		private bool canceled;

		public bool Canceled
		{
			get
			{
				return canceled;
			}
		}

		public bool Committed
		{
			get
			{
				return committed;
			}
		}

		public string Description
		{
			get
			{
				return description;
			}
		}

		protected DesignerTransaction()
			: this(string.Empty)
		{
		}

		protected DesignerTransaction(string description)
		{
			this.description = description;
			committed = false;
			canceled = false;
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			Cancel();
			if (disposing)
			{
				GC.SuppressFinalize(true);
			}
		}

		protected abstract void OnCancel();

		protected abstract void OnCommit();

		public void Cancel()
		{
			if (!Canceled && !Committed)
			{
				canceled = true;
				OnCancel();
			}
		}

		public void Commit()
		{
			if (!Canceled && !Committed)
			{
				committed = true;
				OnCommit();
			}
		}

		~DesignerTransaction()
		{
			Dispose(false);
		}
	}
}
