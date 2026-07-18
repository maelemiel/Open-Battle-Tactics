using System.ComponentModel;
using System.IO;

namespace System.Net
{
	public class OpenReadCompletedEventArgs : AsyncCompletedEventArgs
	{
		private Stream result;

		public Stream Result
		{
			get
			{
				RaiseExceptionIfNecessary();
				return result;
			}
		}

		internal OpenReadCompletedEventArgs(Stream result, Exception error, bool cancelled, object userState)
			: base(error, cancelled, userState)
		{
			this.result = result;
		}
	}
}
