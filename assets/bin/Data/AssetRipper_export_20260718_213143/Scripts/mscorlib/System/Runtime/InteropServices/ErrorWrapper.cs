namespace System.Runtime.InteropServices
{
	[Serializable]
	[ComVisible(true)]
	public sealed class ErrorWrapper
	{
		private int errorCode;

		public int ErrorCode
		{
			get
			{
				return errorCode;
			}
		}

		public ErrorWrapper(Exception e)
		{
			errorCode = Marshal.GetHRForException(e);
		}

		public ErrorWrapper(int errorCode)
		{
			this.errorCode = errorCode;
		}

		public ErrorWrapper(object errorCode)
		{
			if (errorCode.GetType() != typeof(int))
			{
				throw new ArgumentException("errorCode has to be an int type");
			}
			this.errorCode = (int)errorCode;
		}
	}
}
