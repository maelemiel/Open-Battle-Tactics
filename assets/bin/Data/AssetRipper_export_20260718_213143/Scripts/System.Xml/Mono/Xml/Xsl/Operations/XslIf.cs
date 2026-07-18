using System.Xml.XPath;

namespace Mono.Xml.Xsl.Operations
{
	internal class XslIf : XslCompiledElement
	{
		private CompiledExpression test;

		private XslOperation children;

		public XslIf(Compiler c)
			: base(c)
		{
		}

		protected override void Compile(Compiler c)
		{
			if (c.Debugger != null)
			{
				c.Debugger.DebugCompile(c.Input);
			}
			c.CheckExtraAttributes("if", "test");
			c.AssertAttribute("test");
			test = c.CompileExpression(c.GetAttribute("test"));
			if (c.Input.MoveToFirstChild())
			{
				children = c.CompileTemplateContent();
				c.Input.MoveToParent();
			}
		}

		public bool EvaluateIfTrue(XslTransformProcessor p)
		{
			if (p.EvaluateBoolean(test))
			{
				if (children != null)
				{
					children.Evaluate(p);
				}
				return true;
			}
			return false;
		}

		public override void Evaluate(XslTransformProcessor p)
		{
			if (p.Debugger != null)
			{
				p.Debugger.DebugExecute(p, base.DebugInput);
			}
			EvaluateIfTrue(p);
		}
	}
}
