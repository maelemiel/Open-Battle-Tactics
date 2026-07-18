namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Field, Inherited = false)]
	[ComVisible(true)]
	public sealed class FieldOffsetAttribute : Attribute
	{
		private int val;

		public int Value
		{
			get
			{
				return val;
			}
		}

		public FieldOffsetAttribute(int offset)
		{
			val = offset;
		}
	}
}
