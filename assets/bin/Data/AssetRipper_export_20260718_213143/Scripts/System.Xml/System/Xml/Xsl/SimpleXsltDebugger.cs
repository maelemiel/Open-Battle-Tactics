using System.Xml.XPath;

namespace System.Xml.Xsl
{
	internal class SimpleXsltDebugger
	{
		public void OnCompile(XPathNavigator style)
		{
			Console.Write("Compiling: ");
			PrintXPathNavigator(style);
			Console.WriteLine();
		}

		public void OnExecute(XPathNodeIterator currentNodeSet, XPathNavigator style, XsltContext xsltContext)
		{
			Console.Write("Executing: ");
			PrintXPathNavigator(style);
			Console.WriteLine(" / NodeSet: (type {1}) {0} / XsltContext: {2}", currentNodeSet, currentNodeSet.GetType(), xsltContext);
		}

		private void PrintXPathNavigator(XPathNavigator nav)
		{
			IXmlLineInfo xmlLineInfo = nav as IXmlLineInfo;
			object obj;
			if (xmlLineInfo != null && xmlLineInfo.HasLineInfo())
			{
				IXmlLineInfo xmlLineInfo2 = xmlLineInfo;
				obj = xmlLineInfo2;
			}
			else
			{
				obj = null;
			}
			xmlLineInfo = (IXmlLineInfo)obj;
			Console.Write("({0}, {1}) element {2}", (xmlLineInfo != null) ? xmlLineInfo.LineNumber : 0, (xmlLineInfo != null) ? xmlLineInfo.LinePosition : 0, nav.Name);
		}
	}
}
