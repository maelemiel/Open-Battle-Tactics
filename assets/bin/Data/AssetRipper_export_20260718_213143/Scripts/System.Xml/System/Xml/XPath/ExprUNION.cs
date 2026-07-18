namespace System.Xml.XPath
{
	internal class ExprUNION : NodeSet
	{
		internal Expression left;

		internal Expression right;

		internal override XPathNodeType EvaluatedNodeType
		{
			get
			{
				return (left.EvaluatedNodeType != right.EvaluatedNodeType) ? XPathNodeType.All : left.EvaluatedNodeType;
			}
		}

		internal override bool IsPositional
		{
			get
			{
				return left.IsPositional || right.IsPositional;
			}
		}

		internal override bool Peer
		{
			get
			{
				return left.Peer && right.Peer;
			}
		}

		internal override bool Subtree
		{
			get
			{
				NodeSet nodeSet = left as NodeSet;
				NodeSet nodeSet2 = right as NodeSet;
				return nodeSet != null && nodeSet2 != null && nodeSet.Subtree && nodeSet2.Subtree;
			}
		}

		public ExprUNION(Expression left, Expression right)
		{
			this.left = left;
			this.right = right;
		}

		public override Expression Optimize()
		{
			left = left.Optimize();
			right = right.Optimize();
			return this;
		}

		public override string ToString()
		{
			return left.ToString() + " | " + right.ToString();
		}

		public override object Evaluate(BaseIterator iter)
		{
			BaseIterator baseIterator = left.EvaluateNodeSet(iter);
			BaseIterator baseIterator2 = right.EvaluateNodeSet(iter);
			return new UnionIterator(iter, baseIterator, baseIterator2);
		}
	}
}
