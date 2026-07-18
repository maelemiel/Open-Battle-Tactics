using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Threading
{
	[Serializable]
	[ComVisible(false)]
	public class AbandonedMutexException : SystemException
	{
		private Mutex mutex;

		private int mutex_index = -1;

		public Mutex Mutex
		{
			get
			{
				return mutex;
			}
		}

		public int MutexIndex
		{
			get
			{
				return mutex_index;
			}
		}

		public AbandonedMutexException()
			: base("Mutex was abandoned")
		{
		}

		public AbandonedMutexException(string message)
			: base(message)
		{
		}

		public AbandonedMutexException(int location, WaitHandle handle)
			: base("Mutex was abandoned")
		{
			mutex_index = location;
			mutex = handle as Mutex;
		}

		protected AbandonedMutexException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public AbandonedMutexException(string message, Exception inner)
			: base(message, inner)
		{
		}

		public AbandonedMutexException(string message, int location, WaitHandle handle)
			: base(message)
		{
			mutex_index = location;
			mutex = handle as Mutex;
		}

		public AbandonedMutexException(string message, Exception inner, int location, WaitHandle handle)
			: base(message, inner)
		{
			mutex_index = location;
			mutex = handle as Mutex;
		}
	}
}
