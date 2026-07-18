using System.Collections;

namespace System.Xml.XPath
{
	internal class XPathFunctionId : XPathFunction
	{
		private Expression arg0;

		private static char[] rgchWhitespace = new char[4] { ' ', '\t', '\r', '\n' };

		public Expression Id
		{
			get
			{
				return arg0;
			}
		}

		public override XPathResultType ReturnType
		{
			get
			{
				return XPathResultType.NodeSet;
			}
		}

		internal override bool Peer
		{
			get
			{
				return arg0.Peer;
			}
		}

		public XPathFunctionId(FunctionArguments args)
			: base(args)
		{
			if (args == null || args.Tail != null)
			{
				throw new XPathException("id takes 1 arg");
			}
			arg0 = args.Arg;
		}

		public override object Evaluate(BaseIterator iter)
		{
			object obj = arg0.Evaluate(iter);
			XPathNodeIterator xPathNodeIterator = obj as XPathNodeIterator;
			string text;
			if (xPathNodeIterator != null)
			{
				text = string.Empty;
				while (xPathNodeIterator.MoveNext())
				{
					text = text + xPathNodeIterator.Current.Value + " ";
				}
			}
			else
			{
				text = XPathFunctions.ToString(obj);
			}
			XPathNavigator xPathNavigator = iter.Current.Clone();
			ArrayList arrayList = new ArrayList();
			string[] array = text.Split(rgchWhitespace);
			for (int i = 0; i < array.Length; i++)
			{
				if (xPathNavigator.MoveToId(array[i]))
				{
					arrayList.Add(xPathNavigator.Clone());
				}
			}
			arrayList.Sort(XPathNavigatorComparer.Instance);
			return new ListIterator(iter, arrayList);
		}

		public override string ToString()
		{
			return "id(" + arg0.ToString() + ")";
		}
	}
}
