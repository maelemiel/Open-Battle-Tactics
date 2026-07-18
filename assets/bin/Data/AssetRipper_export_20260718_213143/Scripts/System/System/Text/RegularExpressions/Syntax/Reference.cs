namespace System.Text.RegularExpressions.Syntax
{
	internal class Reference : System.Text.RegularExpressions.Syntax.Expression
	{
		private System.Text.RegularExpressions.Syntax.CapturingGroup group;

		private bool ignore;

		public System.Text.RegularExpressions.Syntax.CapturingGroup CapturingGroup
		{
			get
			{
				return group;
			}
			set
			{
				group = value;
			}
		}

		public bool IgnoreCase
		{
			get
			{
				return ignore;
			}
			set
			{
				ignore = value;
			}
		}

		public Reference(bool ignore)
		{
			this.ignore = ignore;
		}

		public override void Compile(System.Text.RegularExpressions.ICompiler cmp, bool reverse)
		{
			cmp.EmitReference(group.Index, ignore, reverse);
		}

		public override void GetWidth(out int min, out int max)
		{
			min = 0;
			max = int.MaxValue;
		}

		public override bool IsComplex()
		{
			return true;
		}
	}
}
