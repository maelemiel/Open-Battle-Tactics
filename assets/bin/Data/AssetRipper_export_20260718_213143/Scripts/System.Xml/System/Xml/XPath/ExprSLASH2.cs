namespace System.Xml.XPath
{
	internal class ExprSLASH2 : NodeSet
	{
		public Expression left;

		public NodeSet right;

		private static NodeTest DescendantOrSelfStar = new NodeTypeTest(Axes.DescendantOrSelf, XPathNodeType.All);

		public override bool RequireSorting
		{
			get
			{
				return left.RequireSorting || right.RequireSorting;
			}
		}

		internal override XPathNodeType EvaluatedNodeType
		{
			get
			{
				return right.EvaluatedNodeType;
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
				return false;
			}
		}

		internal override bool Subtree
		{
			get
			{
				NodeSet nodeSet = left as NodeSet;
				return nodeSet != null && nodeSet.Subtree && right.Subtree;
			}
		}

		public ExprSLASH2(Expression left, NodeSet right)
		{
			this.left = left;
			this.right = right;
		}

		public override Expression Optimize()
		{
			left = left.Optimize();
			right = (NodeSet)right.Optimize();
			NodeTest nodeTest = right as NodeTest;
			if (nodeTest != null && nodeTest.Axis.Axis == Axes.Child)
			{
				NodeNameTest nodeNameTest = nodeTest as NodeNameTest;
				if (nodeNameTest != null)
				{
					return new ExprSLASH(left, new NodeNameTest(nodeNameTest, Axes.Descendant));
				}
				NodeTypeTest nodeTypeTest = nodeTest as NodeTypeTest;
				if (nodeTypeTest != null)
				{
					return new ExprSLASH(left, new NodeTypeTest(nodeTypeTest, Axes.Descendant));
				}
			}
			return this;
		}

		public override string ToString()
		{
			return left.ToString() + "//" + right.ToString();
		}

		public override object Evaluate(BaseIterator iter)
		{
			BaseIterator iter2 = left.EvaluateNodeSet(iter);
			if (left.Peer && !left.RequireSorting)
			{
				iter2 = new SimpleSlashIterator(iter2, DescendantOrSelfStar);
			}
			else
			{
				BaseIterator baseIterator = new SlashIterator(iter2, DescendantOrSelfStar);
				iter2 = ((!left.RequireSorting) ? baseIterator : new SortedIterator(baseIterator));
			}
			SlashIterator iter3 = new SlashIterator(iter2, right);
			return new SortedIterator(iter3);
		}
	}
}
