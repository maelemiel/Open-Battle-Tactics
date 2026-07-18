namespace Mono.Xml.Xsl.Operations
{
	internal class XslFallback : XslCompiledElement
	{
		private XslOperation children;

		public XslFallback(Compiler c)
			: base(c)
		{
		}

		protected override void Compile(Compiler c)
		{
			if (c.Debugger != null)
			{
				c.Debugger.DebugCompile(c.Input);
			}
			c.CheckExtraAttributes("fallback");
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
			children.Evaluate(p);
		}
	}
}
