namespace System.Xml.Schema
{
	internal class ValidationHandler
	{
		public static void RaiseValidationEvent(ValidationEventHandler handle, Exception innerException, string message, XmlSchemaObject xsobj, object sender, string sourceUri, XmlSeverityType severity)
		{
			XmlSchemaException ex = new XmlSchemaException(message, sender, sourceUri, xsobj, innerException);
			ValidationEventArgs e = new ValidationEventArgs(ex, message, severity);
			if (handle == null)
			{
				if (e.Severity == XmlSeverityType.Error)
				{
					throw e.Exception;
				}
			}
			else
			{
				handle(sender, e);
			}
		}
	}
}
