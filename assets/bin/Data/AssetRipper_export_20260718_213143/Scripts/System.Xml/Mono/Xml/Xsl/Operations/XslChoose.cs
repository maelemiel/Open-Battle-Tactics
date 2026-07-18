using System.Collections;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl.Operations
{
	internal class XslChoose : XslCompiledElement
	{
		private XslOperation defaultChoice;

		private ArrayList conditions = new ArrayList();

		public XslChoose(Compiler c)
			: base(c)
		{
		}

		protected override void Compile(Compiler c)
		{
			if (c.Debugger != null)
			{
				c.Debugger.DebugCompile(c.Input);
			}
			c.CheckExtraAttributes("choose");
			if (!c.Input.MoveToFirstChild())
			{
				throw new XsltCompileException("Expecting non-empty element", null, c.Input);
			}
			do
			{
				if (c.Input.NodeType != XPathNodeType.Element || c.Input.NamespaceURI != "http://www.w3.org/1999/XSL/Transform")
				{
					continue;
				}
				if (defaultChoice != null)
				{
					throw new XsltCompileException("otherwise attribute must be last", null, c.Input);
				}
				switch (c.Input.LocalName)
				{
				case "when":
					conditions.Add(new XslIf(c));
					break;
				case "otherwise":
					c.CheckExtraAttributes("otherwise");
					if (c.Input.MoveToFirstChild())
					{
						defaultChoice = c.CompileTemplateContent();
						c.Input.MoveToParent();
					}
					break;
				default:
					if (c.CurrentStylesheet.Version == "1.0")
					{
						throw new XsltCompileException("XSLT choose element accepts only when and otherwise elements", null, c.Input);
					}
					break;
				}
			}
			while (c.Input.MoveToNext());
			c.Input.MoveToParent();
			if (conditions.Count == 0)
			{
				throw new XsltCompileException("Choose must have 1 or more when elements", null, c.Input);
			}
		}

		public override void Evaluate(XslTransformProcessor p)
		{
			if (p.Debugger != null)
			{
				p.Debugger.DebugExecute(p, base.DebugInput);
			}
			int count = conditions.Count;
			for (int i = 0; i < count; i++)
			{
				if (((XslIf)conditions[i]).EvaluateIfTrue(p))
				{
					return;
				}
			}
			if (defaultChoice != null)
			{
				defaultChoice.Evaluate(p);
			}
		}
	}
}
