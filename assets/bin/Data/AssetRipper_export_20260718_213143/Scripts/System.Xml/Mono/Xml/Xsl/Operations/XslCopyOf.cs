using System.Xml.XPath;

namespace Mono.Xml.Xsl.Operations
{
	internal class XslCopyOf : XslCompiledElement
	{
		private XPathExpression select;

		public XslCopyOf(Compiler c)
			: base(c)
		{
		}

		protected override void Compile(Compiler c)
		{
			if (c.Debugger != null)
			{
				c.Debugger.DebugCompile(c.Input);
			}
			c.CheckExtraAttributes("copy-of", "select");
			c.AssertAttribute("select");
			select = c.CompileExpression(c.GetAttribute("select"));
		}

		private void CopyNode(XslTransformProcessor p, XPathNavigator nav)
		{
			Outputter outputter = p.Out;
			switch (nav.NodeType)
			{
			case XPathNodeType.Root:
			{
				XPathNodeIterator xPathNodeIterator = nav.SelectChildren(XPathNodeType.All);
				while (xPathNodeIterator.MoveNext())
				{
					CopyNode(p, xPathNodeIterator.Current);
				}
				break;
			}
			case XPathNodeType.Element:
			{
				bool insideCDataElement = p.InsideCDataElement;
				string prefix = nav.Prefix;
				string namespaceURI = nav.NamespaceURI;
				p.PushElementState(prefix, nav.LocalName, namespaceURI, false);
				outputter.WriteStartElement(prefix, nav.LocalName, namespaceURI);
				if (nav.MoveToFirstNamespace(XPathNamespaceScope.ExcludeXml))
				{
					do
					{
						if (!(prefix == nav.Name) && (nav.Name.Length != 0 || namespaceURI.Length != 0))
						{
							outputter.WriteNamespaceDecl(nav.Name, nav.Value);
						}
					}
					while (nav.MoveToNextNamespace(XPathNamespaceScope.ExcludeXml));
					nav.MoveToParent();
				}
				if (nav.MoveToFirstAttribute())
				{
					do
					{
						outputter.WriteAttributeString(nav.Prefix, nav.LocalName, nav.NamespaceURI, nav.Value);
					}
					while (nav.MoveToNextAttribute());
					nav.MoveToParent();
				}
				if (nav.MoveToFirstChild())
				{
					do
					{
						CopyNode(p, nav);
					}
					while (nav.MoveToNext());
					nav.MoveToParent();
				}
				if (nav.IsEmptyElement)
				{
					outputter.WriteEndElement();
				}
				else
				{
					outputter.WriteFullEndElement();
				}
				p.PopCDataState(insideCDataElement);
				break;
			}
			case XPathNodeType.Namespace:
				if (nav.Name != p.XPathContext.ElementPrefix && (p.XPathContext.ElementNamespace.Length > 0 || nav.Name.Length > 0))
				{
					outputter.WriteNamespaceDecl(nav.Name, nav.Value);
				}
				break;
			case XPathNodeType.Attribute:
				outputter.WriteAttributeString(nav.Prefix, nav.LocalName, nav.NamespaceURI, nav.Value);
				break;
			case XPathNodeType.SignificantWhitespace:
			case XPathNodeType.Whitespace:
			{
				bool insideCDataSection = outputter.InsideCDataSection;
				outputter.InsideCDataSection = false;
				outputter.WriteString(nav.Value);
				outputter.InsideCDataSection = insideCDataSection;
				break;
			}
			case XPathNodeType.Text:
				outputter.WriteString(nav.Value);
				break;
			case XPathNodeType.ProcessingInstruction:
				outputter.WriteProcessingInstruction(nav.Name, nav.Value);
				break;
			case XPathNodeType.Comment:
				outputter.WriteComment(nav.Value);
				break;
			}
		}

		public override void Evaluate(XslTransformProcessor p)
		{
			if (p.Debugger != null)
			{
				p.Debugger.DebugExecute(p, base.DebugInput);
			}
			object obj = p.Evaluate(select);
			XPathNodeIterator xPathNodeIterator = obj as XPathNodeIterator;
			if (xPathNodeIterator != null)
			{
				while (xPathNodeIterator.MoveNext())
				{
					CopyNode(p, xPathNodeIterator.Current);
				}
				return;
			}
			XPathNavigator xPathNavigator = obj as XPathNavigator;
			if (xPathNavigator != null)
			{
				CopyNode(p, xPathNavigator);
			}
			else
			{
				p.Out.WriteString(XPathFunctions.ToString(obj));
			}
		}
	}
}
