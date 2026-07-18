namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Interface, Inherited = false)]
	public sealed class CoClassAttribute : Attribute
	{
		private Type klass;

		public Type CoClass
		{
			get
			{
				return klass;
			}
		}

		public CoClassAttribute(Type coClass)
		{
			klass = coClass;
		}
	}
}
