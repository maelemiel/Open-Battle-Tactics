namespace System.Text.RegularExpressions.Syntax
{
	internal class NonBacktrackingGroup : System.Text.RegularExpressions.Syntax.Group
	{
		public override void Compile(System.Text.RegularExpressions.ICompiler cmp, bool reverse)
		{
			System.Text.RegularExpressions.LinkRef linkRef = cmp.NewLink();
			cmp.EmitSub(linkRef);
			base.Compile(cmp, reverse);
			cmp.EmitTrue();
			cmp.ResolveLink(linkRef);
		}

		public override bool IsComplex()
		{
			return true;
		}
	}
}
