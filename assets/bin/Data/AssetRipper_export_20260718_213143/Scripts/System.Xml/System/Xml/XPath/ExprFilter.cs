namespace System.Xml.XPath
{
	internal class ExprFilter : NodeSet
	{
		internal Expression expr;

		internal Expression pred;

		internal Expression LeftHandSide
		{
			get
			{
				return expr;
			}
		}

		internal override XPathNodeType EvaluatedNodeType
		{
			get
			{
				return expr.EvaluatedNodeType;
			}
		}

		internal override bool IsPositional
		{
			get
			{
				if (pred.ReturnType == XPathResultType.Number)
				{
					return true;
				}
				return expr.IsPositional || pred.IsPositional;
			}
		}

		internal override bool Peer
		{
			get
			{
				return expr.Peer && pred.Peer;
			}
		}

		internal override bool Subtree
		{
			get
			{
				NodeSet nodeSet = expr as NodeSet;
				return nodeSet != null && nodeSet.Subtree;
			}
		}

		public ExprFilter(Expression expr, Expression pred)
		{
			this.expr = expr;
			this.pred = pred;
		}

		public override Expression Optimize()
		{
			expr = expr.Optimize();
			pred = pred.Optimize();
			return this;
		}

		public override string ToString()
		{
			return "(" + expr.ToString() + ")[" + pred.ToString() + "]";
		}

		public override object Evaluate(BaseIterator iter)
		{
			BaseIterator iter2 = expr.EvaluateNodeSet(iter);
			return new PredicateIterator(iter2, pred);
		}
	}
}
