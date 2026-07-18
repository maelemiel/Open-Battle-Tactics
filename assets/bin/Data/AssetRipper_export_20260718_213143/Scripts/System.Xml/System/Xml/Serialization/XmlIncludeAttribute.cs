namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = true)]
	public class XmlIncludeAttribute : Attribute
	{
		private Type type;

		public Type Type
		{
			get
			{
				return type;
			}
			set
			{
				type = value;
			}
		}

		public XmlIncludeAttribute(Type type)
		{
			this.type = type;
		}
	}
}
