using System.Runtime.Serialization;

namespace System.Net.Mail
{
	[Serializable]
	public class SmtpFailedRecipientsException : SmtpFailedRecipientException, ISerializable
	{
		private SmtpFailedRecipientException[] innerExceptions;

		public SmtpFailedRecipientException[] InnerExceptions
		{
			get
			{
				return innerExceptions;
			}
		}

		public SmtpFailedRecipientsException()
		{
		}

		public SmtpFailedRecipientsException(string message)
			: base(message)
		{
		}

		public SmtpFailedRecipientsException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public SmtpFailedRecipientsException(string message, SmtpFailedRecipientException[] innerExceptions)
			: base(message)
		{
			this.innerExceptions = innerExceptions;
		}

		protected SmtpFailedRecipientsException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			innerExceptions = (SmtpFailedRecipientException[])info.GetValue("innerExceptions", typeof(SmtpFailedRecipientException[]));
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			GetObjectData(info, context);
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			base.GetObjectData(info, context);
			info.AddValue("innerExceptions", innerExceptions);
		}
	}
}
