using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
	public sealed class MemberListBinding : MemberBinding
	{
		private ReadOnlyCollection<ElementInit> initializers;

		public ReadOnlyCollection<ElementInit> Initializers
		{
			get
			{
				return initializers;
			}
		}

		internal MemberListBinding(MemberInfo member, ReadOnlyCollection<ElementInit> initializers)
			: base(MemberBindingType.ListBinding, member)
		{
			this.initializers = initializers;
		}

		internal override void Emit(EmitContext ec, LocalBuilder local)
		{
			LocalBuilder local2 = EmitLoadMember(ec, local);
			foreach (ElementInit initializer in initializers)
			{
				initializer.Emit(ec, local2);
			}
		}
	}
}
