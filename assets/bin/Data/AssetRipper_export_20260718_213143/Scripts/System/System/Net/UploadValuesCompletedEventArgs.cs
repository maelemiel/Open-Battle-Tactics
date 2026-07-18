using System.ComponentModel;

namespace System.Net
{
	public class UploadValuesCompletedEventArgs : AsyncCompletedEventArgs
	{
		private byte[] result;

		public byte[] Result
		{
			get
			{
				return result;
			}
		}

		internal UploadValuesCompletedEventArgs(byte[] result, Exception error, bool cancelled, object userState)
			: base(error, cancelled, userState)
		{
			this.result = result;
		}
	}
}
