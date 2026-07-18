using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl.Operations
{
	internal class XslValueOf : XslCompiledElement
	{
		private XPathExpression select;

		private bool disableOutputEscaping;

		public XslValueOf(Compiler c)
			: base(c)
		{
		}

		protected override void Compile(Compiler c)
		{
			if (c.Debugger != null)
			{
				c.Debugger.DebugCompile(base.DebugInput);
			}
			c.CheckExtraAttributes("value-of", "select", "disable-output-escaping");
			c.AssertAttribute("select");
			select = c.CompileExpression(c.GetAttribute("select"));
			disableOutputEscaping = c.ParseYesNoAttribute("disable-output-escaping", false);
			if (!c.Input.MoveToFirstChild())
			{
				return;
			}
			do
			{
				switch (c.Input.NodeType)
				{
				case XPathNodeType.Element:
					if (c.Input.NamespaceURI == "http://www.w3.org/1999/XSL/Transform")
					{
						break;
					}
					continue;
				case XPathNodeType.Text:
				case XPathNodeType.SignificantWhitespace:
					break;
				default:
					continue;
				}
				throw new XsltCompileException("XSLT value-of element cannot contain any child.", null, c.Input);
			}
			while (c.Input.MoveToNext());
		}

		public override void Evaluate(XslTransformProcessor p)
		{
			if (p.Debugger != null)
			{
				p.Debugger.DebugExecute(p, base.DebugInput);
			}
			if (!disableOutputEscaping)
			{
				p.Out.WriteString(p.EvaluateString(select));
			}
			else
			{
				p.Out.WriteRaw(p.EvaluateString(select));
			}
		}
	}
}
