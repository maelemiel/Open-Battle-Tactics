namespace System.Xml.Schema
{
	public class ValidationEventArgs : EventArgs
	{
		private XmlSchemaException exception;

		private string message;

		private XmlSeverityType severity;

		public XmlSchemaException Exception
		{
			get
			{
				return exception;
			}
		}

		public string Message
		{
			get
			{
				return message;
			}
		}

		public XmlSeverityType Severity
		{
			get
			{
				return severity;
			}
		}

		private ValidationEventArgs()
		{
		}

		internal ValidationEventArgs(XmlSchemaException ex, string message, XmlSeverityType severity)
		{
			exception = ex;
			this.message = message;
			this.severity = severity;
		}
	}
}
