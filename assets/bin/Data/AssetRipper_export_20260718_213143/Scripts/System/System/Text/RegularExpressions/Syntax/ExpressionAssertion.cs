namespace System.Text.RegularExpressions.Syntax
{
	internal class ExpressionAssertion : System.Text.RegularExpressions.Syntax.Assertion
	{
		private bool reverse;

		private bool negate;

		public bool Reverse
		{
			get
			{
				return reverse;
			}
			set
			{
				reverse = value;
			}
		}

		public bool Negate
		{
			get
			{
				return negate;
			}
			set
			{
				negate = value;
			}
		}

		public System.Text.RegularExpressions.Syntax.Expression TestExpression
		{
			get
			{
				return base.Expressions[2];
			}
			set
			{
				base.Expressions[2] = value;
			}
		}

		public ExpressionAssertion()
		{
			base.Expressions.Add(null);
		}

		public override void Compile(System.Text.RegularExpressions.ICompiler cmp, bool reverse)
		{
			System.Text.RegularExpressions.LinkRef linkRef = cmp.NewLink();
			System.Text.RegularExpressions.LinkRef linkRef2 = cmp.NewLink();
			if (!negate)
			{
				cmp.EmitTest(linkRef, linkRef2);
			}
			else
			{
				cmp.EmitTest(linkRef2, linkRef);
			}
			TestExpression.Compile(cmp, this.reverse);
			cmp.EmitTrue();
			if (base.TrueExpression == null)
			{
				cmp.ResolveLink(linkRef2);
				cmp.EmitFalse();
				cmp.ResolveLink(linkRef);
				return;
			}
			cmp.ResolveLink(linkRef);
			base.TrueExpression.Compile(cmp, reverse);
			if (base.FalseExpression == null)
			{
				cmp.ResolveLink(linkRef2);
				return;
			}
			System.Text.RegularExpressions.LinkRef linkRef3 = cmp.NewLink();
			cmp.EmitJump(linkRef3);
			cmp.ResolveLink(linkRef2);
			base.FalseExpression.Compile(cmp, reverse);
			cmp.ResolveLink(linkRef3);
		}

		public override bool IsComplex()
		{
			return true;
		}
	}
}
