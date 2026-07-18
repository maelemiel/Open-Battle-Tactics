namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event, Inherited = false)]
	public sealed class DispIdAttribute : Attribute
	{
		private int id;

		public int Value
		{
			get
			{
				return id;
			}
		}

		public DispIdAttribute(int dispId)
		{
			id = dispId;
		}
	}
}
