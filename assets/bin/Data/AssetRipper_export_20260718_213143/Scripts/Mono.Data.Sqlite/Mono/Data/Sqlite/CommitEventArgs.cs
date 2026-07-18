using System;

namespace Mono.Data.Sqlite
{
	public class CommitEventArgs : EventArgs
	{
		public bool AbortTransaction;

		internal CommitEventArgs()
		{
		}
	}
}
