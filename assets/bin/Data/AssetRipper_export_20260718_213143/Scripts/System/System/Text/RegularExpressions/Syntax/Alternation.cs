namespace System.Text.RegularExpressions.Syntax
{
	internal class Alternation : System.Text.RegularExpressions.Syntax.CompositeExpression
	{
		public System.Text.RegularExpressions.Syntax.ExpressionCollection Alternatives
		{
			get
			{
				return base.Expressions;
			}
		}

		public void AddAlternative(System.Text.RegularExpressions.Syntax.Expression e)
		{
			Alternatives.Add(e);
		}

		public override void Compile(System.Text.RegularExpressions.ICompiler cmp, bool reverse)
		{
			System.Text.RegularExpressions.LinkRef linkRef = cmp.NewLink();
			foreach (System.Text.RegularExpressions.Syntax.Expression alternative in Alternatives)
			{
				System.Text.RegularExpressions.LinkRef linkRef2 = cmp.NewLink();
				cmp.EmitBranch(linkRef2);
				alternative.Compile(cmp, reverse);
				cmp.EmitJump(linkRef);
				cmp.ResolveLink(linkRef2);
				cmp.EmitBranchEnd();
			}
			cmp.EmitFalse();
			cmp.ResolveLink(linkRef);
			cmp.EmitAlternationEnd();
		}

		public override void GetWidth(out int min, out int max)
		{
			GetWidth(out min, out max, Alternatives.Count);
		}
	}
}
