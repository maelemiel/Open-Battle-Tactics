namespace System.Runtime.CompilerServices
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Assembly)]
	public sealed class DefaultDependencyAttribute : Attribute
	{
		private LoadHint hint;

		public LoadHint LoadHint
		{
			get
			{
				return hint;
			}
		}

		public DefaultDependencyAttribute(LoadHint loadHintArgument)
		{
			hint = loadHintArgument;
		}
	}
}
