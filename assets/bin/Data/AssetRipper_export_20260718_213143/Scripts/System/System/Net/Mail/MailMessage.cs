using System.Collections.Specialized;
using System.Net.Mime;
using System.Text;

namespace System.Net.Mail
{
	public class MailMessage : IDisposable
	{
		private AlternateViewCollection alternateViews;

		private AttachmentCollection attachments;

		private MailAddressCollection bcc;

		private MailAddressCollection replyTo;

		private string body;

		private MailPriority priority;

		private MailAddress sender;

		private DeliveryNotificationOptions deliveryNotificationOptions;

		private MailAddressCollection cc;

		private MailAddress from;

		private NameValueCollection headers;

		private MailAddressCollection to;

		private string subject;

		private Encoding subjectEncoding;

		private Encoding bodyEncoding;

		private Encoding headersEncoding = Encoding.UTF8;

		private bool isHtml;

		public AlternateViewCollection AlternateViews
		{
			get
			{
				return alternateViews;
			}
		}

		public AttachmentCollection Attachments
		{
			get
			{
				return attachments;
			}
		}

		public MailAddressCollection Bcc
		{
			get
			{
				return bcc;
			}
		}

		public string Body
		{
			get
			{
				return body;
			}
			set
			{
				if (value != null && bodyEncoding == null)
				{
					bodyEncoding = GuessEncoding(value) ?? Encoding.ASCII;
				}
				body = value;
			}
		}

		internal ContentType BodyContentType
		{
			get
			{
				ContentType contentType = new ContentType((!isHtml) ? "text/plain" : "text/html");
				contentType.CharSet = (BodyEncoding ?? Encoding.ASCII).HeaderName;
				return contentType;
			}
		}

		internal TransferEncoding ContentTransferEncoding
		{
			get
			{
				return ContentType.GuessTransferEncoding(BodyEncoding);
			}
		}

		public Encoding BodyEncoding
		{
			get
			{
				return bodyEncoding;
			}
			set
			{
				bodyEncoding = value;
			}
		}

		public MailAddressCollection CC
		{
			get
			{
				return cc;
			}
		}

		public DeliveryNotificationOptions DeliveryNotificationOptions
		{
			get
			{
				return deliveryNotificationOptions;
			}
			set
			{
				deliveryNotificationOptions = value;
			}
		}

		public MailAddress From
		{
			get
			{
				return from;
			}
			set
			{
				from = value;
			}
		}

		public NameValueCollection Headers
		{
			get
			{
				return headers;
			}
		}

		public bool IsBodyHtml
		{
			get
			{
				return isHtml;
			}
			set
			{
				isHtml = value;
			}
		}

		public MailPriority Priority
		{
			get
			{
				return priority;
			}
			set
			{
				priority = value;
			}
		}

		internal Encoding HeadersEncoding
		{
			get
			{
				return headersEncoding;
			}
			set
			{
				headersEncoding = value;
			}
		}

		internal MailAddressCollection ReplyToList
		{
			get
			{
				return replyTo;
			}
		}

		public MailAddress ReplyTo
		{
			get
			{
				if (replyTo.Count == 0)
				{
					return null;
				}
				return replyTo[0];
			}
			set
			{
				replyTo.Clear();
				replyTo.Add(value);
			}
		}

		public MailAddress Sender
		{
			get
			{
				return sender;
			}
			set
			{
				sender = value;
			}
		}

		public string Subject
		{
			get
			{
				return subject;
			}
			set
			{
				if (value != null && subjectEncoding == null)
				{
					subjectEncoding = GuessEncoding(value);
				}
				subject = value;
			}
		}

		public Encoding SubjectEncoding
		{
			get
			{
				return subjectEncoding;
			}
			set
			{
				subjectEncoding = value;
			}
		}

		public MailAddressCollection To
		{
			get
			{
				return to;
			}
		}

		public MailMessage()
		{
			to = new MailAddressCollection();
			alternateViews = new AlternateViewCollection();
			attachments = new AttachmentCollection();
			bcc = new MailAddressCollection();
			cc = new MailAddressCollection();
			replyTo = new MailAddressCollection();
			headers = new NameValueCollection();
			headers.Add("MIME-Version", "1.0");
		}

		public MailMessage(MailAddress from, MailAddress to)
			: this()
		{
			if (from == null || to == null)
			{
				throw new ArgumentNullException();
			}
			From = from;
			this.to.Add(to);
		}

		public MailMessage(string from, string to)
			: this()
		{
			if (from == null || from == string.Empty)
			{
				throw new ArgumentNullException("from");
			}
			if (to == null || to == string.Empty)
			{
				throw new ArgumentNullException("to");
			}
			this.from = new MailAddress(from);
			string[] array = to.Split(',');
			foreach (string text in array)
			{
				this.to.Add(new MailAddress(text.Trim()));
			}
		}

		public MailMessage(string from, string to, string subject, string body)
			: this()
		{
			if (from == null || from == string.Empty)
			{
				throw new ArgumentNullException("from");
			}
			if (to == null || to == string.Empty)
			{
				throw new ArgumentNullException("to");
			}
			this.from = new MailAddress(from);
			string[] array = to.Split(',');
			foreach (string text in array)
			{
				this.to.Add(new MailAddress(text.Trim()));
			}
			Body = body;
			Subject = subject;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		private Encoding GuessEncoding(string s)
		{
			return ContentType.GuessEncoding(s);
		}
	}
}
