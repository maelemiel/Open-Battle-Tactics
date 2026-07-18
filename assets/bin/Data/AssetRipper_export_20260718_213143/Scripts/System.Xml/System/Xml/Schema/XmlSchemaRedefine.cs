using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaRedefine : XmlSchemaExternal
	{
		private const string xmlname = "redefine";

		private XmlSchemaObjectTable attributeGroups;

		private XmlSchemaObjectTable groups;

		private XmlSchemaObjectCollection items;

		private XmlSchemaObjectTable schemaTypes;

		[XmlElement("simpleType", typeof(XmlSchemaSimpleType))]
		[XmlElement("group", typeof(XmlSchemaGroup))]
		[XmlElement("complexType", typeof(XmlSchemaComplexType))]
		[XmlElement("attributeGroup", typeof(XmlSchemaAttributeGroup))]
		[XmlElement("annotation", typeof(XmlSchemaAnnotation))]
		public XmlSchemaObjectCollection Items
		{
			get
			{
				return items;
			}
		}

		[XmlIgnore]
		public XmlSchemaObjectTable AttributeGroups
		{
			get
			{
				return attributeGroups;
			}
		}

		[XmlIgnore]
		public XmlSchemaObjectTable SchemaTypes
		{
			get
			{
				return schemaTypes;
			}
		}

		[XmlIgnore]
		public XmlSchemaObjectTable Groups
		{
			get
			{
				return groups;
			}
		}

		public XmlSchemaRedefine()
		{
			attributeGroups = new XmlSchemaObjectTable();
			groups = new XmlSchemaObjectTable();
			items = new XmlSchemaObjectCollection(this);
			schemaTypes = new XmlSchemaObjectTable();
		}

		internal override void SetParent(XmlSchemaObject parent)
		{
			base.SetParent(parent);
			foreach (XmlSchemaObject item in Items)
			{
				item.SetParent(this);
				item.isRedefinedComponent = true;
				item.isRedefineChild = true;
			}
		}

		internal static XmlSchemaRedefine Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaRedefine xmlSchemaRedefine = new XmlSchemaRedefine();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "redefine")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaRedefine.Read, name=" + reader.Name, null);
				reader.Skip();
				return null;
			}
			xmlSchemaRedefine.LineNumber = reader.LineNumber;
			xmlSchemaRedefine.LinePosition = reader.LinePosition;
			xmlSchemaRedefine.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "id")
				{
					xmlSchemaRedefine.Id = reader.Value;
				}
				else if (reader.Name == "schemaLocation")
				{
					xmlSchemaRedefine.SchemaLocation = reader.Value;
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for redefine", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaRedefine);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaRedefine;
			}
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "redefine")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaRedefine.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (reader.LocalName == "annotation")
				{
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaRedefine.items.Add(xmlSchemaAnnotation);
					}
				}
				else if (reader.LocalName == "simpleType")
				{
					XmlSchemaSimpleType xmlSchemaSimpleType = XmlSchemaSimpleType.Read(reader, h);
					if (xmlSchemaSimpleType != null)
					{
						xmlSchemaRedefine.items.Add(xmlSchemaSimpleType);
					}
				}
				else if (reader.LocalName == "complexType")
				{
					XmlSchemaComplexType xmlSchemaComplexType = XmlSchemaComplexType.Read(reader, h);
					if (xmlSchemaComplexType != null)
					{
						xmlSchemaRedefine.items.Add(xmlSchemaComplexType);
					}
				}
				else if (reader.LocalName == "group")
				{
					XmlSchemaGroup xmlSchemaGroup = XmlSchemaGroup.Read(reader, h);
					if (xmlSchemaGroup != null)
					{
						xmlSchemaRedefine.items.Add(xmlSchemaGroup);
					}
				}
				else if (reader.LocalName == "attributeGroup")
				{
					XmlSchemaAttributeGroup xmlSchemaAttributeGroup = XmlSchemaAttributeGroup.Read(reader, h);
					if (xmlSchemaAttributeGroup != null)
					{
						xmlSchemaRedefine.items.Add(xmlSchemaAttributeGroup);
					}
				}
				else
				{
					reader.RaiseInvalidElementError();
				}
			}
			return xmlSchemaRedefine;
		}
	}
}
