using System.Xml.Xsl;

namespace System.Xml.XPath
{
	internal class NodeNameTest : NodeTest
	{
		protected XmlQualifiedName _name;

		protected readonly bool resolvedName;

		public XmlQualifiedName Name
		{
			get
			{
				return _name;
			}
		}

		public NodeNameTest(Axes axis, XmlQualifiedName name, IStaticXsltContext ctx)
			: base(axis)
		{
			if (ctx != null)
			{
				name = ctx.LookupQName(name.ToString());
				resolvedName = true;
			}
			_name = name;
		}

		public NodeNameTest(Axes axis, XmlQualifiedName name, bool resolvedName)
			: base(axis)
		{
			_name = name;
			this.resolvedName = resolvedName;
		}

		internal NodeNameTest(NodeNameTest source, Axes axis)
			: base(axis)
		{
			_name = source._name;
			resolvedName = source.resolvedName;
		}

		public override string ToString()
		{
			return _axis.ToString() + "::" + _name.ToString();
		}

		public override bool Match(IXmlNamespaceResolver nsm, XPathNavigator nav)
		{
			if (nav.NodeType != _axis.NodeType)
			{
				return false;
			}
			if (_name.Name != string.Empty && _name.Name != nav.LocalName)
			{
				return false;
			}
			string text = string.Empty;
			if (_name.Namespace != string.Empty)
			{
				if (resolvedName)
				{
					text = _name.Namespace;
				}
				else if (nsm != null)
				{
					text = nsm.LookupNamespace(_name.Namespace);
				}
				if (text == null)
				{
					throw new XPathException("Invalid namespace prefix: " + _name.Namespace);
				}
			}
			return text == nav.NamespaceURI;
		}

		public override void GetInfo(out string name, out string ns, out XPathNodeType nodetype, IXmlNamespaceResolver nsm)
		{
			nodetype = _axis.NodeType;
			if (_name.Name != string.Empty)
			{
				name = _name.Name;
			}
			else
			{
				name = null;
			}
			ns = string.Empty;
			if (nsm != null && _name.Namespace != string.Empty)
			{
				if (resolvedName)
				{
					ns = _name.Namespace;
				}
				else
				{
					ns = nsm.LookupNamespace(_name.Namespace);
				}
				if (ns == null)
				{
					throw new XPathException("Invalid namespace prefix: " + _name.Namespace);
				}
			}
		}
	}
}
