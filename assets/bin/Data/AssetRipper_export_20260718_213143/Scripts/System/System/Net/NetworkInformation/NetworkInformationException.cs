namespace System.Net.NetworkInformation
{
	[Serializable]
	public class NetworkInformationException : Exception
	{
		private int error_code;

		public int ErrorCode
		{
			get
			{
				return error_code;
			}
		}

		public NetworkInformationException()
		{
		}

		public NetworkInformationException(int errorCode)
		{
			error_code = errorCode;
		}
	}
}
