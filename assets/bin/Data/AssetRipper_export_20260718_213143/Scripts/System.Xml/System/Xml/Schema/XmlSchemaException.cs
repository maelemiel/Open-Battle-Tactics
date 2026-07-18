using System.Globalization;
using System.Runtime.Serialization;

namespace System.Xml.Schema
{
	[Serializable]
	public class XmlSchemaException : SystemException
	{
		private bool hasLineInfo;

		private int lineNumber;

		private int linePosition;

		private XmlSchemaObject sourceObj;

		private string sourceUri;

		public int LineNumber
		{
			get
			{
				return lineNumber;
			}
		}

		public int LinePosition
		{
			get
			{
				return linePosition;
			}
		}

		public XmlSchemaObject SourceSchemaObject
		{
			get
			{
				return sourceObj;
			}
		}

		public string SourceUri
		{
			get
			{
				return sourceUri;
			}
		}

		public override string Message
		{
			get
			{
				return base.Message;
			}
		}

		public XmlSchemaException()
			: this("A schema error occured.", null)
		{
		}

		public XmlSchemaException(string message)
			: this(message, null)
		{
		}

		protected XmlSchemaException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			hasLineInfo = info.GetBoolean("hasLineInfo");
			lineNumber = info.GetInt32("lineNumber");
			linePosition = info.GetInt32("linePosition");
			sourceUri = info.GetString("sourceUri");
			sourceObj = info.GetValue("sourceObj", typeof(XmlSchemaObject)) as XmlSchemaObject;
		}

		public XmlSchemaException(string message, Exception innerException, int lineNumber, int linePosition)
			: this(message, lineNumber, linePosition, null, null, innerException)
		{
		}

		internal XmlSchemaException(string message, int lineNumber, int linePosition, XmlSchemaObject sourceObject, string sourceUri, Exception innerException)
			: base(GetMessage(message, sourceUri, lineNumber, linePosition, sourceObject), innerException)
		{
			hasLineInfo = true;
			this.lineNumber = lineNumber;
			this.linePosition = linePosition;
			sourceObj = sourceObject;
			this.sourceUri = sourceUri;
		}

		internal XmlSchemaException(string message, object sender, string sourceUri, XmlSchemaObject sourceObject, Exception innerException)
			: base(GetMessage(message, sourceUri, sender, sourceObject), innerException)
		{
			IXmlLineInfo xmlLineInfo = sender as IXmlLineInfo;
			if (xmlLineInfo != null && xmlLineInfo.HasLineInfo())
			{
				hasLineInfo = true;
				lineNumber = xmlLineInfo.LineNumber;
				linePosition = xmlLineInfo.LinePosition;
			}
			sourceObj = sourceObject;
		}

		internal XmlSchemaException(string message, XmlSchemaObject sourceObject, Exception innerException)
			: base(GetMessage(message, null, 0, 0, sourceObject), innerException)
		{
			hasLineInfo = true;
		}

		public XmlSchemaException(string message, Exception innerException)
			: base(GetMessage(message, null, 0, 0, null), innerException)
		{
		}

		private static string GetMessage(string message, string sourceUri, object sender, XmlSchemaObject sourceObj)
		{
			IXmlLineInfo xmlLineInfo = sender as IXmlLineInfo;
			if (xmlLineInfo == null)
			{
				return GetMessage(message, sourceUri, 0, 0, sourceObj);
			}
			return GetMessage(message, sourceUri, xmlLineInfo.LineNumber, xmlLineInfo.LinePosition, sourceObj);
		}

		private static string GetMessage(string message, string sourceUri, int lineNumber, int linePosition, XmlSchemaObject sourceObj)
		{
			string text = "XmlSchema error: " + message;
			if (lineNumber > 0)
			{
				text += string.Format(CultureInfo.InvariantCulture, " XML {0} Line {1}, Position {2}.", (sourceUri == null || !(sourceUri != string.Empty)) ? string.Empty : ("URI: " + sourceUri + " ."), lineNumber, linePosition);
			}
			return text;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("hasLineInfo", hasLineInfo);
			info.AddValue("lineNumber", lineNumber);
			info.AddValue("linePosition", linePosition);
			info.AddValue("sourceUri", sourceUri);
			info.AddValue("sourceObj", sourceObj);
		}
	}
}
