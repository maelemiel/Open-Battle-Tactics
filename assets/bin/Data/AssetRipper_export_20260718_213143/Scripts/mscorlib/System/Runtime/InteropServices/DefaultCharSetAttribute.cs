namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Module, Inherited = false)]
	public sealed class DefaultCharSetAttribute : Attribute
	{
		private CharSet _set;

		public CharSet CharSet
		{
			get
			{
				return _set;
			}
		}

		public DefaultCharSetAttribute(CharSet charSet)
		{
			_set = charSet;
		}
	}
}
