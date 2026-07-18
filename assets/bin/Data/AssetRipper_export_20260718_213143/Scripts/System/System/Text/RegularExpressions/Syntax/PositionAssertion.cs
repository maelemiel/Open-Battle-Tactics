namespace System.Text.RegularExpressions.Syntax
{
	internal class PositionAssertion : System.Text.RegularExpressions.Syntax.Expression
	{
		private System.Text.RegularExpressions.Position pos;

		public System.Text.RegularExpressions.Position Position
		{
			get
			{
				return pos;
			}
			set
			{
				pos = value;
			}
		}

		public PositionAssertion(System.Text.RegularExpressions.Position pos)
		{
			this.pos = pos;
		}

		public override void Compile(System.Text.RegularExpressions.ICompiler cmp, bool reverse)
		{
			cmp.EmitPosition(pos);
		}

		public override void GetWidth(out int min, out int max)
		{
			min = (max = 0);
		}

		public override bool IsComplex()
		{
			return false;
		}

		public override System.Text.RegularExpressions.Syntax.AnchorInfo GetAnchorInfo(bool revers)
		{
			switch (pos)
			{
			case System.Text.RegularExpressions.Position.StartOfString:
			case System.Text.RegularExpressions.Position.StartOfLine:
			case System.Text.RegularExpressions.Position.StartOfScan:
				return new System.Text.RegularExpressions.Syntax.AnchorInfo(this, 0, 0, pos);
			default:
				return new System.Text.RegularExpressions.Syntax.AnchorInfo(this, 0);
			}
		}
	}
}
