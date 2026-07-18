using System.Xml.Schema;

namespace System.Xml.Serialization
{
	internal class XmlTypeMapMemberAttribute : XmlTypeMapMember
	{
		private string _attributeName;

		private string _namespace = string.Empty;

		private XmlSchemaForm _form;

		private XmlTypeMapping _mappedType;

		public string AttributeName
		{
			get
			{
				return _attributeName;
			}
			set
			{
				_attributeName = value;
			}
		}

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

		public string DataTypeNamespace
		{
			get
			{
				if (_mappedType == null)
				{
					return "http://www.w3.org/2001/XMLSchema";
				}
				return _mappedType.Namespace;
			}
		}

		public XmlSchemaForm Form
		{
			get
			{
				return _form;
			}
			set
			{
				_form = value;
			}
		}

		public XmlTypeMapping MappedType
		{
			get
			{
				return _mappedType;
			}
			set
			{
				_mappedType = value;
			}
		}
	}
}
