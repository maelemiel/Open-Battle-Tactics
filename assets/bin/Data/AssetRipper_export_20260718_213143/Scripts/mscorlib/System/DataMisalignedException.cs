using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public sealed class DataMisalignedException : SystemException
	{
		private const int Result = -2146233023;

		public DataMisalignedException()
			: base(Locale.GetText("A datatype misalignment was detected in a load or store instruction."))
		{
			base.HResult = -2146233023;
		}

		public DataMisalignedException(string message)
			: base(message)
		{
			base.HResult = -2146233023;
		}

		public DataMisalignedException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2146233023;
		}
	}
}
