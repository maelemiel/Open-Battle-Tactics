namespace System.Xml.Schema
{
	[System.MonoTODO]
	public class XmlSchemaInfo : IXmlSchemaInfo
	{
		private bool isDefault;

		private bool isNil;

		private XmlSchemaSimpleType memberType;

		private XmlSchemaAttribute attr;

		private XmlSchemaElement elem;

		private XmlSchemaType type;

		private XmlSchemaValidity validity;

		private XmlSchemaContentType contentType;

		[System.MonoTODO]
		public XmlSchemaContentType ContentType
		{
			get
			{
				return contentType;
			}
			set
			{
				contentType = value;
			}
		}

		[System.MonoTODO]
		public bool IsDefault
		{
			get
			{
				return isDefault;
			}
			set
			{
				isDefault = value;
			}
		}

		[System.MonoTODO]
		public bool IsNil
		{
			get
			{
				return isNil;
			}
			set
			{
				isNil = value;
			}
		}

		[System.MonoTODO]
		public XmlSchemaSimpleType MemberType
		{
			get
			{
				return memberType;
			}
			set
			{
				memberType = value;
			}
		}

		[System.MonoTODO]
		public XmlSchemaAttribute SchemaAttribute
		{
			get
			{
				return attr;
			}
			set
			{
				attr = value;
			}
		}

		[System.MonoTODO]
		public XmlSchemaElement SchemaElement
		{
			get
			{
				return elem;
			}
			set
			{
				elem = value;
			}
		}

		[System.MonoTODO]
		public XmlSchemaType SchemaType
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

		[System.MonoTODO]
		public XmlSchemaValidity Validity
		{
			get
			{
				return validity;
			}
			set
			{
				validity = value;
			}
		}

		public XmlSchemaInfo()
		{
		}

		internal XmlSchemaInfo(IXmlSchemaInfo info)
		{
			isDefault = info.IsDefault;
			isNil = info.IsNil;
			memberType = info.MemberType;
			attr = info.SchemaAttribute;
			elem = info.SchemaElement;
			type = info.SchemaType;
			validity = info.Validity;
		}
	}
}
