using System.Reflection;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
	public sealed class MemberExpression : Expression
	{
		private Expression expression;

		private MemberInfo member;

		public Expression Expression
		{
			get
			{
				return expression;
			}
		}

		public MemberInfo Member
		{
			get
			{
				return member;
			}
		}

		internal MemberExpression(Expression expression, MemberInfo member, Type type)
			: base(ExpressionType.MemberAccess, type)
		{
			this.expression = expression;
			this.member = member;
		}

		internal override void Emit(EmitContext ec)
		{
			member.OnFieldOrProperty(delegate(FieldInfo field)
			{
				EmitFieldAccess(ec, field);
			}, delegate(PropertyInfo prop)
			{
				EmitPropertyAccess(ec, prop);
			});
		}

		private void EmitPropertyAccess(EmitContext ec, PropertyInfo property)
		{
			MethodInfo getMethod = property.GetGetMethod(true);
			if (!getMethod.IsStatic)
			{
				ec.EmitLoadSubject(expression);
			}
			ec.EmitCall(getMethod);
		}

		private void EmitFieldAccess(EmitContext ec, FieldInfo field)
		{
			if (!field.IsStatic)
			{
				ec.EmitLoadSubject(expression);
				ec.ig.Emit(OpCodes.Ldfld, field);
			}
			else
			{
				ec.ig.Emit(OpCodes.Ldsfld, field);
			}
		}
	}
}
