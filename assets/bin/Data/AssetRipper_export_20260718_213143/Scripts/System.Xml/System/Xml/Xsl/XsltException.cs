using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.XPath;

namespace System.Xml.Xsl
{
	[Serializable]
	public class XsltException : SystemException
	{
		private int lineNumber;

		private int linePosition;

		private string sourceUri;

		private string templateFrames;

		public virtual int LineNumber
		{
			get
			{
				return lineNumber;
			}
		}

		public virtual int LinePosition
		{
			get
			{
				return linePosition;
			}
		}

		public override string Message
		{
			get
			{
				return (templateFrames == null) ? base.Message : (base.Message + templateFrames);
			}
		}

		public virtual string SourceUri
		{
			get
			{
				return sourceUri;
			}
		}

		public XsltException()
			: this(string.Empty, null)
		{
		}

		public XsltException(string message)
			: this(message, null)
		{
		}

		public XsltException(string message, Exception innerException)
			: this("{0}", message, innerException, 0, 0, null)
		{
		}

		protected XsltException(SerializationInfo info, StreamingContext context)
		{
			lineNumber = info.GetInt32("lineNumber");
			linePosition = info.GetInt32("linePosition");
			sourceUri = info.GetString("sourceUri");
			templateFrames = info.GetString("templateFrames");
		}

		internal XsltException(string msgFormat, string message, Exception innerException, int lineNumber, int linePosition, string sourceUri)
			: base(CreateMessage(msgFormat, message, lineNumber, linePosition, sourceUri), innerException)
		{
			this.lineNumber = lineNumber;
			this.linePosition = linePosition;
			this.sourceUri = sourceUri;
		}

		internal XsltException(string message, Exception innerException, XPathNavigator nav)
			: base(CreateMessage(message, nav), innerException)
		{
			IXmlLineInfo xmlLineInfo = nav as IXmlLineInfo;
			lineNumber = ((xmlLineInfo != null) ? xmlLineInfo.LineNumber : 0);
			linePosition = ((xmlLineInfo != null) ? xmlLineInfo.LinePosition : 0);
			sourceUri = ((nav == null) ? string.Empty : nav.BaseURI);
		}

		private static string CreateMessage(string message, XPathNavigator nav)
		{
			IXmlLineInfo xmlLineInfo = nav as IXmlLineInfo;
			int num = ((xmlLineInfo != null) ? xmlLineInfo.LineNumber : 0);
			int num2 = ((xmlLineInfo != null) ? xmlLineInfo.LinePosition : 0);
			string text = ((nav == null) ? string.Empty : nav.BaseURI);
			if (num != 0)
			{
				return CreateMessage("{0} at {1}({2},{3}).", message, num, num2, text);
			}
			return CreateMessage("{0}.", message, num, num2, text);
		}

		private static string CreateMessage(string msgFormat, string message, int lineNumber, int linePosition, string sourceUri)
		{
			return string.Format(CultureInfo.InvariantCulture, msgFormat, message, sourceUri, lineNumber.ToString(CultureInfo.InvariantCulture), linePosition.ToString(CultureInfo.InvariantCulture));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("lineNumber", lineNumber);
			info.AddValue("linePosition", linePosition);
			info.AddValue("sourceUri", sourceUri);
			info.AddValue("templateFrames", templateFrames);
		}

		internal void AddTemplateFrame(string frame)
		{
			templateFrames += frame;
		}
	}
}
