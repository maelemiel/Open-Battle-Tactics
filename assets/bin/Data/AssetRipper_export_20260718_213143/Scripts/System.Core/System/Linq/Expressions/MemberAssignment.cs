using System.Reflection;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
	public sealed class MemberAssignment : MemberBinding
	{
		private Expression expression;

		public Expression Expression
		{
			get
			{
				return expression;
			}
		}

		internal MemberAssignment(MemberInfo member, Expression expression)
			: base(MemberBindingType.Assignment, member)
		{
			this.expression = expression;
		}

		internal override void Emit(EmitContext ec, LocalBuilder local)
		{
			base.Member.OnFieldOrProperty(delegate(FieldInfo field)
			{
				EmitFieldAssignment(ec, field, local);
			}, delegate(PropertyInfo prop)
			{
				EmitPropertyAssignment(ec, prop, local);
			});
		}

		private void EmitFieldAssignment(EmitContext ec, FieldInfo field, LocalBuilder local)
		{
			ec.EmitLoadSubject(local);
			expression.Emit(ec);
			ec.ig.Emit(OpCodes.Stfld, field);
		}

		private void EmitPropertyAssignment(EmitContext ec, PropertyInfo property, LocalBuilder local)
		{
			MethodInfo setMethod = property.GetSetMethod(true);
			if (setMethod == null)
			{
				throw new InvalidOperationException();
			}
			ec.EmitLoadSubject(local);
			expression.Emit(ec);
			ec.EmitCall(setMethod);
		}
	}
}
