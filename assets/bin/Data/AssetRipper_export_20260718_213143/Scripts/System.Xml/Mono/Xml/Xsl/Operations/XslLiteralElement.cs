using System.Collections;
using System.Xml;
using System.Xml.XPath;

namespace Mono.Xml.Xsl.Operations
{
	internal class XslLiteralElement : XslCompiledElement
	{
		private class XslLiteralAttribute
		{
			private string localname;

			private string prefix;

			private string nsUri;

			private XslAvt val;

			public XslLiteralAttribute(Compiler c)
			{
				prefix = c.Input.Prefix;
				if (prefix.Length > 0)
				{
					string actualPrefix = c.CurrentStylesheet.GetActualPrefix(prefix);
					if (actualPrefix != prefix)
					{
						prefix = actualPrefix;
						XPathNavigator xPathNavigator = c.Input.Clone();
						xPathNavigator.MoveToParent();
						nsUri = xPathNavigator.GetNamespace(actualPrefix);
					}
					else
					{
						nsUri = c.Input.NamespaceURI;
					}
				}
				else
				{
					nsUri = string.Empty;
				}
				localname = c.Input.LocalName;
				val = new XslAvt(c.Input.Value, c);
			}

			public void Evaluate(XslTransformProcessor p)
			{
				p.Out.WriteAttributeString(prefix, localname, nsUri, val.Evaluate(p));
			}
		}

		private XslOperation children;

		private string localname;

		private string prefix;

		private string nsUri;

		private bool isEmptyElement;

		private ArrayList attrs;

		private XmlQualifiedName[] useAttributeSets;

		private Hashtable nsDecls;

		public XslLiteralElement(Compiler c)
			: base(c)
		{
		}

		protected override void Compile(Compiler c)
		{
			if (c.Debugger != null)
			{
				c.Debugger.DebugCompile(base.DebugInput);
			}
			prefix = c.Input.Prefix;
			string actualPrefix = c.CurrentStylesheet.GetActualPrefix(prefix);
			if (actualPrefix != prefix)
			{
				prefix = actualPrefix;
				nsUri = c.Input.GetNamespace(actualPrefix);
			}
			else
			{
				nsUri = c.Input.NamespaceURI;
			}
			localname = c.Input.LocalName;
			useAttributeSets = c.ParseQNameListAttribute("use-attribute-sets", "http://www.w3.org/1999/XSL/Transform");
			nsDecls = c.GetNamespacesToCopy();
			if (nsDecls.Count == 0)
			{
				nsDecls = null;
			}
			isEmptyElement = c.Input.IsEmptyElement;
			if (c.Input.MoveToFirstAttribute())
			{
				attrs = new ArrayList();
				do
				{
					if (!(c.Input.NamespaceURI == "http://www.w3.org/1999/XSL/Transform"))
					{
						attrs.Add(new XslLiteralAttribute(c));
					}
				}
				while (c.Input.MoveToNextAttribute());
				c.Input.MoveToParent();
			}
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
			bool insideCDataElement = p.InsideCDataElement;
			p.PushElementState(prefix, localname, nsUri, true);
			p.Out.WriteStartElement(prefix, localname, nsUri);
			if (useAttributeSets != null)
			{
				XmlQualifiedName[] array = useAttributeSets;
				foreach (XmlQualifiedName name in array)
				{
					p.ResolveAttributeSet(name).Evaluate(p);
				}
			}
			if (attrs != null)
			{
				int count = attrs.Count;
				for (int j = 0; j < count; j++)
				{
					((XslLiteralAttribute)attrs[j]).Evaluate(p);
				}
			}
			p.OutputLiteralNamespaceUriNodes(nsDecls, null, null);
			if (children != null)
			{
				children.Evaluate(p);
			}
			if (isEmptyElement)
			{
				p.Out.WriteEndElement();
			}
			else
			{
				p.Out.WriteFullEndElement();
			}
			p.PopCDataState(insideCDataElement);
		}
	}
}
