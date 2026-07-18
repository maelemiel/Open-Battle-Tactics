using System.Runtime.Serialization;

namespace System.Xml.Schema
{
	[Serializable]
	public class XmlSchemaInferenceException : XmlSchemaException
	{
		public XmlSchemaInferenceException()
		{
		}

		public XmlSchemaInferenceException(string message)
			: base(message)
		{
		}

		protected XmlSchemaInferenceException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public XmlSchemaInferenceException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public XmlSchemaInferenceException(string message, Exception innerException, int line, int column)
			: base(message, innerException, line, column)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
