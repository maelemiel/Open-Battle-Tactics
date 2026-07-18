namespace Mono.Xml.Xsl.Operations
{
	internal class XslLocalVariable : XslGeneralVariable
	{
		protected int slot;

		public override bool IsLocal
		{
			get
			{
				return true;
			}
		}

		public override bool IsParam
		{
			get
			{
				return false;
			}
		}

		public XslLocalVariable(Compiler c)
			: base(c)
		{
			slot = c.AddVariable(this);
		}

		public override void Evaluate(XslTransformProcessor p)
		{
			if (p.Debugger != null)
			{
				p.Debugger.DebugExecute(p, base.DebugInput);
			}
			p.SetStackItem(slot, var.Evaluate(p));
		}

		protected override object GetValue(XslTransformProcessor p)
		{
			return p.GetStackItem(slot);
		}

		public bool IsEvaluated(XslTransformProcessor p)
		{
			return p.GetStackItem(slot) != null;
		}
	}
}
