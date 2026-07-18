using System.Runtime.Serialization;

namespace System.Xml.Schema
{
	[Serializable]
	public class XmlSchemaValidationException : XmlSchemaException
	{
		private object source_object;

		public object SourceObject
		{
			get
			{
				return source_object;
			}
		}

		public XmlSchemaValidationException()
		{
		}

		public XmlSchemaValidationException(string message)
			: base(message)
		{
		}

		protected XmlSchemaValidationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public XmlSchemaValidationException(string message, Exception innerException, int lineNumber, int linePosition)
			: base(message, lineNumber, linePosition, null, null, innerException)
		{
		}

		internal XmlSchemaValidationException(string message, int lineNumber, int linePosition, XmlSchemaObject sourceObject, string sourceUri, Exception innerException)
			: base(message, lineNumber, linePosition, sourceObject, sourceUri, innerException)
		{
		}

		internal XmlSchemaValidationException(string message, object sender, string sourceUri, XmlSchemaObject sourceObject, Exception innerException)
			: base(message, sender, sourceUri, sourceObject, innerException)
		{
		}

		internal XmlSchemaValidationException(string message, XmlSchemaObject sourceObject, Exception innerException)
			: base(message, sourceObject, innerException)
		{
		}

		public XmlSchemaValidationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		protected internal void SetSourceObject(object o)
		{
			source_object = o;
		}
	}
}
