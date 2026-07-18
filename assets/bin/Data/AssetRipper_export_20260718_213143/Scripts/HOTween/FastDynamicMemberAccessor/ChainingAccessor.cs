namespace FastDynamicMemberAccessor
{
	internal sealed class ChainingAccessor : IMemberAccessor
	{
		private readonly IMemberAccessor _pimp;

		private readonly IMemberAccessor _chain;

		internal ChainingAccessor(IMemberAccessor impl, IMemberAccessor chain)
		{
			_pimp = impl;
			_chain = chain;
		}

		public object Get(object target)
		{
			return _pimp.Get(_chain.Get(target));
		}

		public void Set(object target, object value)
		{
			_pimp.Set(_chain.Get(target), value);
		}
	}
}
