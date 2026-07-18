using System.Xml.XPath;

namespace Mono.Xml.Xsl
{
	internal class ExprKeyContainer : Expression
	{
		private Expression expr;

		public Expression BodyExpression
		{
			get
			{
				return expr;
			}
		}

		internal override XPathNodeType EvaluatedNodeType
		{
			get
			{
				return expr.EvaluatedNodeType;
			}
		}

		public override XPathResultType ReturnType
		{
			get
			{
				return expr.ReturnType;
			}
		}

		public ExprKeyContainer(Expression expr)
		{
			this.expr = expr;
		}

		public override object Evaluate(BaseIterator iter)
		{
			return expr.Evaluate(iter);
		}
	}
}
