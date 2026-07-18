using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.XPath
{
	internal class UnionPattern : Pattern
	{
		public readonly Pattern p0;

		public readonly Pattern p1;

		public override XPathNodeType EvaluatedNodeType
		{
			get
			{
				return (p0.EvaluatedNodeType != p1.EvaluatedNodeType) ? XPathNodeType.All : p0.EvaluatedNodeType;
			}
		}

		public UnionPattern(Pattern p0, Pattern p1)
		{
			this.p0 = p0;
			this.p1 = p1;
		}

		public override bool Matches(XPathNavigator node, XsltContext ctx)
		{
			return p0.Matches(node, ctx) || p1.Matches(node, ctx);
		}

		public override string ToString()
		{
			return p0.ToString() + " | " + p1.ToString();
		}
	}
}
