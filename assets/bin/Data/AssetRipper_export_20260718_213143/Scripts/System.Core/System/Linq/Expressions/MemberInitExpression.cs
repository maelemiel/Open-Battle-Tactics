using System.Collections.ObjectModel;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
	public sealed class MemberInitExpression : Expression
	{
		private NewExpression new_expression;

		private ReadOnlyCollection<MemberBinding> bindings;

		public NewExpression NewExpression
		{
			get
			{
				return new_expression;
			}
		}

		public ReadOnlyCollection<MemberBinding> Bindings
		{
			get
			{
				return bindings;
			}
		}

		internal MemberInitExpression(NewExpression new_expression, ReadOnlyCollection<MemberBinding> bindings)
			: base(ExpressionType.MemberInit, new_expression.Type)
		{
			this.new_expression = new_expression;
			this.bindings = bindings;
		}

		internal override void Emit(EmitContext ec)
		{
			LocalBuilder local = ec.EmitStored(new_expression);
			ec.EmitCollection(bindings, local);
			ec.EmitLoad(local);
		}
	}
}
