namespace Mono.Xml.Xsl.Operations
{
	internal class XslLocalParam : XslLocalVariable
	{
		public override bool IsParam
		{
			get
			{
				return true;
			}
		}

		public XslLocalParam(Compiler c)
			: base(c)
		{
		}

		public override void Evaluate(XslTransformProcessor p)
		{
			if (p.Debugger != null)
			{
				p.Debugger.DebugExecute(p, base.DebugInput);
			}
			if (p.GetStackItem(slot) != null)
			{
				return;
			}
			if (p.Arguments != null && var.Select == null && var.Content == null)
			{
				object param = p.Arguments.GetParam(base.Name.Name, base.Name.Namespace);
				if (param != null)
				{
					Override(p, param);
					return;
				}
			}
			base.Evaluate(p);
		}

		public void Override(XslTransformProcessor p, object paramVal)
		{
			p.SetStackItem(slot, paramVal);
		}
	}
}
