namespace System.Xml.Serialization
{
	[AttributeUsage(AttributeTargets.Assembly)]
	public sealed class XmlSerializerVersionAttribute : Attribute
	{
		private string _namespace;

		private string _parentAssemblyId;

		private Type _type;

		private string _version;

		public string Namespace
		{
			get
			{
				return _namespace;
			}
			set
			{
				_namespace = value;
			}
		}

		public string ParentAssemblyId
		{
			get
			{
				return _parentAssemblyId;
			}
			set
			{
				_parentAssemblyId = value;
			}
		}

		public Type Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		public string Version
		{
			get
			{
				return _version;
			}
			set
			{
				_version = value;
			}
		}

		public XmlSerializerVersionAttribute()
		{
		}

		public XmlSerializerVersionAttribute(Type type)
		{
			_type = type;
		}
	}
}
