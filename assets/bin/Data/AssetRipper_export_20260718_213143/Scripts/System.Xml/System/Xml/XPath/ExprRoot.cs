namespace System.Xml.XPath
{
	internal class ExprRoot : NodeSet
	{
		internal override XPathNodeType EvaluatedNodeType
		{
			get
			{
				return XPathNodeType.Root;
			}
		}

		internal override bool Peer
		{
			get
			{
				return true;
			}
		}

		internal override bool Subtree
		{
			get
			{
				return false;
			}
		}

		public override string ToString()
		{
			return string.Empty;
		}

		public override object Evaluate(BaseIterator iter)
		{
			if (iter.CurrentPosition == 0)
			{
				iter = (BaseIterator)iter.Clone();
				iter.MoveNext();
			}
			XPathNavigator xPathNavigator = iter.Current.Clone();
			xPathNavigator.MoveToRoot();
			return new SelfIterator(xPathNavigator, iter.NamespaceManager);
		}
	}
}
