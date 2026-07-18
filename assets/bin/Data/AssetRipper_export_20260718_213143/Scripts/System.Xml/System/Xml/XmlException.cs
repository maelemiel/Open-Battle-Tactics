using System.Globalization;
using System.Runtime.Serialization;

namespace System.Xml
{
	[Serializable]
	public class XmlException : SystemException
	{
		private const string Xml_DefaultException = "Xml_DefaultException";

		private const string Xml_UserException = "Xml_UserException";

		private int lineNumber;

		private int linePosition;

		private string sourceUri;

		private string res;

		private string[] messages;

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
				if (lineNumber == 0)
				{
					return base.Message;
				}
				return string.Format(CultureInfo.InvariantCulture, "{0} {3} Line {1}, position {2}.", base.Message, lineNumber, linePosition, sourceUri);
			}
		}

		public XmlException()
		{
			res = "Xml_DefaultException";
			messages = new string[1];
		}

		public XmlException(string message, Exception innerException)
			: base(message, innerException)
		{
			res = "Xml_UserException";
			messages = new string[1] { message };
		}

		protected XmlException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			lineNumber = info.GetInt32("lineNumber");
			linePosition = info.GetInt32("linePosition");
			res = info.GetString("res");
			messages = (string[])info.GetValue("args", typeof(string[]));
			sourceUri = info.GetString("sourceUri");
		}

		public XmlException(string message)
			: base(message)
		{
			res = "Xml_UserException";
			messages = new string[1] { message };
		}

		internal XmlException(IXmlLineInfo li, string sourceUri, string message)
			: this(li, null, sourceUri, message)
		{
		}

		internal XmlException(IXmlLineInfo li, Exception innerException, string sourceUri, string message)
			: this(message, innerException)
		{
			if (li != null)
			{
				lineNumber = li.LineNumber;
				linePosition = li.LinePosition;
			}
			this.sourceUri = sourceUri;
		}

		public XmlException(string message, Exception innerException, int lineNumber, int linePosition)
			: this(message, innerException)
		{
			this.lineNumber = lineNumber;
			this.linePosition = linePosition;
		}

		internal XmlException(string message, int lineNumber, int linePosition, object sourceObject, string sourceUri, Exception innerException)
			: base(GetMessage(message, sourceUri, lineNumber, linePosition, sourceObject), innerException)
		{
			this.lineNumber = lineNumber;
			this.linePosition = linePosition;
			this.sourceUri = sourceUri;
		}

		private static string GetMessage(string message, string sourceUri, int lineNumber, int linePosition, object sourceObj)
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
			info.AddValue("lineNumber", lineNumber);
			info.AddValue("linePosition", linePosition);
			info.AddValue("res", res);
			info.AddValue("args", messages);
			info.AddValue("sourceUri", sourceUri);
		}
	}
}
