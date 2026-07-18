using System.Threading;

namespace System.ComponentModel
{
	public sealed class AsyncOperation
	{
		private SynchronizationContext ctx;

		private object state;

		private bool done;

		public SynchronizationContext SynchronizationContext
		{
			get
			{
				return ctx;
			}
		}

		public object UserSuppliedState
		{
			get
			{
				return state;
			}
		}

		internal AsyncOperation(SynchronizationContext ctx, object state)
		{
			this.ctx = ctx;
			this.state = state;
			ctx.OperationStarted();
		}

		~AsyncOperation()
		{
			if (!done && ctx != null)
			{
				ctx.OperationCompleted();
			}
		}

		public void OperationCompleted()
		{
			if (done)
			{
				throw new InvalidOperationException("This task is already completed. Multiple call to OperationCompleted is not allowed.");
			}
			ctx.OperationCompleted();
			done = true;
		}

		public void Post(SendOrPostCallback d, object arg)
		{
			if (done)
			{
				throw new InvalidOperationException("This task is already completed. Multiple call to Post is not allowed.");
			}
			ctx.Post(d, arg);
		}

		public void PostOperationCompleted(SendOrPostCallback d, object arg)
		{
			if (done)
			{
				throw new InvalidOperationException("This task is already completed. Multiple call to PostOperationCompleted is not allowed.");
			}
			Post(d, arg);
			OperationCompleted();
		}
	}
}
