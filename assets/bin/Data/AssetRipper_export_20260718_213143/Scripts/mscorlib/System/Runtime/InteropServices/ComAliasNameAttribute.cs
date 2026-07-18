namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false)]
	public sealed class ComAliasNameAttribute : Attribute
	{
		private string val;

		public string Value
		{
			get
			{
				return val;
			}
		}

		public ComAliasNameAttribute(string alias)
		{
			val = alias;
		}
	}
}
