using System.Xml.XPath;
using System.Xml.Xsl;
using Mono.Xml.Xsl;

namespace Mono.Xml.XPath
{
	internal abstract class Pattern
	{
		public virtual double DefaultPriority
		{
			get
			{
				return 0.5;
			}
		}

		public virtual XPathNodeType EvaluatedNodeType
		{
			get
			{
				return XPathNodeType.All;
			}
		}

		internal static Pattern Compile(string s, Compiler comp)
		{
			return Compile(comp.patternParser.Compile(s));
		}

		internal static Pattern Compile(Expression e)
		{
			if (e is ExprUNION)
			{
				return new UnionPattern(Compile(((ExprUNION)e).left), Compile(((ExprUNION)e).right));
			}
			if (e is ExprRoot)
			{
				return new LocationPathPattern(new NodeTypeTest(Axes.Self, XPathNodeType.Root));
			}
			if (e is NodeTest)
			{
				return new LocationPathPattern((NodeTest)e);
			}
			if (e is ExprFilter)
			{
				return new LocationPathPattern((ExprFilter)e);
			}
			if (e is ExprSLASH)
			{
				Pattern prev = Compile(((ExprSLASH)e).left);
				LocationPathPattern locationPathPattern = (LocationPathPattern)Compile(((ExprSLASH)e).right);
				locationPathPattern.SetPreviousPattern(prev, false);
				return locationPathPattern;
			}
			if (e is ExprSLASH2)
			{
				if (((ExprSLASH2)e).left is ExprRoot)
				{
					return Compile(((ExprSLASH2)e).right);
				}
				Pattern prev2 = Compile(((ExprSLASH2)e).left);
				LocationPathPattern locationPathPattern2 = (LocationPathPattern)Compile(((ExprSLASH2)e).right);
				locationPathPattern2.SetPreviousPattern(prev2, true);
				return locationPathPattern2;
			}
			if (e is XPathFunctionId)
			{
				ExprLiteral exprLiteral = ((XPathFunctionId)e).Id as ExprLiteral;
				return new IdPattern(exprLiteral.Value);
			}
			if (e is XsltKey)
			{
				return new KeyPattern((XsltKey)e);
			}
			return null;
		}

		public abstract bool Matches(XPathNavigator node, XsltContext ctx);
	}
}
