using System.Collections;

namespace System.Text.RegularExpressions.Syntax
{
	internal class ExpressionCollection : CollectionBase
	{
		public System.Text.RegularExpressions.Syntax.Expression this[int i]
		{
			get
			{
				return (System.Text.RegularExpressions.Syntax.Expression)base.List[i];
			}
			set
			{
				base.List[i] = value;
			}
		}

		public void Add(System.Text.RegularExpressions.Syntax.Expression e)
		{
			base.List.Add(e);
		}

		protected override void OnValidate(object o)
		{
		}
	}
}
