using System.Xml.XPath;
using System.Xml.Xsl;
using Mono.Xml.Xsl;

namespace Mono.Xml.XPath
{
	internal class LocationPathPattern : Pattern
	{
		private LocationPathPattern patternPrevious;

		private bool isAncestor;

		private NodeTest nodeTest;

		private ExprFilter filter;

		public override double DefaultPriority
		{
			get
			{
				if (patternPrevious == null && filter == null)
				{
					NodeNameTest nodeNameTest = nodeTest as NodeNameTest;
					if (nodeNameTest != null)
					{
						if (nodeNameTest.Name.Name == "*" || nodeNameTest.Name.Name.Length == 0)
						{
							return -0.25;
						}
						return 0.0;
					}
					return -0.5;
				}
				return 0.5;
			}
		}

		public override XPathNodeType EvaluatedNodeType
		{
			get
			{
				return nodeTest.EvaluatedNodeType;
			}
		}

		public LocationPathPattern LastPathPattern
		{
			get
			{
				LocationPathPattern locationPathPattern = this;
				while (locationPathPattern.patternPrevious != null)
				{
					locationPathPattern = locationPathPattern.patternPrevious;
				}
				return locationPathPattern;
			}
		}

		public LocationPathPattern(NodeTest nodeTest)
		{
			this.nodeTest = nodeTest;
		}

		public LocationPathPattern(ExprFilter filter)
		{
			this.filter = filter;
			while (!(filter.expr is NodeTest))
			{
				filter = (ExprFilter)filter.expr;
			}
			nodeTest = (NodeTest)filter.expr;
		}

		internal void SetPreviousPattern(Pattern prev, bool isAncestor)
		{
			LocationPathPattern lastPathPattern = LastPathPattern;
			lastPathPattern.patternPrevious = (LocationPathPattern)prev;
			lastPathPattern.isAncestor = isAncestor;
		}

		public override bool Matches(XPathNavigator node, XsltContext ctx)
		{
			if (!nodeTest.Match(ctx, node))
			{
				return false;
			}
			if (nodeTest is NodeTypeTest && ((NodeTypeTest)nodeTest).type == XPathNodeType.All && (node.NodeType == XPathNodeType.Root || node.NodeType == XPathNodeType.Attribute))
			{
				return false;
			}
			if (filter == null && patternPrevious == null)
			{
				return true;
			}
			XPathNavigator navCache;
			if (patternPrevious != null)
			{
				navCache = ((XsltCompiledContext)ctx).GetNavCache(this, node);
				if (!isAncestor)
				{
					navCache.MoveToParent();
					if (!patternPrevious.Matches(navCache, ctx))
					{
						return false;
					}
				}
				else
				{
					do
					{
						if (!navCache.MoveToParent())
						{
							return false;
						}
					}
					while (!patternPrevious.Matches(navCache, ctx));
				}
			}
			if (filter == null)
			{
				return true;
			}
			if (!filter.IsPositional && !(filter.expr is ExprFilter))
			{
				return filter.pred.EvaluateBoolean(new NullIterator(node, ctx));
			}
			navCache = ((XsltCompiledContext)ctx).GetNavCache(this, node);
			navCache.MoveToParent();
			BaseIterator baseIterator = filter.EvaluateNodeSet(new NullIterator(navCache, ctx));
			while (baseIterator.MoveNext())
			{
				if (node.IsSamePosition(baseIterator.Current))
				{
					return true;
				}
			}
			return false;
		}

		public override string ToString()
		{
			string text = string.Empty;
			if (patternPrevious != null)
			{
				text = patternPrevious.ToString() + ((!isAncestor) ? "/" : "//");
			}
			if (filter != null)
			{
				return text + filter.ToString();
			}
			return text + nodeTest.ToString();
		}
	}
}
