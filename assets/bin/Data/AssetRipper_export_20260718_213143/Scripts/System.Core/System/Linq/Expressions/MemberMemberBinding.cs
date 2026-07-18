using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
	public sealed class MemberMemberBinding : MemberBinding
	{
		private ReadOnlyCollection<MemberBinding> bindings;

		public ReadOnlyCollection<MemberBinding> Bindings
		{
			get
			{
				return bindings;
			}
		}

		internal MemberMemberBinding(MemberInfo member, ReadOnlyCollection<MemberBinding> bindings)
			: base(MemberBindingType.MemberBinding, member)
		{
			this.bindings = bindings;
		}

		internal override void Emit(EmitContext ec, LocalBuilder local)
		{
			LocalBuilder local2 = EmitLoadMember(ec, local);
			foreach (MemberBinding binding in bindings)
			{
				binding.Emit(ec, local2);
			}
		}
	}
}
