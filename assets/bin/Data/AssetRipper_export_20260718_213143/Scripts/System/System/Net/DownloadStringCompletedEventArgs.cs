using System.ComponentModel;

namespace System.Net
{
	public class DownloadStringCompletedEventArgs : AsyncCompletedEventArgs
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

		internal DownloadStringCompletedEventArgs(string result, Exception error, bool cancelled, object userState)
			: base(error, cancelled, userState)
		{
			this.result = result;
		}
	}
}
