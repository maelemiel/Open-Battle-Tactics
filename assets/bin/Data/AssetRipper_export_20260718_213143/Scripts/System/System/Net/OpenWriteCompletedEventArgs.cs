using System.ComponentModel;
using System.IO;

namespace System.Net
{
	public class OpenWriteCompletedEventArgs : AsyncCompletedEventArgs
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

		internal OpenWriteCompletedEventArgs(Stream result, Exception error, bool cancelled, object userState)
			: base(error, cancelled, userState)
		{
			this.result = result;
		}
	}
}
