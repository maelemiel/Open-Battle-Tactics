namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
	public sealed class XmlSchemaProviderAttribute : Attribute
	{
		private string _methodName;

		private bool _isAny;

		public string MethodName
		{
			get
			{
				return _methodName;
			}
		}

		public bool IsAny
		{
			get
			{
				return _isAny;
			}
			set
			{
				_isAny = value;
			}
		}

		public XmlSchemaProviderAttribute(string methodName)
		{
			_methodName = methodName;
		}
	}
}
