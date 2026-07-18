using System.Collections;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	internal class XmlSchemaSerializationWriter : XmlSerializationWriter
	{
		private const string xmlNamespace = "http://www.w3.org/2000/xmlns/";

		public void WriteRoot_XmlSchema(object o)
		{
			WriteStartDocument();
			XmlSchema ob = (XmlSchema)o;
			TopLevelElement();
			WriteObject_XmlSchema(ob, "schema", "http://www.w3.org/2001/XMLSchema", true, false, true);
		}

		private void WriteObject_XmlSchema(XmlSchema ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchema))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchema", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			if (ob.AttributeFormDefault != XmlSchemaForm.None)
			{
				WriteAttribute("attributeFormDefault", string.Empty, GetEnumValue_XmlSchemaForm(ob.AttributeFormDefault));
			}
			if (ob.BlockDefault != XmlSchemaDerivationMethod.None)
			{
				WriteAttribute("blockDefault", string.Empty, GetEnumValue_XmlSchemaDerivationMethod(ob.BlockDefault));
			}
			if (ob.FinalDefault != XmlSchemaDerivationMethod.None)
			{
				WriteAttribute("finalDefault", string.Empty, GetEnumValue_XmlSchemaDerivationMethod(ob.FinalDefault));
			}
			if (ob.ElementFormDefault != XmlSchemaForm.None)
			{
				WriteAttribute("elementFormDefault", string.Empty, GetEnumValue_XmlSchemaForm(ob.ElementFormDefault));
			}
			WriteAttribute("targetNamespace", string.Empty, ob.TargetNamespace);
			WriteAttribute("version", string.Empty, ob.Version);
			WriteAttribute("id", string.Empty, ob.Id);
			if (ob.Includes != null)
			{
				for (int i = 0; i < ob.Includes.Count; i++)
				{
					if (ob.Includes[i] == null)
					{
						continue;
					}
					if (ob.Includes[i].GetType() == typeof(XmlSchemaInclude))
					{
						WriteObject_XmlSchemaInclude((XmlSchemaInclude)ob.Includes[i], "include", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Includes[i].GetType() == typeof(XmlSchemaImport))
					{
						WriteObject_XmlSchemaImport((XmlSchemaImport)ob.Includes[i], "import", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Includes[i].GetType() == typeof(XmlSchemaRedefine))
					{
						WriteObject_XmlSchemaRedefine((XmlSchemaRedefine)ob.Includes[i], "redefine", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					throw CreateUnknownTypeException(ob.Includes[i]);
				}
			}
			if (ob.Items != null)
			{
				for (int j = 0; j < ob.Items.Count; j++)
				{
					if (ob.Items[j] == null)
					{
						continue;
					}
					if (ob.Items[j].GetType() == typeof(XmlSchemaElement))
					{
						WriteObject_XmlSchemaElement((XmlSchemaElement)ob.Items[j], "element", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Items[j].GetType() == typeof(XmlSchemaSimpleType))
					{
						WriteObject_XmlSchemaSimpleType((XmlSchemaSimpleType)ob.Items[j], "simpleType", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Items[j].GetType() == typeof(XmlSchemaAttribute))
					{
						WriteObject_XmlSchemaAttribute((XmlSchemaAttribute)ob.Items[j], "attribute", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Items[j].GetType() == typeof(XmlSchemaAnnotation))
					{
						WriteObject_XmlSchemaAnnotation((XmlSchemaAnnotation)ob.Items[j], "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Items[j].GetType() == typeof(XmlSchemaAttributeGroup))
					{
						WriteObject_XmlSchemaAttributeGroup((XmlSchemaAttributeGroup)ob.Items[j], "attributeGroup", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Items[j].GetType() == typeof(XmlSchemaGroup))
					{
						WriteObject_XmlSchemaGroup((XmlSchemaGroup)ob.Items[j], "group", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Items[j].GetType() == typeof(XmlSchemaComplexType))
					{
						WriteObject_XmlSchemaComplexType((XmlSchemaComplexType)ob.Items[j], "complexType", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Items[j].GetType() == typeof(XmlSchemaNotation))
					{
						WriteObject_XmlSchemaNotation((XmlSchemaNotation)ob.Items[j], "notation", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					throw CreateUnknownTypeException(ob.Items[j]);
				}
			}
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private string GetEnumValue_XmlSchemaForm(XmlSchemaForm val)
		{
			switch (val)
			{
			case XmlSchemaForm.Qualified:
				return "qualified";
			case XmlSchemaForm.Unqualified:
				return "unqualified";
			default:
				return ((long)val).ToString(CultureInfo.InvariantCulture);
			}
		}

		private string GetEnumValue_XmlSchemaDerivationMethod(XmlSchemaDerivationMethod val)
		{
			switch (val)
			{
			case XmlSchemaDerivationMethod.Empty:
				return string.Empty;
			case XmlSchemaDerivationMethod.Substitution:
				return "substitution";
			case XmlSchemaDerivationMethod.Extension:
				return "extension";
			case XmlSchemaDerivationMethod.Restriction:
				return "restriction";
			case XmlSchemaDerivationMethod.List:
				return "list";
			case XmlSchemaDerivationMethod.Union:
				return "union";
			case XmlSchemaDerivationMethod.All:
				return "#all";
			default:
			{
				StringBuilder stringBuilder = new StringBuilder();
				string[] array = val.ToString().Split(',');
				string[] array2 = array;
				foreach (string text in array2)
				{
					switch (text.Trim())
					{
					case "Empty":
						stringBuilder.Append(string.Empty).Append(' ');
						break;
					case "Substitution":
						stringBuilder.Append("substitution").Append(' ');
						break;
					case "Extension":
						stringBuilder.Append("extension").Append(' ');
						break;
					case "Restriction":
						stringBuilder.Append("restriction").Append(' ');
						break;
					case "List":
						stringBuilder.Append("list").Append(' ');
						break;
					case "Union":
						stringBuilder.Append("union").Append(' ');
						break;
					case "All":
						stringBuilder.Append("#all").Append(' ');
						break;
					default:
						stringBuilder.Append(text.Trim()).Append(' ');
						break;
					}
				}
				return stringBuilder.ToString().Trim();
			}
			}
		}

		private void WriteObject_XmlSchemaInclude(XmlSchemaInclude ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaInclude))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaInclude", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("schemaLocation", string.Empty, ob.SchemaLocation);
			WriteAttribute("id", string.Empty, ob.Id);
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaImport(XmlSchemaImport ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaImport))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaImport", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("schemaLocation", string.Empty, ob.SchemaLocation);
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("namespace", string.Empty, ob.Namespace);
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaRedefine(XmlSchemaRedefine ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaRedefine))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaRedefine", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("schemaLocation", string.Empty, ob.SchemaLocation);
			WriteAttribute("id", string.Empty, ob.Id);
			if (ob.Items != null)
			{
				for (int i = 0; i < ob.Items.Count; i++)
				{
					if (ob.Items[i] == null)
					{
						continue;
					}
					if (ob.Items[i].GetType() == typeof(XmlSchemaGroup))
					{
						WriteObject_XmlSchemaGroup((XmlSchemaGroup)ob.Items[i], "group", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Items[i].GetType() == typeof(XmlSchemaComplexType))
					{
						WriteObject_XmlSchemaComplexType((XmlSchemaComplexType)ob.Items[i], "complexType", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Items[i].GetType() == typeof(XmlSchemaSimpleType))
					{
						WriteObject_XmlSchemaSimpleType((XmlSchemaSimpleType)ob.Items[i], "simpleType", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Items[i].GetType() == typeof(XmlSchemaAnnotation))
					{
						WriteObject_XmlSchemaAnnotation((XmlSchemaAnnotation)ob.Items[i], "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Items[i].GetType() == typeof(XmlSchemaAttributeGroup))
					{
						WriteObject_XmlSchemaAttributeGroup((XmlSchemaAttributeGroup)ob.Items[i], "attributeGroup", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					throw CreateUnknownTypeException(ob.Items[i]);
				}
			}
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaElement(XmlSchemaElement ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaElement))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaElement", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("minOccurs", string.Empty, ob.MinOccursString);
			WriteAttribute("maxOccurs", string.Empty, ob.MaxOccursString);
			if (ob.IsAbstract)
			{
				WriteAttribute("abstract", string.Empty, (!ob.IsAbstract) ? "false" : "true");
			}
			if (ob.Block != XmlSchemaDerivationMethod.None)
			{
				WriteAttribute("block", string.Empty, GetEnumValue_XmlSchemaDerivationMethod(ob.Block));
			}
			if (ob.DefaultValue != null)
			{
				WriteAttribute("default", string.Empty, ob.DefaultValue);
			}
			if (ob.Final != XmlSchemaDerivationMethod.None)
			{
				WriteAttribute("final", string.Empty, GetEnumValue_XmlSchemaDerivationMethod(ob.Final));
			}
			if (ob.FixedValue != null)
			{
				WriteAttribute("fixed", string.Empty, ob.FixedValue);
			}
			if (ob.Form != XmlSchemaForm.None)
			{
				WriteAttribute("form", string.Empty, GetEnumValue_XmlSchemaForm(ob.Form));
			}
			if (ob.Name != null)
			{
				WriteAttribute("name", string.Empty, ob.Name);
			}
			if (ob.IsNillable)
			{
				WriteAttribute("nillable", string.Empty, (!ob.IsNillable) ? "false" : "true");
			}
			WriteAttribute("ref", string.Empty, FromXmlQualifiedName(ob.RefName));
			WriteAttribute("substitutionGroup", string.Empty, FromXmlQualifiedName(ob.SubstitutionGroup));
			WriteAttribute("type", string.Empty, FromXmlQualifiedName(ob.SchemaTypeName));
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (ob.SchemaType is XmlSchemaSimpleType)
			{
				WriteObject_XmlSchemaSimpleType((XmlSchemaSimpleType)ob.SchemaType, "simpleType", "http://www.w3.org/2001/XMLSchema", false, false, true);
			}
			else if (ob.SchemaType is XmlSchemaComplexType)
			{
				WriteObject_XmlSchemaComplexType((XmlSchemaComplexType)ob.SchemaType, "complexType", "http://www.w3.org/2001/XMLSchema", false, false, true);
			}
			if (ob.Constraints != null)
			{
				for (int i = 0; i < ob.Constraints.Count; i++)
				{
					if (ob.Constraints[i] == null)
					{
						continue;
					}
					if (ob.Constraints[i].GetType() == typeof(XmlSchemaKeyref))
					{
						WriteObject_XmlSchemaKeyref((XmlSchemaKeyref)ob.Constraints[i], "keyref", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Constraints[i].GetType() == typeof(XmlSchemaKey))
					{
						WriteObject_XmlSchemaKey((XmlSchemaKey)ob.Constraints[i], "key", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Constraints[i].GetType() == typeof(XmlSchemaUnique))
					{
						WriteObject_XmlSchemaUnique((XmlSchemaUnique)ob.Constraints[i], "unique", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					throw CreateUnknownTypeException(ob.Constraints[i]);
				}
			}
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaSimpleType(XmlSchemaSimpleType ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaSimpleType))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaSimpleType", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("name", string.Empty, ob.Name);
			if (ob.Final != XmlSchemaDerivationMethod.None)
			{
				WriteAttribute("final", string.Empty, GetEnumValue_XmlSchemaDerivationMethod(ob.Final));
			}
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (ob.Content is XmlSchemaSimpleTypeUnion)
			{
				WriteObject_XmlSchemaSimpleTypeUnion((XmlSchemaSimpleTypeUnion)ob.Content, "union", "http://www.w3.org/2001/XMLSchema", false, false, true);
			}
			else if (ob.Content is XmlSchemaSimpleTypeList)
			{
				WriteObject_XmlSchemaSimpleTypeList((XmlSchemaSimpleTypeList)ob.Content, "list", "http://www.w3.org/2001/XMLSchema", false, false, true);
			}
			else if (ob.Content is XmlSchemaSimpleTypeRestriction)
			{
				WriteObject_XmlSchemaSimpleTypeRestriction((XmlSchemaSimpleTypeRestriction)ob.Content, "restriction", "http://www.w3.org/2001/XMLSchema", false, false, true);
			}
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaAttribute(XmlSchemaAttribute ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaAttribute))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaAttribute", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			if (ob.DefaultValue != null)
			{
				WriteAttribute("default", string.Empty, ob.DefaultValue);
			}
			if (ob.FixedValue != null)
			{
				WriteAttribute("fixed", string.Empty, ob.FixedValue);
			}
			if (ob.Form != XmlSchemaForm.None)
			{
				WriteAttribute("form", string.Empty, GetEnumValue_XmlSchemaForm(ob.Form));
			}
			WriteAttribute("name", string.Empty, ob.Name);
			WriteAttribute("ref", string.Empty, FromXmlQualifiedName(ob.RefName));
			WriteAttribute("type", string.Empty, FromXmlQualifiedName(ob.SchemaTypeName));
			if (ob.Use != XmlSchemaUse.None)
			{
				WriteAttribute("use", string.Empty, GetEnumValue_XmlSchemaUse(ob.Use));
			}
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			WriteObject_XmlSchemaSimpleType(ob.SchemaType, "simpleType", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaAnnotation(XmlSchemaAnnotation ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaAnnotation))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaAnnotation", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			if (ob.Items != null)
			{
				for (int i = 0; i < ob.Items.Count; i++)
				{
					if (ob.Items[i] == null)
					{
						continue;
					}
					if (ob.Items[i].GetType() == typeof(XmlSchemaAppInfo))
					{
						WriteObject_XmlSchemaAppInfo((XmlSchemaAppInfo)ob.Items[i], "appinfo", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Items[i].GetType() == typeof(XmlSchemaDocumentation))
					{
						WriteObject_XmlSchemaDocumentation((XmlSchemaDocumentation)ob.Items[i], "documentation", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					throw CreateUnknownTypeException(ob.Items[i]);
				}
			}
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaAttributeGroup(XmlSchemaAttributeGroup ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaAttributeGroup))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaAttributeGroup", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("name", string.Empty, ob.Name);
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (ob.Attributes != null)
			{
				for (int i = 0; i < ob.Attributes.Count; i++)
				{
					if (ob.Attributes[i] == null)
					{
						continue;
					}
					if (ob.Attributes[i].GetType() == typeof(XmlSchemaAttribute))
					{
						WriteObject_XmlSchemaAttribute((XmlSchemaAttribute)ob.Attributes[i], "attribute", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Attributes[i].GetType() == typeof(XmlSchemaAttributeGroupRef))
					{
						WriteObject_XmlSchemaAttributeGroupRef((XmlSchemaAttributeGroupRef)ob.Attributes[i], "attributeGroup", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					throw CreateUnknownTypeException(ob.Attributes[i]);
				}
			}
			WriteObject_XmlSchemaAnyAttribute(ob.AnyAttribute, "anyAttribute", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaGroup(XmlSchemaGroup ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaGroup))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaGroup", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("name", string.Empty, ob.Name);
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (ob.Particle is XmlSchemaSequence)
			{
				WriteObject_XmlSchemaSequence((XmlSchemaSequence)ob.Particle, "sequence", "http://www.w3.org/2001/XMLSchema", false, false, true);
			}
			else if (ob.Particle is XmlSchemaChoice)
			{
				WriteObject_XmlSchemaChoice((XmlSchemaChoice)ob.Particle, "choice", "http://www.w3.org/2001/XMLSchema", false, false, true);
			}
			else if (ob.Particle is XmlSchemaAll)
			{
				WriteObject_XmlSchemaAll((XmlSchemaAll)ob.Particle, "all", "http://www.w3.org/2001/XMLSchema", false, false, true);
			}
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaComplexType(XmlSchemaComplexType ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaComplexType))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaComplexType", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("name", string.Empty, ob.Name);
			if (ob.Final != XmlSchemaDerivationMethod.None)
			{
				WriteAttribute("final", string.Empty, GetEnumValue_XmlSchemaDerivationMethod(ob.Final));
			}
			if (ob.IsAbstract)
			{
				WriteAttribute("abstract", string.Empty, (!ob.IsAbstract) ? "false" : "true");
			}
			if (ob.Block != XmlSchemaDerivationMethod.None)
			{
				WriteAttribute("block", string.Empty, GetEnumValue_XmlSchemaDerivationMethod(ob.Block));
			}
			if (ob.IsMixed)
			{
				WriteAttribute("mixed", string.Empty, (!ob.IsMixed) ? "false" : "true");
			}
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (ob.ContentModel is XmlSchemaComplexContent)
			{
				WriteObject_XmlSchemaComplexContent((XmlSchemaComplexContent)ob.ContentModel, "complexContent", "http://www.w3.org/2001/XMLSchema", false, false, true);
			}
			else if (ob.ContentModel is XmlSchemaSimpleContent)
			{
				WriteObject_XmlSchemaSimpleContent((XmlSchemaSimpleContent)ob.ContentModel, "simpleContent", "http://www.w3.org/2001/XMLSchema", false, false, true);
			}
			if (ob.Particle is XmlSchemaAll)
			{
				WriteObject_XmlSchemaAll((XmlSchemaAll)ob.Particle, "all", "http://www.w3.org/2001/XMLSchema", false, false, true);
			}
			else if (ob.Particle is XmlSchemaGroupRef)
			{
				WriteObject_XmlSchemaGroupRef((XmlSchemaGroupRef)ob.Particle, "group", "http://www.w3.org/2001/XMLSchema", false, false, true);
			}
			else if (ob.Particle is XmlSchemaSequence)
			{
				WriteObject_XmlSchemaSequence((XmlSchemaSequence)ob.Particle, "sequence", "http://www.w3.org/2001/XMLSchema", false, false, true);
			}
			else if (ob.Particle is XmlSchemaChoice)
			{
				WriteObject_XmlSchemaChoice((XmlSchemaChoice)ob.Particle, "choice", "http://www.w3.org/2001/XMLSchema", false, false, true);
			}
			if (ob.Attributes != null)
			{
				for (int i = 0; i < ob.Attributes.Count; i++)
				{
					if (ob.Attributes[i] == null)
					{
						continue;
					}
					if (ob.Attributes[i].GetType() == typeof(XmlSchemaAttributeGroupRef))
					{
						WriteObject_XmlSchemaAttributeGroupRef((XmlSchemaAttributeGroupRef)ob.Attributes[i], "attributeGroup", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Attributes[i].GetType() == typeof(XmlSchemaAttribute))
					{
						WriteObject_XmlSchemaAttribute((XmlSchemaAttribute)ob.Attributes[i], "attribute", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					throw CreateUnknownTypeException(ob.Attributes[i]);
				}
			}
			WriteObject_XmlSchemaAnyAttribute(ob.AnyAttribute, "anyAttribute", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaNotation(XmlSchemaNotation ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaNotation))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaNotation", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("name", string.Empty, ob.Name);
			WriteAttribute("public", string.Empty, ob.Public);
			WriteAttribute("system", string.Empty, ob.System);
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaKeyref(XmlSchemaKeyref ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaKeyref))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaKeyref", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("name", string.Empty, ob.Name);
			WriteAttribute("refer", string.Empty, FromXmlQualifiedName(ob.Refer));
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			WriteObject_XmlSchemaXPath(ob.Selector, "selector", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (ob.Fields != null)
			{
				for (int i = 0; i < ob.Fields.Count; i++)
				{
					WriteObject_XmlSchemaXPath((XmlSchemaXPath)ob.Fields[i], "field", "http://www.w3.org/2001/XMLSchema", false, false, true);
				}
			}
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaKey(XmlSchemaKey ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaKey))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaKey", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("name", string.Empty, ob.Name);
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			WriteObject_XmlSchemaXPath(ob.Selector, "selector", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (ob.Fields != null)
			{
				for (int i = 0; i < ob.Fields.Count; i++)
				{
					WriteObject_XmlSchemaXPath((XmlSchemaXPath)ob.Fields[i], "field", "http://www.w3.org/2001/XMLSchema", false, false, true);
				}
			}
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaUnique(XmlSchemaUnique ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaUnique))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaUnique", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("name", string.Empty, ob.Name);
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			WriteObject_XmlSchemaXPath(ob.Selector, "selector", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (ob.Fields != null)
			{
				for (int i = 0; i < ob.Fields.Count; i++)
				{
					WriteObject_XmlSchemaXPath((XmlSchemaXPath)ob.Fields[i], "field", "http://www.w3.org/2001/XMLSchema", false, false, true);
				}
			}
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaSimpleTypeUnion(XmlSchemaSimpleTypeUnion ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaSimpleTypeUnion))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaSimpleTypeUnion", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			string value = null;
			if (ob.MemberTypes != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < ob.MemberTypes.Length; i++)
				{
					stringBuilder.Append(FromXmlQualifiedName(ob.MemberTypes[i])).Append(" ");
				}
				value = stringBuilder.ToString().Trim();
			}
			WriteAttribute("memberTypes", string.Empty, value);
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (ob.BaseTypes != null)
			{
				for (int j = 0; j < ob.BaseTypes.Count; j++)
				{
					WriteObject_XmlSchemaSimpleType((XmlSchemaSimpleType)ob.BaseTypes[j], "simpleType", "http://www.w3.org/2001/XMLSchema", false, false, true);
				}
			}
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaSimpleTypeList(XmlSchemaSimpleTypeList ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaSimpleTypeList))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaSimpleTypeList", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("itemType", string.Empty, FromXmlQualifiedName(ob.ItemTypeName));
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			WriteObject_XmlSchemaSimpleType(ob.ItemType, "simpleType", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaSimpleTypeRestriction(XmlSchemaSimpleTypeRestriction ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaSimpleTypeRestriction))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaSimpleTypeRestriction", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("base", string.Empty, FromXmlQualifiedName(ob.BaseTypeName));
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			WriteObject_XmlSchemaSimpleType(ob.BaseType, "simpleType", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (ob.Facets != null)
			{
				for (int i = 0; i < ob.Facets.Count; i++)
				{
					if (ob.Facets[i] == null)
					{
						continue;
					}
					if (ob.Facets[i].GetType() == typeof(XmlSchemaMaxLengthFacet))
					{
						WriteObject_XmlSchemaMaxLengthFacet((XmlSchemaMaxLengthFacet)ob.Facets[i], "maxLength", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Facets[i].GetType() == typeof(XmlSchemaMinLengthFacet))
					{
						WriteObject_XmlSchemaMinLengthFacet((XmlSchemaMinLengthFacet)ob.Facets[i], "minLength", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Facets[i].GetType() == typeof(XmlSchemaLengthFacet))
					{
						WriteObject_XmlSchemaLengthFacet((XmlSchemaLengthFacet)ob.Facets[i], "length", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Facets[i].GetType() == typeof(XmlSchemaFractionDigitsFacet))
					{
						WriteObject_XmlSchemaFractionDigitsFacet((XmlSchemaFractionDigitsFacet)ob.Facets[i], "fractionDigits", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Facets[i].GetType() == typeof(XmlSchemaMaxInclusiveFacet))
					{
						WriteObject_XmlSchemaMaxInclusiveFacet((XmlSchemaMaxInclusiveFacet)ob.Facets[i], "maxInclusive", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Facets[i].GetType() == typeof(XmlSchemaMaxExclusiveFacet))
					{
						WriteObject_XmlSchemaMaxExclusiveFacet((XmlSchemaMaxExclusiveFacet)ob.Facets[i], "maxExclusive", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Facets[i].GetType() == typeof(XmlSchemaMinExclusiveFacet))
					{
						WriteObject_XmlSchemaMinExclusiveFacet((XmlSchemaMinExclusiveFacet)ob.Facets[i], "minExclusive", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Facets[i].GetType() == typeof(XmlSchemaEnumerationFacet))
					{
						WriteObject_XmlSchemaEnumerationFacet((XmlSchemaEnumerationFacet)ob.Facets[i], "enumeration", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Facets[i].GetType() == typeof(XmlSchemaTotalDigitsFacet))
					{
						WriteObject_XmlSchemaTotalDigitsFacet((XmlSchemaTotalDigitsFacet)ob.Facets[i], "totalDigits", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Facets[i].GetType() == typeof(XmlSchemaMinInclusiveFacet))
					{
						WriteObject_XmlSchemaMinInclusiveFacet((XmlSchemaMinInclusiveFacet)ob.Facets[i], "minInclusive", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Facets[i].GetType() == typeof(XmlSchemaWhiteSpaceFacet))
					{
						WriteObject_XmlSchemaWhiteSpaceFacet((XmlSchemaWhiteSpaceFacet)ob.Facets[i], "whiteSpace", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Facets[i].GetType() == typeof(XmlSchemaPatternFacet))
					{
						WriteObject_XmlSchemaPatternFacet((XmlSchemaPatternFacet)ob.Facets[i], "pattern", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					throw CreateUnknownTypeException(ob.Facets[i]);
				}
			}
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private string GetEnumValue_XmlSchemaUse(XmlSchemaUse val)
		{
			switch (val)
			{
			case XmlSchemaUse.Optional:
				return "optional";
			case XmlSchemaUse.Prohibited:
				return "prohibited";
			case XmlSchemaUse.Required:
				return "required";
			default:
				return ((long)val).ToString(CultureInfo.InvariantCulture);
			}
		}

		private void WriteObject_XmlSchemaAppInfo(XmlSchemaAppInfo ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaAppInfo))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaAppInfo", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			WriteAttribute("source", string.Empty, ob.Source);
			if (ob.Markup != null)
			{
				XmlNode[] markup = ob.Markup;
				foreach (XmlNode xmlNode in markup)
				{
					XmlNode xmlNode2 = xmlNode;
					if (xmlNode2 is XmlElement)
					{
						WriteElementLiteral(xmlNode2, string.Empty, string.Empty, false, true);
					}
					else
					{
						xmlNode2.WriteTo(base.Writer);
					}
				}
			}
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaDocumentation(XmlSchemaDocumentation ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaDocumentation))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaDocumentation", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			WriteAttribute("source", string.Empty, ob.Source);
			WriteAttribute("xml:lang", string.Empty, ob.Language);
			if (ob.Markup != null)
			{
				XmlNode[] markup = ob.Markup;
				foreach (XmlNode xmlNode in markup)
				{
					XmlNode xmlNode2 = xmlNode;
					if (xmlNode2 is XmlElement)
					{
						WriteElementLiteral(xmlNode2, string.Empty, string.Empty, false, true);
					}
					else
					{
						xmlNode2.WriteTo(base.Writer);
					}
				}
			}
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaAttributeGroupRef(XmlSchemaAttributeGroupRef ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaAttributeGroupRef))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaAttributeGroupRef", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("ref", string.Empty, FromXmlQualifiedName(ob.RefName));
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaAnyAttribute(XmlSchemaAnyAttribute ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaAnyAttribute))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaAnyAttribute", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("namespace", string.Empty, ob.Namespace);
			if (ob.ProcessContents != XmlSchemaContentProcessing.None)
			{
				WriteAttribute("processContents", string.Empty, GetEnumValue_XmlSchemaContentProcessing(ob.ProcessContents));
			}
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaSequence(XmlSchemaSequence ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaSequence))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaSequence", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("minOccurs", string.Empty, ob.MinOccursString);
			WriteAttribute("maxOccurs", string.Empty, ob.MaxOccursString);
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (ob.Items != null)
			{
				for (int i = 0; i < ob.Items.Count; i++)
				{
					if (ob.Items[i] == null)
					{
						continue;
					}
					if (ob.Items[i].GetType() == typeof(XmlSchemaSequence))
					{
						WriteObject_XmlSchemaSequence((XmlSchemaSequence)ob.Items[i], "sequence", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Items[i].GetType() == typeof(XmlSchemaChoice))
					{
						WriteObject_XmlSchemaChoice((XmlSchemaChoice)ob.Items[i], "choice", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Items[i].GetType() == typeof(XmlSchemaGroupRef))
					{
						WriteObject_XmlSchemaGroupRef((XmlSchemaGroupRef)ob.Items[i], "group", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Items[i].GetType() == typeof(XmlSchemaElement))
					{
						WriteObject_XmlSchemaElement((XmlSchemaElement)ob.Items[i], "element", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Items[i].GetType() == typeof(XmlSchemaAny))
					{
						WriteObject_XmlSchemaAny((XmlSchemaAny)ob.Items[i], "any", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					throw CreateUnknownTypeException(ob.Items[i]);
				}
			}
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaChoice(XmlSchemaChoice ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaChoice))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaChoice", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("minOccurs", string.Empty, ob.MinOccursString);
			WriteAttribute("maxOccurs", string.Empty, ob.MaxOccursString);
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (ob.Items != null)
			{
				for (int i = 0; i < ob.Items.Count; i++)
				{
					if (ob.Items[i] == null)
					{
						continue;
					}
					if (ob.Items[i].GetType() == typeof(XmlSchemaGroupRef))
					{
						WriteObject_XmlSchemaGroupRef((XmlSchemaGroupRef)ob.Items[i], "group", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Items[i].GetType() == typeof(XmlSchemaElement))
					{
						WriteObject_XmlSchemaElement((XmlSchemaElement)ob.Items[i], "element", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Items[i].GetType() == typeof(XmlSchemaAny))
					{
						WriteObject_XmlSchemaAny((XmlSchemaAny)ob.Items[i], "any", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Items[i].GetType() == typeof(XmlSchemaSequence))
					{
						WriteObject_XmlSchemaSequence((XmlSchemaSequence)ob.Items[i], "sequence", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Items[i].GetType() == typeof(XmlSchemaChoice))
					{
						WriteObject_XmlSchemaChoice((XmlSchemaChoice)ob.Items[i], "choice", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					throw CreateUnknownTypeException(ob.Items[i]);
				}
			}
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaAll(XmlSchemaAll ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaAll))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaAll", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("minOccurs", string.Empty, ob.MinOccursString);
			WriteAttribute("maxOccurs", string.Empty, ob.MaxOccursString);
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (ob.Items != null)
			{
				for (int i = 0; i < ob.Items.Count; i++)
				{
					WriteObject_XmlSchemaElement((XmlSchemaElement)ob.Items[i], "element", "http://www.w3.org/2001/XMLSchema", false, false, true);
				}
			}
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaComplexContent(XmlSchemaComplexContent ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaComplexContent))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaComplexContent", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("mixed", string.Empty, (!ob.IsMixed) ? "false" : "true");
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (ob.Content is XmlSchemaComplexContentExtension)
			{
				WriteObject_XmlSchemaComplexContentExtension((XmlSchemaComplexContentExtension)ob.Content, "extension", "http://www.w3.org/2001/XMLSchema", false, false, true);
			}
			else if (ob.Content is XmlSchemaComplexContentRestriction)
			{
				WriteObject_XmlSchemaComplexContentRestriction((XmlSchemaComplexContentRestriction)ob.Content, "restriction", "http://www.w3.org/2001/XMLSchema", false, false, true);
			}
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaSimpleContent(XmlSchemaSimpleContent ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaSimpleContent))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaSimpleContent", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (ob.Content is XmlSchemaSimpleContentExtension)
			{
				WriteObject_XmlSchemaSimpleContentExtension((XmlSchemaSimpleContentExtension)ob.Content, "extension", "http://www.w3.org/2001/XMLSchema", false, false, true);
			}
			else if (ob.Content is XmlSchemaSimpleContentRestriction)
			{
				WriteObject_XmlSchemaSimpleContentRestriction((XmlSchemaSimpleContentRestriction)ob.Content, "restriction", "http://www.w3.org/2001/XMLSchema", false, false, true);
			}
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaGroupRef(XmlSchemaGroupRef ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaGroupRef))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaGroupRef", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("minOccurs", string.Empty, ob.MinOccursString);
			WriteAttribute("maxOccurs", string.Empty, ob.MaxOccursString);
			WriteAttribute("ref", string.Empty, FromXmlQualifiedName(ob.RefName));
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaXPath(XmlSchemaXPath ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaXPath))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaXPath", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			if (ob.XPath != null)
			{
				WriteAttribute("xpath", string.Empty, ob.XPath);
			}
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaMaxLengthFacet(XmlSchemaMaxLengthFacet ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaMaxLengthFacet))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaMaxLengthFacet", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("value", string.Empty, ob.Value);
			if (ob.IsFixed)
			{
				WriteAttribute("fixed", string.Empty, (!ob.IsFixed) ? "false" : "true");
			}
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaMinLengthFacet(XmlSchemaMinLengthFacet ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaMinLengthFacet))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaMinLengthFacet", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("value", string.Empty, ob.Value);
			if (ob.IsFixed)
			{
				WriteAttribute("fixed", string.Empty, (!ob.IsFixed) ? "false" : "true");
			}
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaLengthFacet(XmlSchemaLengthFacet ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaLengthFacet))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaLengthFacet", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("value", string.Empty, ob.Value);
			if (ob.IsFixed)
			{
				WriteAttribute("fixed", string.Empty, (!ob.IsFixed) ? "false" : "true");
			}
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaFractionDigitsFacet(XmlSchemaFractionDigitsFacet ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaFractionDigitsFacet))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaFractionDigitsFacet", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("value", string.Empty, ob.Value);
			if (ob.IsFixed)
			{
				WriteAttribute("fixed", string.Empty, (!ob.IsFixed) ? "false" : "true");
			}
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaMaxInclusiveFacet(XmlSchemaMaxInclusiveFacet ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaMaxInclusiveFacet))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaMaxInclusiveFacet", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("value", string.Empty, ob.Value);
			if (ob.IsFixed)
			{
				WriteAttribute("fixed", string.Empty, (!ob.IsFixed) ? "false" : "true");
			}
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaMaxExclusiveFacet(XmlSchemaMaxExclusiveFacet ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaMaxExclusiveFacet))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaMaxExclusiveFacet", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("value", string.Empty, ob.Value);
			if (ob.IsFixed)
			{
				WriteAttribute("fixed", string.Empty, (!ob.IsFixed) ? "false" : "true");
			}
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaMinExclusiveFacet(XmlSchemaMinExclusiveFacet ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaMinExclusiveFacet))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaMinExclusiveFacet", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("value", string.Empty, ob.Value);
			if (ob.IsFixed)
			{
				WriteAttribute("fixed", string.Empty, (!ob.IsFixed) ? "false" : "true");
			}
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaEnumerationFacet(XmlSchemaEnumerationFacet ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaEnumerationFacet))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaEnumerationFacet", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("value", string.Empty, ob.Value);
			if (ob.IsFixed)
			{
				WriteAttribute("fixed", string.Empty, (!ob.IsFixed) ? "false" : "true");
			}
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaTotalDigitsFacet(XmlSchemaTotalDigitsFacet ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaTotalDigitsFacet))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaTotalDigitsFacet", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("value", string.Empty, ob.Value);
			if (ob.IsFixed)
			{
				WriteAttribute("fixed", string.Empty, (!ob.IsFixed) ? "false" : "true");
			}
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaMinInclusiveFacet(XmlSchemaMinInclusiveFacet ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaMinInclusiveFacet))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaMinInclusiveFacet", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("value", string.Empty, ob.Value);
			if (ob.IsFixed)
			{
				WriteAttribute("fixed", string.Empty, (!ob.IsFixed) ? "false" : "true");
			}
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaWhiteSpaceFacet(XmlSchemaWhiteSpaceFacet ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaWhiteSpaceFacet))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaWhiteSpaceFacet", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("value", string.Empty, ob.Value);
			if (ob.IsFixed)
			{
				WriteAttribute("fixed", string.Empty, (!ob.IsFixed) ? "false" : "true");
			}
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaPatternFacet(XmlSchemaPatternFacet ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaPatternFacet))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaPatternFacet", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("value", string.Empty, ob.Value);
			if (ob.IsFixed)
			{
				WriteAttribute("fixed", string.Empty, (!ob.IsFixed) ? "false" : "true");
			}
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private string GetEnumValue_XmlSchemaContentProcessing(XmlSchemaContentProcessing val)
		{
			switch (val)
			{
			case XmlSchemaContentProcessing.Skip:
				return "skip";
			case XmlSchemaContentProcessing.Lax:
				return "lax";
			case XmlSchemaContentProcessing.Strict:
				return "strict";
			default:
				return ((long)val).ToString(CultureInfo.InvariantCulture);
			}
		}

		private void WriteObject_XmlSchemaAny(XmlSchemaAny ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaAny))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaAny", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("minOccurs", string.Empty, ob.MinOccursString);
			WriteAttribute("maxOccurs", string.Empty, ob.MaxOccursString);
			WriteAttribute("namespace", string.Empty, ob.Namespace);
			if (ob.ProcessContents != XmlSchemaContentProcessing.None)
			{
				WriteAttribute("processContents", string.Empty, GetEnumValue_XmlSchemaContentProcessing(ob.ProcessContents));
			}
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaComplexContentExtension(XmlSchemaComplexContentExtension ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaComplexContentExtension))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaComplexContentExtension", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("base", string.Empty, FromXmlQualifiedName(ob.BaseTypeName));
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (ob.Particle is XmlSchemaGroupRef)
			{
				WriteObject_XmlSchemaGroupRef((XmlSchemaGroupRef)ob.Particle, "group", "http://www.w3.org/2001/XMLSchema", false, false, true);
			}
			else if (ob.Particle is XmlSchemaSequence)
			{
				WriteObject_XmlSchemaSequence((XmlSchemaSequence)ob.Particle, "sequence", "http://www.w3.org/2001/XMLSchema", false, false, true);
			}
			else if (ob.Particle is XmlSchemaChoice)
			{
				WriteObject_XmlSchemaChoice((XmlSchemaChoice)ob.Particle, "choice", "http://www.w3.org/2001/XMLSchema", false, false, true);
			}
			else if (ob.Particle is XmlSchemaAll)
			{
				WriteObject_XmlSchemaAll((XmlSchemaAll)ob.Particle, "all", "http://www.w3.org/2001/XMLSchema", false, false, true);
			}
			if (ob.Attributes != null)
			{
				for (int i = 0; i < ob.Attributes.Count; i++)
				{
					if (ob.Attributes[i] == null)
					{
						continue;
					}
					if (ob.Attributes[i].GetType() == typeof(XmlSchemaAttribute))
					{
						WriteObject_XmlSchemaAttribute((XmlSchemaAttribute)ob.Attributes[i], "attribute", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Attributes[i].GetType() == typeof(XmlSchemaAttributeGroupRef))
					{
						WriteObject_XmlSchemaAttributeGroupRef((XmlSchemaAttributeGroupRef)ob.Attributes[i], "attributeGroup", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					throw CreateUnknownTypeException(ob.Attributes[i]);
				}
			}
			WriteObject_XmlSchemaAnyAttribute(ob.AnyAttribute, "anyAttribute", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaComplexContentRestriction(XmlSchemaComplexContentRestriction ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaComplexContentRestriction))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaComplexContentRestriction", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("base", string.Empty, FromXmlQualifiedName(ob.BaseTypeName));
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (ob.Particle is XmlSchemaSequence)
			{
				WriteObject_XmlSchemaSequence((XmlSchemaSequence)ob.Particle, "sequence", "http://www.w3.org/2001/XMLSchema", false, false, true);
			}
			else if (ob.Particle is XmlSchemaChoice)
			{
				WriteObject_XmlSchemaChoice((XmlSchemaChoice)ob.Particle, "choice", "http://www.w3.org/2001/XMLSchema", false, false, true);
			}
			else if (ob.Particle is XmlSchemaGroupRef)
			{
				WriteObject_XmlSchemaGroupRef((XmlSchemaGroupRef)ob.Particle, "group", "http://www.w3.org/2001/XMLSchema", false, false, true);
			}
			else if (ob.Particle is XmlSchemaAll)
			{
				WriteObject_XmlSchemaAll((XmlSchemaAll)ob.Particle, "all", "http://www.w3.org/2001/XMLSchema", false, false, true);
			}
			if (ob.Attributes != null)
			{
				for (int i = 0; i < ob.Attributes.Count; i++)
				{
					if (ob.Attributes[i] == null)
					{
						continue;
					}
					if (ob.Attributes[i].GetType() == typeof(XmlSchemaAttributeGroupRef))
					{
						WriteObject_XmlSchemaAttributeGroupRef((XmlSchemaAttributeGroupRef)ob.Attributes[i], "attributeGroup", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Attributes[i].GetType() == typeof(XmlSchemaAttribute))
					{
						WriteObject_XmlSchemaAttribute((XmlSchemaAttribute)ob.Attributes[i], "attribute", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					throw CreateUnknownTypeException(ob.Attributes[i]);
				}
			}
			WriteObject_XmlSchemaAnyAttribute(ob.AnyAttribute, "anyAttribute", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaSimpleContentExtension(XmlSchemaSimpleContentExtension ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaSimpleContentExtension))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaSimpleContentExtension", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("base", string.Empty, FromXmlQualifiedName(ob.BaseTypeName));
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (ob.Attributes != null)
			{
				for (int i = 0; i < ob.Attributes.Count; i++)
				{
					if (ob.Attributes[i] == null)
					{
						continue;
					}
					if (ob.Attributes[i].GetType() == typeof(XmlSchemaAttributeGroupRef))
					{
						WriteObject_XmlSchemaAttributeGroupRef((XmlSchemaAttributeGroupRef)ob.Attributes[i], "attributeGroup", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Attributes[i].GetType() == typeof(XmlSchemaAttribute))
					{
						WriteObject_XmlSchemaAttribute((XmlSchemaAttribute)ob.Attributes[i], "attribute", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					throw CreateUnknownTypeException(ob.Attributes[i]);
				}
			}
			WriteObject_XmlSchemaAnyAttribute(ob.AnyAttribute, "anyAttribute", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		private void WriteObject_XmlSchemaSimpleContentRestriction(XmlSchemaSimpleContentRestriction ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)
		{
			if (ob == null)
			{
				if (isNullable)
				{
					WriteNullTagLiteral(element, namesp);
				}
				return;
			}
			Type type = ob.GetType();
			if (type != typeof(XmlSchemaSimpleContentRestriction))
			{
				throw CreateUnknownTypeException(ob);
			}
			if (writeWrappingElem)
			{
				WriteStartElement(element, namesp, ob);
			}
			if (needType)
			{
				WriteXsiType("XmlSchemaSimpleContentRestriction", "http://www.w3.org/2001/XMLSchema");
			}
			WriteNamespaceDeclarations(ob.Namespaces);
			ICollection unhandledAttributes = ob.UnhandledAttributes;
			if (unhandledAttributes != null)
			{
				foreach (XmlAttribute item in unhandledAttributes)
				{
					if (item.NamespaceURI != "http://www.w3.org/2000/xmlns/")
					{
						WriteXmlAttribute(item, ob);
					}
				}
			}
			WriteAttribute("id", string.Empty, ob.Id);
			WriteAttribute("base", string.Empty, FromXmlQualifiedName(ob.BaseTypeName));
			WriteObject_XmlSchemaAnnotation(ob.Annotation, "annotation", "http://www.w3.org/2001/XMLSchema", false, false, true);
			WriteObject_XmlSchemaSimpleType(ob.BaseType, "simpleType", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (ob.Facets != null)
			{
				for (int i = 0; i < ob.Facets.Count; i++)
				{
					if (ob.Facets[i] == null)
					{
						continue;
					}
					if (ob.Facets[i].GetType() == typeof(XmlSchemaEnumerationFacet))
					{
						WriteObject_XmlSchemaEnumerationFacet((XmlSchemaEnumerationFacet)ob.Facets[i], "enumeration", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Facets[i].GetType() == typeof(XmlSchemaMaxLengthFacet))
					{
						WriteObject_XmlSchemaMaxLengthFacet((XmlSchemaMaxLengthFacet)ob.Facets[i], "maxLength", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Facets[i].GetType() == typeof(XmlSchemaMinLengthFacet))
					{
						WriteObject_XmlSchemaMinLengthFacet((XmlSchemaMinLengthFacet)ob.Facets[i], "minLength", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Facets[i].GetType() == typeof(XmlSchemaLengthFacet))
					{
						WriteObject_XmlSchemaLengthFacet((XmlSchemaLengthFacet)ob.Facets[i], "length", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Facets[i].GetType() == typeof(XmlSchemaFractionDigitsFacet))
					{
						WriteObject_XmlSchemaFractionDigitsFacet((XmlSchemaFractionDigitsFacet)ob.Facets[i], "fractionDigits", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Facets[i].GetType() == typeof(XmlSchemaTotalDigitsFacet))
					{
						WriteObject_XmlSchemaTotalDigitsFacet((XmlSchemaTotalDigitsFacet)ob.Facets[i], "totalDigits", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Facets[i].GetType() == typeof(XmlSchemaMaxInclusiveFacet))
					{
						WriteObject_XmlSchemaMaxInclusiveFacet((XmlSchemaMaxInclusiveFacet)ob.Facets[i], "maxInclusive", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Facets[i].GetType() == typeof(XmlSchemaMaxExclusiveFacet))
					{
						WriteObject_XmlSchemaMaxExclusiveFacet((XmlSchemaMaxExclusiveFacet)ob.Facets[i], "maxExclusive", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Facets[i].GetType() == typeof(XmlSchemaMinInclusiveFacet))
					{
						WriteObject_XmlSchemaMinInclusiveFacet((XmlSchemaMinInclusiveFacet)ob.Facets[i], "minInclusive", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Facets[i].GetType() == typeof(XmlSchemaMinExclusiveFacet))
					{
						WriteObject_XmlSchemaMinExclusiveFacet((XmlSchemaMinExclusiveFacet)ob.Facets[i], "minExclusive", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Facets[i].GetType() == typeof(XmlSchemaWhiteSpaceFacet))
					{
						WriteObject_XmlSchemaWhiteSpaceFacet((XmlSchemaWhiteSpaceFacet)ob.Facets[i], "whiteSpace", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Facets[i].GetType() == typeof(XmlSchemaPatternFacet))
					{
						WriteObject_XmlSchemaPatternFacet((XmlSchemaPatternFacet)ob.Facets[i], "pattern", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					throw CreateUnknownTypeException(ob.Facets[i]);
				}
			}
			if (ob.Attributes != null)
			{
				for (int j = 0; j < ob.Attributes.Count; j++)
				{
					if (ob.Attributes[j] == null)
					{
						continue;
					}
					if (ob.Attributes[j].GetType() == typeof(XmlSchemaAttributeGroupRef))
					{
						WriteObject_XmlSchemaAttributeGroupRef((XmlSchemaAttributeGroupRef)ob.Attributes[j], "attributeGroup", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					if (ob.Attributes[j].GetType() == typeof(XmlSchemaAttribute))
					{
						WriteObject_XmlSchemaAttribute((XmlSchemaAttribute)ob.Attributes[j], "attribute", "http://www.w3.org/2001/XMLSchema", false, false, true);
						continue;
					}
					throw CreateUnknownTypeException(ob.Attributes[j]);
				}
			}
			WriteObject_XmlSchemaAnyAttribute(ob.AnyAttribute, "anyAttribute", "http://www.w3.org/2001/XMLSchema", false, false, true);
			if (writeWrappingElem)
			{
				WriteEndElement(ob);
			}
		}

		protected override void InitCallbacks()
		{
		}
	}
}
