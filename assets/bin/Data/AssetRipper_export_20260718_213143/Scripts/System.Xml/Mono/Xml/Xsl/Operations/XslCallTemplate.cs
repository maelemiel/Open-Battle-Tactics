using System.Collections;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl.Operations
{
	internal class XslCallTemplate : XslCompiledElement
	{
		private XmlQualifiedName name;

		private ArrayList withParams;

		public XslCallTemplate(Compiler c)
			: base(c)
		{
		}

		protected override void Compile(Compiler c)
		{
			if (c.Debugger != null)
			{
				c.Debugger.DebugCompile(c.Input);
			}
			c.CheckExtraAttributes("call-template", "name");
			c.AssertAttribute("name");
			name = c.ParseQNameAttribute("name");
			if (!c.Input.MoveToFirstChild())
			{
				return;
			}
			do
			{
				switch (c.Input.NodeType)
				{
				case XPathNodeType.Element:
					if (c.Input.NamespaceURI != "http://www.w3.org/1999/XSL/Transform")
					{
						throw new XsltCompileException("Unexpected element", null, c.Input);
					}
					switch (c.Input.LocalName)
					{
					case "with-param":
						if (withParams == null)
						{
							withParams = new ArrayList();
						}
						withParams.Add(new XslVariableInformation(c));
						break;
					default:
						throw new XsltCompileException("Unexpected element", null, c.Input);
					}
					break;
				default:
					throw new XsltCompileException("Unexpected node type " + c.Input.NodeType, null, c.Input);
				case XPathNodeType.SignificantWhitespace:
				case XPathNodeType.Whitespace:
				case XPathNodeType.ProcessingInstruction:
				case XPathNodeType.Comment:
					break;
				}
			}
			while (c.Input.MoveToNext());
			c.Input.MoveToParent();
		}

		public override void Evaluate(XslTransformProcessor p)
		{
			if (p.Debugger != null)
			{
				p.Debugger.DebugExecute(p, base.DebugInput);
			}
			p.CallTemplate(name, withParams);
		}
	}
}
