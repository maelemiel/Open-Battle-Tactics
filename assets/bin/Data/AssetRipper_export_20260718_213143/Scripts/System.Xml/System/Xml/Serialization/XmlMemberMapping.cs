using System.Xml.Schema;

namespace System.Xml.Serialization
{
	public class XmlMemberMapping
	{
		private XmlTypeMapMember _mapMember;

		private string _elementName;

		private string _memberName;

		private string _namespace;

		private string _typeNamespace;

		private XmlSchemaForm _form;

		public bool Any
		{
			get
			{
				return _mapMember is XmlTypeMapMemberAnyElement;
			}
		}

		public string ElementName
		{
			get
			{
				return _elementName;
			}
		}

		public string MemberName
		{
			get
			{
				return _memberName;
			}
		}

		public string Namespace
		{
			get
			{
				return _namespace;
			}
		}

		public string TypeFullName
		{
			get
			{
				return _mapMember.TypeData.FullTypeName;
			}
		}

		public string TypeName
		{
			get
			{
				return _mapMember.TypeData.XmlType;
			}
		}

		public string TypeNamespace
		{
			get
			{
				return _typeNamespace;
			}
		}

		internal XmlTypeMapMember TypeMapMember
		{
			get
			{
				return _mapMember;
			}
		}

		internal XmlSchemaForm Form
		{
			get
			{
				return _form;
			}
		}

		public string XsdElementName
		{
			get
			{
				return _mapMember.Name;
			}
		}

		public bool CheckSpecified
		{
			get
			{
				return _mapMember.IsOptionalValueType;
			}
		}

		internal XmlMemberMapping(string memberName, string defaultNamespace, XmlTypeMapMember mapMem, bool encodedFormat)
		{
			_mapMember = mapMem;
			_memberName = memberName;
			if (mapMem is XmlTypeMapMemberAnyElement)
			{
				XmlTypeMapMemberAnyElement xmlTypeMapMemberAnyElement = (XmlTypeMapMemberAnyElement)mapMem;
				XmlTypeMapElementInfo xmlTypeMapElementInfo = (XmlTypeMapElementInfo)xmlTypeMapMemberAnyElement.ElementInfo[xmlTypeMapMemberAnyElement.ElementInfo.Count - 1];
				_elementName = xmlTypeMapElementInfo.ElementName;
				_namespace = xmlTypeMapElementInfo.Namespace;
				if (xmlTypeMapElementInfo.MappedType != null)
				{
					_typeNamespace = xmlTypeMapElementInfo.MappedType.Namespace;
				}
				else
				{
					_typeNamespace = string.Empty;
				}
			}
			else if (mapMem is XmlTypeMapMemberElement)
			{
				XmlTypeMapElementInfo xmlTypeMapElementInfo2 = (XmlTypeMapElementInfo)((XmlTypeMapMemberElement)mapMem).ElementInfo[0];
				_elementName = xmlTypeMapElementInfo2.ElementName;
				if (encodedFormat)
				{
					_namespace = defaultNamespace;
					if (xmlTypeMapElementInfo2.MappedType != null)
					{
						_typeNamespace = string.Empty;
					}
					else
					{
						_typeNamespace = xmlTypeMapElementInfo2.DataTypeNamespace;
					}
				}
				else
				{
					_namespace = xmlTypeMapElementInfo2.Namespace;
					if (xmlTypeMapElementInfo2.MappedType != null)
					{
						_typeNamespace = xmlTypeMapElementInfo2.MappedType.Namespace;
					}
					else
					{
						_typeNamespace = string.Empty;
					}
					_form = xmlTypeMapElementInfo2.Form;
				}
			}
			else
			{
				_elementName = _memberName;
				_namespace = string.Empty;
			}
			if (_form == XmlSchemaForm.None)
			{
				_form = XmlSchemaForm.Qualified;
			}
		}
	}
}
