namespace System.Text.RegularExpressions.Syntax
{
	internal class CaptureAssertion : System.Text.RegularExpressions.Syntax.Assertion
	{
		private System.Text.RegularExpressions.Syntax.ExpressionAssertion alternate;

		private System.Text.RegularExpressions.Syntax.CapturingGroup group;

		private System.Text.RegularExpressions.Syntax.Literal literal;

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

		private System.Text.RegularExpressions.Syntax.ExpressionAssertion Alternate
		{
			get
			{
				if (alternate == null)
				{
					alternate = new System.Text.RegularExpressions.Syntax.ExpressionAssertion();
					alternate.TrueExpression = base.TrueExpression;
					alternate.FalseExpression = base.FalseExpression;
					alternate.TestExpression = literal;
				}
				return alternate;
			}
		}

		public CaptureAssertion(System.Text.RegularExpressions.Syntax.Literal l)
		{
			literal = l;
		}

		public override void Compile(System.Text.RegularExpressions.ICompiler cmp, bool reverse)
		{
			if (group == null)
			{
				Alternate.Compile(cmp, reverse);
				return;
			}
			int index = group.Index;
			System.Text.RegularExpressions.LinkRef linkRef = cmp.NewLink();
			if (base.FalseExpression == null)
			{
				cmp.EmitIfDefined(index, linkRef);
				base.TrueExpression.Compile(cmp, reverse);
			}
			else
			{
				System.Text.RegularExpressions.LinkRef linkRef2 = cmp.NewLink();
				cmp.EmitIfDefined(index, linkRef2);
				base.TrueExpression.Compile(cmp, reverse);
				cmp.EmitJump(linkRef);
				cmp.ResolveLink(linkRef2);
				base.FalseExpression.Compile(cmp, reverse);
			}
			cmp.ResolveLink(linkRef);
		}

		public override bool IsComplex()
		{
			if (group == null)
			{
				return Alternate.IsComplex();
			}
			if (base.TrueExpression != null && base.TrueExpression.IsComplex())
			{
				return true;
			}
			if (base.FalseExpression != null && base.FalseExpression.IsComplex())
			{
				return true;
			}
			return GetFixedWidth() <= 0;
		}
	}
}
