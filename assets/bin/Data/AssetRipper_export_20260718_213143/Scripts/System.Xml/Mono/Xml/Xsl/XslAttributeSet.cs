using System.Collections;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Mono.Xml.Xsl.Operations;

namespace Mono.Xml.Xsl
{
	internal class XslAttributeSet : XslCompiledElement
	{
		private XmlQualifiedName name;

		private ArrayList usedAttributeSets = new ArrayList();

		private ArrayList attributes = new ArrayList();

		public XmlQualifiedName Name
		{
			get
			{
				return name;
			}
		}

		public XslAttributeSet(Compiler c)
			: base(c)
		{
		}

		protected override void Compile(Compiler c)
		{
			name = c.ParseQNameAttribute("name");
			XmlQualifiedName[] array = c.ParseQNameListAttribute("use-attribute-sets");
			if (array != null)
			{
				XmlQualifiedName[] array2 = array;
				foreach (XmlQualifiedName value in array2)
				{
					usedAttributeSets.Add(value);
				}
			}
			if (!c.Input.MoveToFirstChild())
			{
				return;
			}
			do
			{
				if (c.Input.NodeType == XPathNodeType.Element)
				{
					if (c.Input.NamespaceURI != "http://www.w3.org/1999/XSL/Transform" || c.Input.LocalName != "attribute")
					{
						throw new XsltCompileException("Invalid attr set content", null, c.Input);
					}
					attributes.Add(new XslAttribute(c));
				}
			}
			while (c.Input.MoveToNext());
			c.Input.MoveToParent();
		}

		public void Merge(XslAttributeSet s)
		{
			attributes.AddRange(s.attributes);
			foreach (XmlQualifiedName usedAttributeSet in s.usedAttributeSets)
			{
				if (!usedAttributeSets.Contains(usedAttributeSet))
				{
					usedAttributeSets.Add(usedAttributeSet);
				}
			}
		}

		public override void Evaluate(XslTransformProcessor p)
		{
			p.SetBusy(this);
			if (usedAttributeSets != null)
			{
				for (int i = 0; i < usedAttributeSets.Count; i++)
				{
					XmlQualifiedName xmlQualifiedName = (XmlQualifiedName)usedAttributeSets[i];
					XslAttributeSet xslAttributeSet = p.ResolveAttributeSet(xmlQualifiedName);
					if (xslAttributeSet == null)
					{
						throw new XsltException("Could not resolve attribute set", null, p.CurrentNode);
					}
					if (p.IsBusy(xslAttributeSet))
					{
						throw new XsltException("circular dependency", null, p.CurrentNode);
					}
					xslAttributeSet.Evaluate(p);
				}
			}
			for (int j = 0; j < attributes.Count; j++)
			{
				((XslAttribute)attributes[j]).Evaluate(p);
			}
			p.SetFree(this);
		}
	}
}
