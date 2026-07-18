using System.Runtime.InteropServices;
using System.Threading;

namespace System
{
	[ComVisible(true)]
	public interface IAsyncResult
	{
		object AsyncState { get; }

		WaitHandle AsyncWaitHandle { get; }

		bool CompletedSynchronously { get; }

		bool IsCompleted { get; }
	}
}
