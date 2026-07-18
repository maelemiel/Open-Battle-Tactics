namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Interface, Inherited = false)]
	[ComVisible(true)]
	public sealed class TypeLibImportClassAttribute : Attribute
	{
		private string _importClass;

		public string Value
		{
			get
			{
				return _importClass;
			}
		}

		public TypeLibImportClassAttribute(Type importClass)
		{
			_importClass = importClass.ToString();
		}
	}
}
