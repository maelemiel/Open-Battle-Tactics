using System.Runtime.Serialization;

namespace System.Net.Mail
{
	[Serializable]
	public class SmtpFailedRecipientException : SmtpException, ISerializable
	{
		private string failedRecipient;

		public string FailedRecipient
		{
			get
			{
				return failedRecipient;
			}
		}

		public SmtpFailedRecipientException()
		{
		}

		public SmtpFailedRecipientException(string message)
			: base(message)
		{
		}

		protected SmtpFailedRecipientException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
			if (serializationInfo == null)
			{
				throw new ArgumentNullException("serializationInfo");
			}
			failedRecipient = serializationInfo.GetString("failedRecipient");
		}

		public SmtpFailedRecipientException(SmtpStatusCode statusCode, string failedRecipient)
			: base(statusCode)
		{
			this.failedRecipient = failedRecipient;
		}

		public SmtpFailedRecipientException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public SmtpFailedRecipientException(string message, string failedRecipient, Exception innerException)
			: base(message, innerException)
		{
			this.failedRecipient = failedRecipient;
		}

		public SmtpFailedRecipientException(SmtpStatusCode statusCode, string failedRecipient, string serverResponse)
			: base(statusCode, serverResponse)
		{
			this.failedRecipient = failedRecipient;
		}

		void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			GetObjectData(serializationInfo, streamingContext);
		}

		public override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			if (serializationInfo == null)
			{
				throw new ArgumentNullException("serializationInfo");
			}
			base.GetObjectData(serializationInfo, streamingContext);
			serializationInfo.AddValue("failedRecipient", failedRecipient);
		}
	}
}
