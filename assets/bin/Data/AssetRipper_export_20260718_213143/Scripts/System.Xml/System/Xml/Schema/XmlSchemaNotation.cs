using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaNotation : XmlSchemaAnnotated
	{
		private const string xmlname = "notation";

		private string name;

		private string pub;

		private string system;

		private XmlQualifiedName qualifiedName;

		[XmlAttribute("name")]
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		[XmlAttribute("public")]
		public string Public
		{
			get
			{
				return pub;
			}
			set
			{
				pub = value;
			}
		}

		[XmlAttribute("system")]
		public string System
		{
			get
			{
				return system;
			}
			set
			{
				system = value;
			}
		}

		[XmlIgnore]
		internal XmlQualifiedName QualifiedName
		{
			get
			{
				return qualifiedName;
			}
		}

		internal override int Compile(ValidationEventHandler h, XmlSchema schema)
		{
			if (CompilationId == schema.CompilationId)
			{
				return 0;
			}
			if (Name == null)
			{
				error(h, "Required attribute name must be present");
			}
			else if (!XmlSchemaUtil.CheckNCName(name))
			{
				error(h, "attribute name must be NCName");
			}
			else
			{
				qualifiedName = new XmlQualifiedName(Name, base.AncestorSchema.TargetNamespace);
			}
			if (Public == null)
			{
				error(h, "public must be present");
			}
			else if (!XmlSchemaUtil.CheckAnyUri(Public))
			{
				error(h, "public must be anyURI");
			}
			if (system != null && !XmlSchemaUtil.CheckAnyUri(system))
			{
				error(h, "system must be present and of Type anyURI");
			}
			XmlSchemaUtil.CompileID(base.Id, this, schema.IDCollection, h);
			return errorCount;
		}

		internal override int Validate(ValidationEventHandler h, XmlSchema schema)
		{
			return errorCount;
		}

		internal static XmlSchemaNotation Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaNotation xmlSchemaNotation = new XmlSchemaNotation();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "notation")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaInclude.Read, name=" + reader.Name, null);
				reader.Skip();
				return null;
			}
			xmlSchemaNotation.LineNumber = reader.LineNumber;
			xmlSchemaNotation.LinePosition = reader.LinePosition;
			xmlSchemaNotation.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "id")
				{
					xmlSchemaNotation.Id = reader.Value;
				}
				else if (reader.Name == "name")
				{
					xmlSchemaNotation.name = reader.Value;
				}
				else if (reader.Name == "public")
				{
					xmlSchemaNotation.pub = reader.Value;
				}
				else if (reader.Name == "system")
				{
					xmlSchemaNotation.system = reader.Value;
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for notation", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaNotation);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaNotation;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "notation")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaNotation.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaNotation.Annotation = xmlSchemaAnnotation;
					}
				}
				else
				{
					reader.RaiseInvalidElementError();
				}
			}
			return xmlSchemaNotation;
		}
	}
}
