namespace System.Text.RegularExpressions.Syntax
{
	internal class BalancingGroup : System.Text.RegularExpressions.Syntax.CapturingGroup
	{
		private System.Text.RegularExpressions.Syntax.CapturingGroup balance;

		public System.Text.RegularExpressions.Syntax.CapturingGroup Balance
		{
			get
			{
				return balance;
			}
			set
			{
				balance = value;
			}
		}

		public BalancingGroup()
		{
			balance = null;
		}

		public override void Compile(System.Text.RegularExpressions.ICompiler cmp, bool reverse)
		{
			System.Text.RegularExpressions.LinkRef linkRef = cmp.NewLink();
			cmp.EmitBalanceStart(base.Index, balance.Index, base.IsNamed, linkRef);
			int count = base.Expressions.Count;
			for (int i = 0; i < count; i++)
			{
				System.Text.RegularExpressions.Syntax.Expression expression = ((!reverse) ? base.Expressions[i] : base.Expressions[count - i - 1]);
				expression.Compile(cmp, reverse);
			}
			cmp.EmitBalance();
			cmp.ResolveLink(linkRef);
		}
	}
}
