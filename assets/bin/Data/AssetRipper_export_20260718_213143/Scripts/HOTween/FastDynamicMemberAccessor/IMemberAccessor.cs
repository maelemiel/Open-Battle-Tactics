namespace FastDynamicMemberAccessor
{
	public interface IMemberAccessor
	{
		object Get(object target);

		void Set(object target, object value);
	}
}
