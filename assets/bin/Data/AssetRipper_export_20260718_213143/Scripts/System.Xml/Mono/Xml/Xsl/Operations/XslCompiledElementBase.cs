using System.Xml;
using System.Xml.XPath;

namespace Mono.Xml.Xsl.Operations
{
	internal abstract class XslCompiledElementBase : XslOperation
	{
		private int lineNumber;

		private int linePosition;

		private XPathNavigator debugInput;

		public XPathNavigator DebugInput
		{
			get
			{
				return debugInput;
			}
		}

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

		public XslCompiledElementBase(Compiler c)
		{
			IXmlLineInfo xmlLineInfo = c.Input as IXmlLineInfo;
			if (xmlLineInfo != null)
			{
				lineNumber = xmlLineInfo.LineNumber;
				linePosition = xmlLineInfo.LinePosition;
			}
			if (c.Debugger != null)
			{
				debugInput = c.Input.Clone();
			}
		}

		protected abstract void Compile(Compiler c);
	}
}
