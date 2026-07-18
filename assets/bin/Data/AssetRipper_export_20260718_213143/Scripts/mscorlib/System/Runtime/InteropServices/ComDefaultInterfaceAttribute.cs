namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	[ComVisible(true)]
	public sealed class ComDefaultInterfaceAttribute : Attribute
	{
		private Type _type;

		public Type Value
		{
			get
			{
				return _type;
			}
		}

		public ComDefaultInterfaceAttribute(Type defaultInterface)
		{
			_type = defaultInterface;
		}
	}
}
