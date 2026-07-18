namespace System.Text.RegularExpressions.Syntax
{
	internal class Repetition : System.Text.RegularExpressions.Syntax.CompositeExpression
	{
		private int min;

		private int max;

		private bool lazy;

		public System.Text.RegularExpressions.Syntax.Expression Expression
		{
			get
			{
				return base.Expressions[0];
			}
			set
			{
				base.Expressions[0] = value;
			}
		}

		public int Minimum
		{
			get
			{
				return min;
			}
			set
			{
				min = value;
			}
		}

		public int Maximum
		{
			get
			{
				return max;
			}
			set
			{
				max = value;
			}
		}

		public bool Lazy
		{
			get
			{
				return lazy;
			}
			set
			{
				lazy = value;
			}
		}

		public Repetition(int min, int max, bool lazy)
		{
			base.Expressions.Add(null);
			this.min = min;
			this.max = max;
			this.lazy = lazy;
		}

		public override void Compile(System.Text.RegularExpressions.ICompiler cmp, bool reverse)
		{
			if (Expression.IsComplex())
			{
				System.Text.RegularExpressions.LinkRef linkRef = cmp.NewLink();
				cmp.EmitRepeat(min, max, lazy, linkRef);
				Expression.Compile(cmp, reverse);
				cmp.EmitUntil(linkRef);
			}
			else
			{
				System.Text.RegularExpressions.LinkRef linkRef2 = cmp.NewLink();
				cmp.EmitFastRepeat(min, max, lazy, linkRef2);
				Expression.Compile(cmp, reverse);
				cmp.EmitTrue();
				cmp.ResolveLink(linkRef2);
			}
		}

		public override void GetWidth(out int min, out int max)
		{
			Expression.GetWidth(out min, out max);
			min *= this.min;
			if (max == int.MaxValue || this.max == 65535)
			{
				max = int.MaxValue;
			}
			else
			{
				max *= this.max;
			}
		}

		public override System.Text.RegularExpressions.Syntax.AnchorInfo GetAnchorInfo(bool reverse)
		{
			int fixedWidth = GetFixedWidth();
			if (Minimum == 0)
			{
				return new System.Text.RegularExpressions.Syntax.AnchorInfo(this, fixedWidth);
			}
			System.Text.RegularExpressions.Syntax.AnchorInfo anchorInfo = Expression.GetAnchorInfo(reverse);
			if (anchorInfo.IsPosition)
			{
				return new System.Text.RegularExpressions.Syntax.AnchorInfo(this, anchorInfo.Offset, fixedWidth, anchorInfo.Position);
			}
			if (anchorInfo.IsSubstring)
			{
				if (anchorInfo.IsComplete)
				{
					string substring = anchorInfo.Substring;
					StringBuilder stringBuilder = new StringBuilder(substring);
					for (int i = 1; i < Minimum; i++)
					{
						stringBuilder.Append(substring);
					}
					return new System.Text.RegularExpressions.Syntax.AnchorInfo(this, 0, fixedWidth, stringBuilder.ToString(), anchorInfo.IgnoreCase);
				}
				return new System.Text.RegularExpressions.Syntax.AnchorInfo(this, anchorInfo.Offset, fixedWidth, anchorInfo.Substring, anchorInfo.IgnoreCase);
			}
			return new System.Text.RegularExpressions.Syntax.AnchorInfo(this, fixedWidth);
		}
	}
}
