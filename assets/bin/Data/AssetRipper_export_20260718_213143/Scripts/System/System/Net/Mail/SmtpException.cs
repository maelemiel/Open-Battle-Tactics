using System.Runtime.Serialization;

namespace System.Net.Mail
{
	[Serializable]
	public class SmtpException : Exception, ISerializable
	{
		private SmtpStatusCode statusCode;

		public SmtpStatusCode StatusCode
		{
			get
			{
				return statusCode;
			}
			set
			{
				statusCode = value;
			}
		}

		public SmtpException()
			: this(SmtpStatusCode.GeneralFailure)
		{
		}

		public SmtpException(SmtpStatusCode statusCode)
			: this(statusCode, "Syntax error, command unrecognized.")
		{
		}

		public SmtpException(string message)
			: this(SmtpStatusCode.GeneralFailure, message)
		{
		}

		protected SmtpException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			try
			{
				statusCode = (SmtpStatusCode)(int)info.GetValue("Status", typeof(int));
			}
			catch (SerializationException)
			{
				statusCode = (SmtpStatusCode)(int)info.GetValue("statusCode", typeof(SmtpStatusCode));
			}
		}

		public SmtpException(SmtpStatusCode statusCode, string message)
			: base(message)
		{
			this.statusCode = statusCode;
		}

		public SmtpException(string message, Exception innerException)
			: base(message, innerException)
		{
			statusCode = SmtpStatusCode.GeneralFailure;
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
			info.AddValue("Status", statusCode, typeof(int));
		}
	}
}
