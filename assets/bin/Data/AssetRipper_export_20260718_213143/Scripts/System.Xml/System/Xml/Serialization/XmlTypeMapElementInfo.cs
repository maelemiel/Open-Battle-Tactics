using System.Xml.Schema;

namespace System.Xml.Serialization
{
	internal class XmlTypeMapElementInfo
	{
		private string _elementName;

		private string _namespace = string.Empty;

		private XmlSchemaForm _form;

		private XmlTypeMapMember _member;

		private object _choiceValue;

		private bool _isNullable;

		private int _nestingLevel;

		private XmlTypeMapping _mappedType;

		private TypeData _type;

		private bool _wrappedElement = true;

		public TypeData TypeData
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

		public object ChoiceValue
		{
			get
			{
				return _choiceValue;
			}
			set
			{
				_choiceValue = value;
			}
		}

		public string ElementName
		{
			get
			{
				return _elementName;
			}
			set
			{
				_elementName = value;
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
				return _mappedType.XmlTypeNamespace;
			}
		}

		public string DataTypeName
		{
			get
			{
				if (_mappedType == null)
				{
					return TypeData.XmlType;
				}
				return _mappedType.XmlType;
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

		public bool IsNullable
		{
			get
			{
				return _isNullable;
			}
			set
			{
				_isNullable = value;
			}
		}

		internal bool IsPrimitive
		{
			get
			{
				return _mappedType == null;
			}
		}

		public XmlTypeMapMember Member
		{
			get
			{
				return _member;
			}
			set
			{
				_member = value;
			}
		}

		public int NestingLevel
		{
			get
			{
				return _nestingLevel;
			}
			set
			{
				_nestingLevel = value;
			}
		}

		public bool MultiReferenceType
		{
			get
			{
				if (_mappedType != null)
				{
					return _mappedType.MultiReferenceType;
				}
				return false;
			}
		}

		public bool WrappedElement
		{
			get
			{
				return _wrappedElement;
			}
			set
			{
				_wrappedElement = value;
			}
		}

		public bool IsTextElement
		{
			get
			{
				return ElementName == "<text>";
			}
			set
			{
				if (!value)
				{
					throw new Exception("INTERNAL ERROR; someone wrote unexpected code in sys.xml");
				}
				ElementName = "<text>";
				Namespace = string.Empty;
			}
		}

		public bool IsUnnamedAnyElement
		{
			get
			{
				return ElementName == string.Empty;
			}
			set
			{
				if (!value)
				{
					throw new Exception("INTERNAL ERROR; someone wrote unexpected code in sys.xml");
				}
				ElementName = string.Empty;
				Namespace = string.Empty;
			}
		}

		public XmlTypeMapElementInfo(XmlTypeMapMember member, TypeData type)
		{
			_member = member;
			_type = type;
			if (type.IsValueType && type.IsNullable)
			{
				_isNullable = true;
			}
		}

		public override bool Equals(object other)
		{
			if (other == null)
			{
				return false;
			}
			XmlTypeMapElementInfo xmlTypeMapElementInfo = (XmlTypeMapElementInfo)other;
			if (_elementName != xmlTypeMapElementInfo._elementName)
			{
				return false;
			}
			if (_type.XmlType != xmlTypeMapElementInfo._type.XmlType)
			{
				return false;
			}
			if (_namespace != xmlTypeMapElementInfo._namespace)
			{
				return false;
			}
			if (_form != xmlTypeMapElementInfo._form)
			{
				return false;
			}
			if (_type != xmlTypeMapElementInfo._type)
			{
				return false;
			}
			if (_isNullable != xmlTypeMapElementInfo._isNullable)
			{
				return false;
			}
			if (_choiceValue != null && !_choiceValue.Equals(xmlTypeMapElementInfo._choiceValue))
			{
				return false;
			}
			if (_choiceValue != xmlTypeMapElementInfo._choiceValue)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
