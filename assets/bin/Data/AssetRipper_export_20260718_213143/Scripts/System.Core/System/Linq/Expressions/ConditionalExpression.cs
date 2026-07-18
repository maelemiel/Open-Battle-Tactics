using System.Reflection.Emit;

namespace System.Linq.Expressions
{
	public sealed class ConditionalExpression : Expression
	{
		private Expression test;

		private Expression if_true;

		private Expression if_false;

		public Expression Test
		{
			get
			{
				return test;
			}
		}

		public Expression IfTrue
		{
			get
			{
				return if_true;
			}
		}

		public Expression IfFalse
		{
			get
			{
				return if_false;
			}
		}

		internal ConditionalExpression(Expression test, Expression if_true, Expression if_false)
			: base(ExpressionType.Conditional, if_true.Type)
		{
			this.test = test;
			this.if_true = if_true;
			this.if_false = if_false;
		}

		internal override void Emit(EmitContext ec)
		{
			ILGenerator ig = ec.ig;
			Label label = ig.DefineLabel();
			Label label2 = ig.DefineLabel();
			test.Emit(ec);
			ig.Emit(OpCodes.Brfalse, label);
			if_true.Emit(ec);
			ig.Emit(OpCodes.Br, label2);
			ig.MarkLabel(label);
			if_false.Emit(ec);
			ig.MarkLabel(label2);
		}
	}
}
