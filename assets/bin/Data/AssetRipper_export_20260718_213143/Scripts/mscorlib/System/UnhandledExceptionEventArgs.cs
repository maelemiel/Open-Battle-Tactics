using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public class UnhandledExceptionEventArgs : EventArgs
	{
		private object exception;

		private bool m_isTerminating;

		public object ExceptionObject
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				return exception;
			}
		}

		public bool IsTerminating
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				return m_isTerminating;
			}
		}

		public UnhandledExceptionEventArgs(object exception, bool isTerminating)
		{
			this.exception = exception;
			m_isTerminating = isTerminating;
		}
	}
}
