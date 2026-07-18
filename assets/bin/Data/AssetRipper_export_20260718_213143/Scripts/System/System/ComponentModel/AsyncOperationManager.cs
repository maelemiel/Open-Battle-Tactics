using System.Threading;

namespace System.ComponentModel
{
	public static class AsyncOperationManager
	{
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static SynchronizationContext SynchronizationContext
		{
			get
			{
				if (SynchronizationContext.Current == null)
				{
					SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
				}
				return SynchronizationContext.Current;
			}
			set
			{
				SynchronizationContext.SetSynchronizationContext(value);
			}
		}

		static AsyncOperationManager()
		{
		}

		public static AsyncOperation CreateOperation(object userSuppliedState)
		{
			return new AsyncOperation(SynchronizationContext, userSuppliedState);
		}
	}
}
