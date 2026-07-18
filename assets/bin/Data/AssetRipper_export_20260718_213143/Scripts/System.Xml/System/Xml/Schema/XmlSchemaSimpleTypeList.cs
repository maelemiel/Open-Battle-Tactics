using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaSimpleTypeList : XmlSchemaSimpleTypeContent
	{
		private const string xmlname = "list";

		private XmlSchemaSimpleType itemType;

		private XmlQualifiedName itemTypeName;

		private object validatedListItemType;

		private XmlSchemaSimpleType validatedListItemSchemaType;

		[XmlAttribute("itemType")]
		public XmlQualifiedName ItemTypeName
		{
			get
			{
				return itemTypeName;
			}
			set
			{
				itemTypeName = value;
			}
		}

		[XmlElement("simpleType", Type = typeof(XmlSchemaSimpleType))]
		public XmlSchemaSimpleType ItemType
		{
			get
			{
				return itemType;
			}
			set
			{
				itemType = value;
			}
		}

		[XmlIgnore]
		public XmlSchemaSimpleType BaseItemType
		{
			get
			{
				return validatedListItemSchemaType;
			}
			set
			{
			}
		}

		internal object ValidatedListItemType
		{
			get
			{
				return validatedListItemType;
			}
		}

		public XmlSchemaSimpleTypeList()
		{
			ItemTypeName = XmlQualifiedName.Empty;
		}

		internal override void SetParent(XmlSchemaObject parent)
		{
			base.SetParent(parent);
			if (ItemType != null)
			{
				ItemType.SetParent(this);
			}
		}

		internal override int Compile(ValidationEventHandler h, XmlSchema schema)
		{
			if (CompilationId == schema.CompilationId)
			{
				return 0;
			}
			errorCount = 0;
			if (ItemType != null && !ItemTypeName.IsEmpty)
			{
				error(h, "both itemType and simpletype can't be present");
			}
			if (ItemType == null && ItemTypeName.IsEmpty)
			{
				error(h, "one of itemType or simpletype must be present");
			}
			if (ItemType != null)
			{
				errorCount += ItemType.Compile(h, schema);
			}
			if (!XmlSchemaUtil.CheckQName(ItemTypeName))
			{
				error(h, "BaseTypeName must be a XmlQualifiedName");
			}
			XmlSchemaUtil.CompileID(base.Id, this, schema.IDCollection, h);
			CompilationId = schema.CompilationId;
			return errorCount;
		}

		internal override int Validate(ValidationEventHandler h, XmlSchema schema)
		{
			if (IsValidated(schema.ValidationId))
			{
				return errorCount;
			}
			XmlSchemaSimpleType xmlSchemaSimpleType = itemType;
			if (xmlSchemaSimpleType == null)
			{
				xmlSchemaSimpleType = schema.FindSchemaType(itemTypeName) as XmlSchemaSimpleType;
			}
			if (xmlSchemaSimpleType != null)
			{
				errorCount += xmlSchemaSimpleType.Validate(h, schema);
				validatedListItemType = xmlSchemaSimpleType;
			}
			else if (itemTypeName == XmlSchemaComplexType.AnyTypeName)
			{
				validatedListItemType = XmlSchemaSimpleType.AnySimpleType;
			}
			else if (XmlSchemaUtil.IsBuiltInDatatypeName(itemTypeName))
			{
				validatedListItemType = XmlSchemaDatatype.FromName(itemTypeName);
				if (validatedListItemType == null)
				{
					error(h, "Invalid schema type name was specified: " + itemTypeName);
				}
			}
			else if (!schema.IsNamespaceAbsent(itemTypeName.Namespace))
			{
				error(h, string.Concat("Referenced base list item schema type ", itemTypeName, " was not found."));
			}
			XmlSchemaSimpleType xmlSchemaSimpleType2 = validatedListItemType as XmlSchemaSimpleType;
			if (xmlSchemaSimpleType2 == null && validatedListItemType != null)
			{
				xmlSchemaSimpleType2 = XmlSchemaType.GetBuiltInSimpleType(((XmlSchemaDatatype)validatedListItemType).TypeCode);
			}
			validatedListItemSchemaType = xmlSchemaSimpleType2;
			ValidationId = schema.ValidationId;
			return errorCount;
		}

		internal static XmlSchemaSimpleTypeList Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaSimpleTypeList xmlSchemaSimpleTypeList = new XmlSchemaSimpleTypeList();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "list")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaSimpleTypeList.Read, name=" + reader.Name, null);
				reader.Skip();
				return null;
			}
			xmlSchemaSimpleTypeList.LineNumber = reader.LineNumber;
			xmlSchemaSimpleTypeList.LinePosition = reader.LinePosition;
			xmlSchemaSimpleTypeList.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "id")
				{
					xmlSchemaSimpleTypeList.Id = reader.Value;
				}
				else if (reader.Name == "itemType")
				{
					Exception innerEx;
					xmlSchemaSimpleTypeList.ItemTypeName = XmlSchemaUtil.ReadQNameAttribute(reader, out innerEx);
					if (innerEx != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is not a valid value for itemType attribute", innerEx);
					}
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for list", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaSimpleTypeList);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaSimpleTypeList;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "list")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaSimpleTypeList.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaSimpleTypeList.Annotation = xmlSchemaAnnotation;
					}
				}
				else if (num <= 2 && reader.LocalName == "simpleType")
				{
					num = 3;
					XmlSchemaSimpleType xmlSchemaSimpleType = XmlSchemaSimpleType.Read(reader, h);
					if (xmlSchemaSimpleType != null)
					{
						xmlSchemaSimpleTypeList.itemType = xmlSchemaSimpleType;
					}
				}
				else
				{
					reader.RaiseInvalidElementError();
				}
			}
			return xmlSchemaSimpleTypeList;
		}
	}
}
