using System.Reflection;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
	public abstract class MemberBinding
	{
		private MemberBindingType binding_type;

		private MemberInfo member;

		public MemberBindingType BindingType
		{
			get
			{
				return binding_type;
			}
		}

		public MemberInfo Member
		{
			get
			{
				return member;
			}
		}

		protected MemberBinding(MemberBindingType binding_type, MemberInfo member)
		{
			this.binding_type = binding_type;
			this.member = member;
		}

		public override string ToString()
		{
			return ExpressionPrinter.ToString(this);
		}

		internal abstract void Emit(EmitContext ec, LocalBuilder local);

		internal LocalBuilder EmitLoadMember(EmitContext ec, LocalBuilder local)
		{
			ec.EmitLoadSubject(local);
			return member.OnFieldOrProperty((FieldInfo field) => EmitLoadField(ec, field), (PropertyInfo prop) => EmitLoadProperty(ec, prop));
		}

		private LocalBuilder EmitLoadProperty(EmitContext ec, PropertyInfo property)
		{
			MethodInfo getMethod = property.GetGetMethod(true);
			if (getMethod == null)
			{
				throw new NotSupportedException();
			}
			LocalBuilder localBuilder = ec.ig.DeclareLocal(property.PropertyType);
			ec.EmitCall(getMethod);
			ec.ig.Emit(OpCodes.Stloc, localBuilder);
			return localBuilder;
		}

		private LocalBuilder EmitLoadField(EmitContext ec, FieldInfo field)
		{
			LocalBuilder localBuilder = ec.ig.DeclareLocal(field.FieldType);
			ec.ig.Emit(OpCodes.Ldfld, field);
			ec.ig.Emit(OpCodes.Stloc, localBuilder);
			return localBuilder;
		}
	}
}
