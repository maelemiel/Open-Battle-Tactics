namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class ProgIdAttribute : Attribute
	{
		private string pid;

		public string Value
		{
			get
			{
				return pid;
			}
		}

		public ProgIdAttribute(string progId)
		{
			pid = progId;
		}
	}
}
