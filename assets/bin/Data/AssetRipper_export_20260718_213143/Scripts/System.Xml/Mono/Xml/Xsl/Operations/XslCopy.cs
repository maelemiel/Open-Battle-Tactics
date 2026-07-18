using System.Collections;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl.Operations
{
	internal class XslCopy : XslCompiledElement
	{
		private XslOperation children;

		private XmlQualifiedName[] useAttributeSets;

		private Hashtable nsDecls;

		public XslCopy(Compiler c)
			: base(c)
		{
		}

		protected override void Compile(Compiler c)
		{
			if (c.Debugger != null)
			{
				c.Debugger.DebugCompile(c.Input);
			}
			nsDecls = c.GetNamespacesToCopy();
			if (nsDecls.Count == 0)
			{
				nsDecls = null;
			}
			c.CheckExtraAttributes("copy", "use-attribute-sets");
			useAttributeSets = c.ParseQNameListAttribute("use-attribute-sets");
			if (c.Input.MoveToFirstChild())
			{
				children = c.CompileTemplateContent();
				c.Input.MoveToParent();
			}
		}

		public override void Evaluate(XslTransformProcessor p)
		{
			if (p.Debugger != null)
			{
				p.Debugger.DebugExecute(p, base.DebugInput);
			}
			XPathNavigator currentNode = p.CurrentNode;
			switch (currentNode.NodeType)
			{
			case XPathNodeType.Root:
				if (p.Out.CanProcessAttributes && useAttributeSets != null)
				{
					XmlQualifiedName[] array = useAttributeSets;
					foreach (XmlQualifiedName name in array)
					{
						XslAttributeSet xslAttributeSet = p.ResolveAttributeSet(name);
						if (xslAttributeSet == null)
						{
							throw new XsltException("Attribute set was not found", null, currentNode);
						}
						xslAttributeSet.Evaluate(p);
					}
				}
				if (children != null)
				{
					children.Evaluate(p);
				}
				break;
			case XPathNodeType.Element:
			{
				bool insideCDataElement = p.InsideCDataElement;
				string prefix = currentNode.Prefix;
				p.PushElementState(prefix, currentNode.LocalName, currentNode.NamespaceURI, true);
				p.Out.WriteStartElement(prefix, currentNode.LocalName, currentNode.NamespaceURI);
				if (useAttributeSets != null)
				{
					XmlQualifiedName[] array2 = useAttributeSets;
					foreach (XmlQualifiedName name2 in array2)
					{
						p.ResolveAttributeSet(name2).Evaluate(p);
					}
				}
				if (currentNode.MoveToFirstNamespace(XPathNamespaceScope.ExcludeXml))
				{
					do
					{
						if (!(currentNode.LocalName == prefix))
						{
							p.Out.WriteNamespaceDecl(currentNode.LocalName, currentNode.Value);
						}
					}
					while (currentNode.MoveToNextNamespace(XPathNamespaceScope.ExcludeXml));
					currentNode.MoveToParent();
				}
				if (children != null)
				{
					children.Evaluate(p);
				}
				p.Out.WriteFullEndElement();
				p.PopCDataState(insideCDataElement);
				break;
			}
			case XPathNodeType.Attribute:
				p.Out.WriteAttributeString(currentNode.Prefix, currentNode.LocalName, currentNode.NamespaceURI, currentNode.Value);
				break;
			case XPathNodeType.SignificantWhitespace:
			case XPathNodeType.Whitespace:
			{
				bool insideCDataSection = p.Out.InsideCDataSection;
				p.Out.InsideCDataSection = false;
				p.Out.WriteString(currentNode.Value);
				p.Out.InsideCDataSection = insideCDataSection;
				break;
			}
			case XPathNodeType.Text:
				p.Out.WriteString(currentNode.Value);
				break;
			case XPathNodeType.Comment:
				p.Out.WriteComment(currentNode.Value);
				break;
			case XPathNodeType.ProcessingInstruction:
				p.Out.WriteProcessingInstruction(currentNode.Name, currentNode.Value);
				break;
			case XPathNodeType.Namespace:
				if (p.XPathContext.ElementPrefix != currentNode.Name)
				{
					p.Out.WriteNamespaceDecl(currentNode.Name, currentNode.Value);
				}
				break;
			}
		}
	}
}
