namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Delegate, Inherited = false)]
	[ComVisible(true)]
	public sealed class ComVisibleAttribute : Attribute
	{
		private bool Visible;

		public bool Value
		{
			get
			{
				return Visible;
			}
		}

		public ComVisibleAttribute(bool visibility)
		{
			Visible = visibility;
		}
	}
}
