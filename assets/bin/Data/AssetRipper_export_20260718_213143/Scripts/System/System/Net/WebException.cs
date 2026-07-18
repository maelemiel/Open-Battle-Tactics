using System.Runtime.Serialization;

namespace System.Net
{
	[Serializable]
	public class WebException : InvalidOperationException, ISerializable
	{
		private WebResponse response;

		private WebExceptionStatus status = WebExceptionStatus.UnknownError;

		public WebResponse Response
		{
			get
			{
				return response;
			}
		}

		public WebExceptionStatus Status
		{
			get
			{
				return status;
			}
		}

		public WebException()
		{
		}

		public WebException(string message)
			: base(message)
		{
		}

		protected WebException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public WebException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public WebException(string message, WebExceptionStatus status)
			: base(message)
		{
			this.status = status;
		}

		internal WebException(string message, Exception innerException, WebExceptionStatus status)
			: base(message, innerException)
		{
			this.status = status;
		}

		public WebException(string message, Exception innerException, WebExceptionStatus status, WebResponse response)
			: base(message, innerException)
		{
			this.status = status;
			this.response = response;
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		public override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			base.GetObjectData(serializationInfo, streamingContext);
		}
	}
}
