using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Mono.Xml.Xsl;

namespace Mono.Xml.XPath
{
	internal class IdPattern : LocationPathPattern
	{
		private string[] ids;

		public override XPathNodeType EvaluatedNodeType
		{
			get
			{
				return XPathNodeType.Element;
			}
		}

		public override double DefaultPriority
		{
			get
			{
				return 0.5;
			}
		}

		public IdPattern(string arg0)
			: base((NodeTest)null)
		{
			ids = arg0.Split(XmlChar.WhitespaceChars);
		}

		public override bool Matches(XPathNavigator node, XsltContext ctx)
		{
			XPathNavigator navCache = ((XsltCompiledContext)ctx).GetNavCache(this, node);
			for (int i = 0; i < ids.Length; i++)
			{
				if (navCache.MoveToId(ids[i]) && navCache.IsSamePosition(node))
				{
					return true;
				}
			}
			return false;
		}
	}
}
