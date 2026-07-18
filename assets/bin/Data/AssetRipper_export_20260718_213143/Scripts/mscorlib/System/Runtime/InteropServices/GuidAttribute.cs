namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Delegate, Inherited = false)]
	public sealed class GuidAttribute : Attribute
	{
		private string guidValue;

		public string Value
		{
			get
			{
				return guidValue;
			}
		}

		public GuidAttribute(string guid)
		{
			guidValue = guid;
		}
	}
}
