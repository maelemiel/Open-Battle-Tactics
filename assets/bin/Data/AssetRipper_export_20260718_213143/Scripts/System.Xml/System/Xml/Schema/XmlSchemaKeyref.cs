using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaKeyref : XmlSchemaIdentityConstraint
	{
		private const string xmlname = "keyref";

		private XmlQualifiedName refer;

		private XmlSchemaIdentityConstraint target;

		[XmlAttribute("refer")]
		public XmlQualifiedName Refer
		{
			get
			{
				return refer;
			}
			set
			{
				refer = value;
			}
		}

		internal XmlSchemaIdentityConstraint Target
		{
			get
			{
				return target;
			}
		}

		public XmlSchemaKeyref()
		{
			refer = XmlQualifiedName.Empty;
		}

		internal override int Compile(ValidationEventHandler h, XmlSchema schema)
		{
			base.Compile(h, schema);
			if (refer == null || refer.IsEmpty)
			{
				error(h, "refer must be present");
			}
			else if (!XmlSchemaUtil.CheckQName(refer))
			{
				error(h, "Refer is not a valid XmlQualifiedName");
			}
			return errorCount;
		}

		internal override int Validate(ValidationEventHandler h, XmlSchema schema)
		{
			XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint = schema.NamedIdentities[Refer] as XmlSchemaIdentityConstraint;
			if (xmlSchemaIdentityConstraint == null)
			{
				error(h, "Target key was not found.");
			}
			else if (xmlSchemaIdentityConstraint is XmlSchemaKeyref)
			{
				error(h, "Target identity constraint was keyref.");
			}
			else if (xmlSchemaIdentityConstraint.Fields.Count != base.Fields.Count)
			{
				error(h, "Target identity constraint has different number of fields.");
			}
			else
			{
				target = xmlSchemaIdentityConstraint;
			}
			return errorCount;
		}

		internal static XmlSchemaKeyref Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaKeyref xmlSchemaKeyref = new XmlSchemaKeyref();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "keyref")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaKeyref.Read, name=" + reader.Name, null);
				reader.Skip();
				return null;
			}
			xmlSchemaKeyref.LineNumber = reader.LineNumber;
			xmlSchemaKeyref.LinePosition = reader.LinePosition;
			xmlSchemaKeyref.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "id")
				{
					xmlSchemaKeyref.Id = reader.Value;
				}
				else if (reader.Name == "name")
				{
					xmlSchemaKeyref.Name = reader.Value;
				}
				else if (reader.Name == "refer")
				{
					Exception innerEx;
					xmlSchemaKeyref.refer = XmlSchemaUtil.ReadQNameAttribute(reader, out innerEx);
					if (innerEx != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is not a valid value for refer attribute", innerEx);
					}
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for keyref", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaKeyref);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaKeyref;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "keyref")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaKeyref.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaKeyref.Annotation = xmlSchemaAnnotation;
					}
				}
				else if (num <= 2 && reader.LocalName == "selector")
				{
					num = 3;
					XmlSchemaXPath xmlSchemaXPath = XmlSchemaXPath.Read(reader, h, "selector");
					if (xmlSchemaXPath != null)
					{
						xmlSchemaKeyref.Selector = xmlSchemaXPath;
					}
				}
				else if (num <= 3 && reader.LocalName == "field")
				{
					num = 3;
					if (xmlSchemaKeyref.Selector == null)
					{
						XmlSchemaObject.error(h, "selector must be defined before field declarations", null);
					}
					XmlSchemaXPath xmlSchemaXPath2 = XmlSchemaXPath.Read(reader, h, "field");
					if (xmlSchemaXPath2 != null)
					{
						xmlSchemaKeyref.Fields.Add(xmlSchemaXPath2);
					}
				}
				else
				{
					reader.RaiseInvalidElementError();
				}
			}
			return xmlSchemaKeyref;
		}
	}
}
