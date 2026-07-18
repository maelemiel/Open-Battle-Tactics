namespace System.Text.RegularExpressions.Syntax
{
	internal abstract class CompositeExpression : System.Text.RegularExpressions.Syntax.Expression
	{
		private System.Text.RegularExpressions.Syntax.ExpressionCollection expressions;

		protected System.Text.RegularExpressions.Syntax.ExpressionCollection Expressions
		{
			get
			{
				return expressions;
			}
		}

		public CompositeExpression()
		{
			expressions = new System.Text.RegularExpressions.Syntax.ExpressionCollection();
		}

		protected void GetWidth(out int min, out int max, int count)
		{
			min = int.MaxValue;
			max = 0;
			bool flag = true;
			for (int i = 0; i < count; i++)
			{
				System.Text.RegularExpressions.Syntax.Expression expression = Expressions[i];
				if (expression != null)
				{
					flag = false;
					int min2;
					int max2;
					expression.GetWidth(out min2, out max2);
					if (min2 < min)
					{
						min = min2;
					}
					if (max2 > max)
					{
						max = max2;
					}
				}
			}
			if (flag)
			{
				min = (max = 0);
			}
		}

		public override bool IsComplex()
		{
			foreach (System.Text.RegularExpressions.Syntax.Expression expression in Expressions)
			{
				if (expression.IsComplex())
				{
					return true;
				}
			}
			return GetFixedWidth() <= 0;
		}
	}
}
