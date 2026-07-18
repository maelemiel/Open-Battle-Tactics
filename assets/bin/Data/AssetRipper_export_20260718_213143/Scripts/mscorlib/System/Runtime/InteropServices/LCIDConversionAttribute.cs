namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public sealed class LCIDConversionAttribute : Attribute
	{
		private int id;

		public int Value
		{
			get
			{
				return id;
			}
		}

		public LCIDConversionAttribute(int lcid)
		{
			id = lcid;
		}
	}
}
