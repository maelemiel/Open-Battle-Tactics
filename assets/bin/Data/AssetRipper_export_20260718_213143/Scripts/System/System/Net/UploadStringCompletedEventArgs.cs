using System.ComponentModel;

namespace System.Net
{
	public class UploadStringCompletedEventArgs : AsyncCompletedEventArgs
	{
		private string result;

		public string Result
		{
			get
			{
				RaiseExceptionIfNecessary();
				return result;
			}
		}

		internal UploadStringCompletedEventArgs(string result, Exception error, bool cancelled, object userState)
			: base(error, cancelled, userState)
		{
			this.result = result;
		}
	}
}
