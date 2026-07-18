using System.Xml.XPath;

namespace Mono.Xml.Xsl.Operations
{
	internal class XslComment : XslCompiledElement
	{
		private XslOperation value;

		public XslComment(Compiler c)
			: base(c)
		{
		}

		protected override void Compile(Compiler c)
		{
			if (c.Debugger != null)
			{
				c.Debugger.DebugCompile(c.Input);
			}
			c.CheckExtraAttributes("comment");
			if (c.Input.MoveToFirstChild())
			{
				value = c.CompileTemplateContent(XPathNodeType.Comment);
				c.Input.MoveToParent();
			}
		}

		public override void Evaluate(XslTransformProcessor p)
		{
			if (p.Debugger != null)
			{
				p.Debugger.DebugExecute(p, base.DebugInput);
			}
			p.Out.WriteComment((value != null) ? value.EvaluateAsString(p) : string.Empty);
		}
	}
}
