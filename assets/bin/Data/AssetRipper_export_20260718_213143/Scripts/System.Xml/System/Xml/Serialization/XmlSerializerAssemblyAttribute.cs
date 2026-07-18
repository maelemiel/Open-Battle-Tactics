namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
	public sealed class XmlSerializerAssemblyAttribute : Attribute
	{
		private string _assemblyName;

		private string _codeBase;

		public string AssemblyName
		{
			get
			{
				return _assemblyName;
			}
			set
			{
				_assemblyName = value;
			}
		}

		public string CodeBase
		{
			get
			{
				return _codeBase;
			}
			set
			{
				_codeBase = value;
			}
		}

		public XmlSerializerAssemblyAttribute()
		{
		}

		public XmlSerializerAssemblyAttribute(string assemblyName)
		{
			_assemblyName = assemblyName;
		}

		public XmlSerializerAssemblyAttribute(string assemblyName, string codeBase)
			: this(assemblyName)
		{
			_codeBase = codeBase;
		}
	}
}
