namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Interface, Inherited = false)]
	[ComVisible(true)]
	public sealed class ComEventInterfaceAttribute : Attribute
	{
		private Type si;

		private Type ep;

		public Type EventProvider
		{
			get
			{
				return ep;
			}
		}

		public Type SourceInterface
		{
			get
			{
				return si;
			}
		}

		public ComEventInterfaceAttribute(Type SourceInterface, Type EventProvider)
		{
			si = SourceInterface;
			ep = EventProvider;
		}
	}
}
