namespace System.Runtime.CompilerServices
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class DependencyAttribute : Attribute
	{
		private string dependentAssembly;

		private LoadHint hint;

		public string DependentAssembly
		{
			get
			{
				return dependentAssembly;
			}
		}

		public LoadHint LoadHint
		{
			get
			{
				return hint;
			}
		}

		public DependencyAttribute(string dependentAssemblyArgument, LoadHint loadHintArgument)
		{
			dependentAssembly = dependentAssemblyArgument;
			hint = loadHintArgument;
		}
	}
}
