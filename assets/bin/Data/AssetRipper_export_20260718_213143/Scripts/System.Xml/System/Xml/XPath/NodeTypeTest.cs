namespace System.Xml.XPath
{
	internal class NodeTypeTest : NodeTest
	{
		public readonly XPathNodeType type;

		protected string _param;

		public NodeTypeTest(Axes axis)
			: base(axis)
		{
			type = _axis.NodeType;
		}

		public NodeTypeTest(Axes axis, XPathNodeType type)
			: base(axis)
		{
			this.type = type;
		}

		public NodeTypeTest(Axes axis, XPathNodeType type, string param)
			: base(axis)
		{
			this.type = type;
			_param = param;
			if (param != null && type != XPathNodeType.ProcessingInstruction)
			{
				throw new XPathException("No argument allowed for " + ToString(type) + "() test");
			}
		}

		internal NodeTypeTest(NodeTypeTest other, Axes axis)
			: base(axis)
		{
			type = other.type;
			_param = other._param;
		}

		public override string ToString()
		{
			string text = ToString(type);
			return string.Concat(str2: (type != XPathNodeType.ProcessingInstruction || _param == null) ? (text + "()") : (text + "('" + _param + "')"), str0: _axis.ToString(), str1: "::");
		}

		private static string ToString(XPathNodeType type)
		{
			switch (type)
			{
			case XPathNodeType.Comment:
				return "comment";
			case XPathNodeType.Text:
				return "text";
			case XPathNodeType.ProcessingInstruction:
				return "processing-instruction";
			case XPathNodeType.Element:
			case XPathNodeType.Attribute:
			case XPathNodeType.Namespace:
			case XPathNodeType.All:
				return "node";
			default:
				return "node-type [" + type.ToString() + "]";
			}
		}

		public override bool Match(IXmlNamespaceResolver nsm, XPathNavigator nav)
		{
			XPathNodeType nodeType = nav.NodeType;
			switch (type)
			{
			case XPathNodeType.All:
				return true;
			case XPathNodeType.ProcessingInstruction:
				if (nodeType != XPathNodeType.ProcessingInstruction)
				{
					return false;
				}
				if (_param != null && nav.Name != _param)
				{
					return false;
				}
				return true;
			case XPathNodeType.Text:
				switch (nodeType)
				{
				case XPathNodeType.Text:
				case XPathNodeType.SignificantWhitespace:
				case XPathNodeType.Whitespace:
					return true;
				default:
					return false;
				}
			default:
				return type == nodeType;
			}
		}

		public override void GetInfo(out string name, out string ns, out XPathNodeType nodetype, IXmlNamespaceResolver nsm)
		{
			name = _param;
			ns = null;
			nodetype = type;
		}
	}
}
