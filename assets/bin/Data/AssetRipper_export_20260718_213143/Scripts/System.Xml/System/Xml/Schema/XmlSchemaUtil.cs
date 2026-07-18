using System.Collections;
using System.Text;
using System.Xml.Serialization;
using Mono.Xml;
using Mono.Xml.Schema;

namespace System.Xml.Schema
{
	internal class XmlSchemaUtil
	{
		internal static XmlSchemaDerivationMethod FinalAllowed;

		internal static XmlSchemaDerivationMethod ElementBlockAllowed;

		internal static XmlSchemaDerivationMethod ComplexTypeBlockAllowed;

		internal static readonly bool StrictMsCompliant;

		static XmlSchemaUtil()
		{
			StrictMsCompliant = Environment.GetEnvironmentVariable("MONO_STRICT_MS_COMPLIANT") == "yes";
			FinalAllowed = XmlSchemaDerivationMethod.Extension | XmlSchemaDerivationMethod.Restriction;
			ComplexTypeBlockAllowed = FinalAllowed;
			ElementBlockAllowed = XmlSchemaDerivationMethod.Substitution | FinalAllowed;
		}

		public static void AddToTable(XmlSchemaObjectTable table, XmlSchemaObject obj, XmlQualifiedName qname, ValidationEventHandler h)
		{
			if (table.Contains(qname))
			{
				if (obj.isRedefineChild)
				{
					if (obj.redefinedObject != null)
					{
						obj.error(h, string.Format("Named item {0} was already contained in the schema object table.", qname));
					}
					else
					{
						obj.redefinedObject = table[qname];
					}
					table.Set(qname, obj);
				}
				else if (table[qname].isRedefineChild)
				{
					if (table[qname].redefinedObject != null)
					{
						obj.error(h, string.Format("Named item {0} was already contained in the schema object table.", qname));
					}
					else
					{
						table[qname].redefinedObject = obj;
					}
				}
				else if (StrictMsCompliant)
				{
					table.Set(qname, obj);
				}
				else
				{
					obj.error(h, string.Format("Named item {0} was already contained in the schema object table. {1}", qname, "Consider setting MONO_STRICT_MS_COMPLIANT to 'yes' to mimic MS implementation."));
				}
			}
			else
			{
				table.Set(qname, obj);
			}
		}

		public static void CompileID(string id, XmlSchemaObject xso, Hashtable idCollection, ValidationEventHandler h)
		{
			if (id != null)
			{
				if (!CheckNCName(id))
				{
					xso.error(h, id + " is not a valid id attribute");
				}
				else if (idCollection.ContainsKey(id))
				{
					xso.error(h, "Duplicate id attribute " + id);
				}
				else
				{
					idCollection.Add(id, xso);
				}
			}
		}

		public static bool CheckAnyUri(string uri)
		{
			if (uri.StartsWith("##"))
			{
				return false;
			}
			return true;
		}

		public static bool CheckNormalizedString(string token)
		{
			return true;
		}

		public static bool CheckNCName(string name)
		{
			return XmlChar.IsNCName(name);
		}

		public static bool CheckQName(XmlQualifiedName qname)
		{
			return true;
		}

		public static XmlParserContext GetParserContext(XmlReader reader)
		{
			IHasXmlParserContext hasXmlParserContext = reader as IHasXmlParserContext;
			if (hasXmlParserContext != null)
			{
				return hasXmlParserContext.ParserContext;
			}
			return null;
		}

		public static bool IsBuiltInDatatypeName(XmlQualifiedName qname)
		{
			if (qname.Namespace == "http://www.w3.org/2003/11/xpath-datatypes")
			{
				switch (qname.Name)
				{
				case "anyAtomicType":
				case "untypedAtomic":
				case "dayTimeDuration":
				case "yearMonthDuration":
					return true;
				default:
					return false;
				}
			}
			if (qname.Namespace != "http://www.w3.org/2001/XMLSchema")
			{
				return false;
			}
			switch (qname.Name)
			{
			case "anySimpleType":
			case "duration":
			case "dateTime":
			case "time":
			case "date":
			case "gYearMonth":
			case "gYear":
			case "gMonthDay":
			case "gDay":
			case "gMonth":
			case "boolean":
			case "base64Binary":
			case "hexBinary":
			case "float":
			case "double":
			case "anyURI":
			case "QName":
			case "NOTATION":
			case "string":
			case "normalizedString":
			case "token":
			case "language":
			case "Name":
			case "NCName":
			case "ID":
			case "IDREF":
			case "IDREFS":
			case "ENTITY":
			case "ENTITIES":
			case "NMTOKEN":
			case "NMTOKENS":
			case "decimal":
			case "integer":
			case "nonPositiveInteger":
			case "negativeInteger":
			case "nonNegativeInteger":
			case "unsignedLong":
			case "unsignedInt":
			case "unsignedShort":
			case "unsignedByte":
			case "positiveInteger":
			case "long":
			case "int":
			case "short":
			case "byte":
				return true;
			default:
				return false;
			}
		}

		public static bool AreSchemaDatatypeEqual(XmlSchemaSimpleType st1, object v1, XmlSchemaSimpleType st2, object v2)
		{
			if (st1.Datatype is XsdAnySimpleType)
			{
				return AreSchemaDatatypeEqual(st1.Datatype as XsdAnySimpleType, v1, st2.Datatype as XsdAnySimpleType, v2);
			}
			string[] array = v1 as string[];
			string[] array2 = v2 as string[];
			if (st1 != st2 || array == null || array2 == null || array.Length != array2.Length)
			{
				return false;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != array2[i])
				{
					return false;
				}
			}
			return true;
		}

		public static bool AreSchemaDatatypeEqual(XsdAnySimpleType st1, object v1, XsdAnySimpleType st2, object v2)
		{
			if (v1 == null || v2 == null)
			{
				return false;
			}
			if (st1 == null)
			{
				st1 = XmlSchemaSimpleType.AnySimpleType;
			}
			if (st2 == null)
			{
				st2 = XmlSchemaSimpleType.AnySimpleType;
			}
			Type type = st2.GetType();
			if (st1 is XsdFloat)
			{
				return st2 is XsdFloat && Convert.ToSingle(v1) == Convert.ToSingle(v2);
			}
			if (st1 is XsdDouble)
			{
				return st2 is XsdDouble && Convert.ToDouble(v1) == Convert.ToDouble(v2);
			}
			if (st1 is XsdDecimal)
			{
				if (!(st2 is XsdDecimal) || Convert.ToDecimal(v1) != Convert.ToDecimal(v2))
				{
					return false;
				}
				if (st1 is XsdNonPositiveInteger)
				{
					return st2 is XsdNonPositiveInteger || type == typeof(XsdDecimal) || type == typeof(XsdInteger);
				}
				if (st1 is XsdPositiveInteger)
				{
					return st2 is XsdPositiveInteger || type == typeof(XsdDecimal) || type == typeof(XsdInteger) || type == typeof(XsdNonNegativeInteger);
				}
				if (st1 is XsdUnsignedLong)
				{
					return st2 is XsdUnsignedLong || type == typeof(XsdDecimal) || type == typeof(XsdInteger) || type == typeof(XsdNonNegativeInteger);
				}
				if (st1 is XsdNonNegativeInteger)
				{
					return st2 is XsdNonNegativeInteger || type == typeof(XsdDecimal) || type == typeof(XsdInteger);
				}
				if (st1 is XsdLong)
				{
					return st2 is XsdLong || type == typeof(XsdDecimal) || type == typeof(XsdInteger);
				}
				return true;
			}
			if (!v1.Equals(v2))
			{
				return false;
			}
			if (st1 is XsdString)
			{
				if (!(st2 is XsdString))
				{
					return false;
				}
				if (st1 is XsdNMToken && (st2 is XsdLanguage || st2 is XsdName))
				{
					return false;
				}
				if (st2 is XsdNMToken && (st1 is XsdLanguage || st1 is XsdName))
				{
					return false;
				}
				if (st1 is XsdName && (st2 is XsdLanguage || st2 is XsdNMToken))
				{
					return false;
				}
				if (st2 is XsdName && (st1 is XsdLanguage || st1 is XsdNMToken))
				{
					return false;
				}
				if (st1 is XsdID && st2 is XsdIDRef)
				{
					return false;
				}
				if (st1 is XsdIDRef && st2 is XsdID)
				{
					return false;
				}
			}
			else if (st1 != st2)
			{
				return false;
			}
			return true;
		}

		public static bool IsValidQName(string qname)
		{
			string[] array = qname.Split(new char[1] { ':' }, 2);
			foreach (string name in array)
			{
				if (!CheckNCName(name))
				{
					return false;
				}
			}
			return true;
		}

		public static string[] SplitList(string list)
		{
			if (list == null || list == string.Empty)
			{
				return new string[0];
			}
			ArrayList arrayList = null;
			int num = 0;
			bool flag = true;
			for (int i = 0; i < list.Length; i++)
			{
				switch (list[i])
				{
				case '\t':
				case '\n':
				case '\r':
				case ' ':
					if (!flag)
					{
						if (arrayList == null)
						{
							arrayList = new ArrayList();
						}
						arrayList.Add(list.Substring(num, i - num));
					}
					flag = true;
					break;
				default:
					if (flag)
					{
						flag = false;
						num = i;
					}
					break;
				}
			}
			if (!flag && num == 0)
			{
				return new string[1] { list };
			}
			if (!flag && num < list.Length)
			{
				arrayList.Add((num != 0) ? list.Substring(num) : list);
			}
			return arrayList.ToArray(typeof(string)) as string[];
		}

		public static void ReadUnhandledAttribute(XmlReader reader, XmlSchemaObject xso)
		{
			if (reader.Prefix == "xmlns")
			{
				xso.Namespaces.Add(reader.LocalName, reader.Value);
				return;
			}
			if (reader.Name == "xmlns")
			{
				xso.Namespaces.Add(string.Empty, reader.Value);
				return;
			}
			if (xso.unhandledAttributeList == null)
			{
				xso.unhandledAttributeList = new ArrayList();
			}
			XmlAttribute xmlAttribute = new XmlDocument().CreateAttribute(reader.LocalName, reader.NamespaceURI);
			xmlAttribute.Value = reader.Value;
			ParseWsdlArrayType(reader, xmlAttribute);
			xso.unhandledAttributeList.Add(xmlAttribute);
		}

		private static void ParseWsdlArrayType(XmlReader reader, XmlAttribute attr)
		{
			if (attr.NamespaceURI == "http://schemas.xmlsoap.org/wsdl/" && attr.LocalName == "arrayType")
			{
				string ns = string.Empty;
				string type;
				string dimensions;
				TypeTranslator.ParseArrayType(attr.Value, out type, out ns, out dimensions);
				if (ns != string.Empty)
				{
					ns = reader.LookupNamespace(ns) + ":";
				}
				attr.Value = ns + type + dimensions;
			}
		}

		public static bool ReadBoolAttribute(XmlReader reader, out Exception innerExcpetion)
		{
			innerExcpetion = null;
			try
			{
				return XmlConvert.ToBoolean(reader.Value);
			}
			catch (Exception ex)
			{
				innerExcpetion = ex;
				return false;
			}
		}

		public static decimal ReadDecimalAttribute(XmlReader reader, out Exception innerExcpetion)
		{
			innerExcpetion = null;
			try
			{
				return XmlConvert.ToDecimal(reader.Value);
			}
			catch (Exception ex)
			{
				innerExcpetion = ex;
				return 0m;
			}
		}

		public static XmlSchemaDerivationMethod ReadDerivationAttribute(XmlReader reader, out Exception innerExcpetion, string name, XmlSchemaDerivationMethod allowed)
		{
			innerExcpetion = null;
			try
			{
				string value = reader.Value;
				string text = string.Empty;
				XmlSchemaDerivationMethod xmlSchemaDerivationMethod = XmlSchemaDerivationMethod.Empty;
				if (value.IndexOf("#all") != -1 && value.Trim() != "#all")
				{
					innerExcpetion = new Exception(value + " is not a valid value for " + name + ". #all if present must be the only value");
					return XmlSchemaDerivationMethod.All;
				}
				string[] array = SplitList(value);
				foreach (string text2 in array)
				{
					switch (text2)
					{
					case "":
						xmlSchemaDerivationMethod = AddFlag(xmlSchemaDerivationMethod, XmlSchemaDerivationMethod.Empty, allowed);
						break;
					case "#all":
						xmlSchemaDerivationMethod = AddFlag(xmlSchemaDerivationMethod, XmlSchemaDerivationMethod.All, allowed);
						break;
					case "substitution":
						xmlSchemaDerivationMethod = AddFlag(xmlSchemaDerivationMethod, XmlSchemaDerivationMethod.Substitution, allowed);
						break;
					case "extension":
						xmlSchemaDerivationMethod = AddFlag(xmlSchemaDerivationMethod, XmlSchemaDerivationMethod.Extension, allowed);
						break;
					case "restriction":
						xmlSchemaDerivationMethod = AddFlag(xmlSchemaDerivationMethod, XmlSchemaDerivationMethod.Restriction, allowed);
						break;
					case "list":
						xmlSchemaDerivationMethod = AddFlag(xmlSchemaDerivationMethod, XmlSchemaDerivationMethod.List, allowed);
						break;
					case "union":
						xmlSchemaDerivationMethod = AddFlag(xmlSchemaDerivationMethod, XmlSchemaDerivationMethod.Union, allowed);
						break;
					default:
						text = text + text2 + " ";
						break;
					}
				}
				if (text != string.Empty)
				{
					innerExcpetion = new Exception(text + "is/are not valid values for " + name);
				}
				return xmlSchemaDerivationMethod;
			}
			catch (Exception ex)
			{
				innerExcpetion = ex;
				return XmlSchemaDerivationMethod.None;
			}
		}

		private static XmlSchemaDerivationMethod AddFlag(XmlSchemaDerivationMethod dst, XmlSchemaDerivationMethod add, XmlSchemaDerivationMethod allowed)
		{
			if ((add & allowed) == 0 && allowed != XmlSchemaDerivationMethod.All)
			{
				throw new ArgumentException(string.Concat(add, " is not allowed in this attribute."));
			}
			if ((dst & add) != XmlSchemaDerivationMethod.Empty)
			{
				throw new ArgumentException(string.Concat(add, " is already specified in this attribute."));
			}
			return dst | add;
		}

		public static XmlSchemaForm ReadFormAttribute(XmlReader reader, out Exception innerExcpetion)
		{
			innerExcpetion = null;
			XmlSchemaForm result = XmlSchemaForm.None;
			switch (reader.Value)
			{
			default:
			{
				int num;
				if (num == 1)
				{
					result = XmlSchemaForm.Unqualified;
				}
				else
				{
					innerExcpetion = new Exception("only qualified or unqulified is a valid value");
				}
				break;
			}
			case "qualified":
				result = XmlSchemaForm.Qualified;
				break;
			}
			return result;
		}

		public static XmlSchemaContentProcessing ReadProcessingAttribute(XmlReader reader, out Exception innerExcpetion)
		{
			innerExcpetion = null;
			XmlSchemaContentProcessing result = XmlSchemaContentProcessing.None;
			switch (reader.Value)
			{
			case "lax":
				result = XmlSchemaContentProcessing.Lax;
				break;
			case "strict":
				result = XmlSchemaContentProcessing.Strict;
				break;
			case "skip":
				result = XmlSchemaContentProcessing.Skip;
				break;
			default:
				innerExcpetion = new Exception("only lax , strict or skip are valid values for processContents");
				break;
			}
			return result;
		}

		public static XmlSchemaUse ReadUseAttribute(XmlReader reader, out Exception innerExcpetion)
		{
			innerExcpetion = null;
			XmlSchemaUse result = XmlSchemaUse.None;
			switch (reader.Value)
			{
			case "optional":
				result = XmlSchemaUse.Optional;
				break;
			case "prohibited":
				result = XmlSchemaUse.Prohibited;
				break;
			case "required":
				result = XmlSchemaUse.Required;
				break;
			default:
				innerExcpetion = new Exception("only optional , prohibited or required are valid values for use");
				break;
			}
			return result;
		}

		public static XmlQualifiedName ReadQNameAttribute(XmlReader reader, out Exception innerEx)
		{
			return ToQName(reader, reader.Value, out innerEx);
		}

		public static XmlQualifiedName ToQName(XmlReader reader, string qnamestr, out Exception innerEx)
		{
			innerEx = null;
			if (!IsValidQName(qnamestr))
			{
				innerEx = new Exception(qnamestr + " is an invalid QName. Either name or namespace is not a NCName");
				return XmlQualifiedName.Empty;
			}
			string[] array = qnamestr.Split(new char[1] { ':' }, 2);
			string text;
			string name;
			if (array.Length == 2)
			{
				text = reader.LookupNamespace(array[0]);
				if (text == null)
				{
					innerEx = new Exception("Namespace Prefix '" + array[0] + "could not be resolved");
					return XmlQualifiedName.Empty;
				}
				name = array[1];
			}
			else
			{
				text = reader.LookupNamespace(string.Empty);
				name = array[0];
			}
			return new XmlQualifiedName(name, text);
		}

		public static int ValidateAttributesResolved(XmlSchemaObjectTable attributesResolved, ValidationEventHandler h, XmlSchema schema, XmlSchemaObjectCollection attributes, XmlSchemaAnyAttribute anyAttribute, ref XmlSchemaAnyAttribute anyAttributeUse, XmlSchemaAttributeGroup redefined, bool skipEquivalent)
		{
			int num = 0;
			if (anyAttribute != null && anyAttributeUse == null)
			{
				anyAttributeUse = anyAttribute;
			}
			ArrayList arrayList = new ArrayList();
			foreach (XmlSchemaObject attribute in attributes)
			{
				XmlSchemaAttributeGroupRef xmlSchemaAttributeGroupRef = attribute as XmlSchemaAttributeGroupRef;
				if (xmlSchemaAttributeGroupRef != null)
				{
					XmlSchemaAttributeGroup xmlSchemaAttributeGroup = null;
					xmlSchemaAttributeGroup = ((redefined == null || !(xmlSchemaAttributeGroupRef.RefName == redefined.QualifiedName)) ? schema.FindAttributeGroup(xmlSchemaAttributeGroupRef.RefName) : redefined);
					if (xmlSchemaAttributeGroup == null)
					{
						if (!schema.missedSubComponents)
						{
							xmlSchemaAttributeGroupRef.error(h, string.Concat("Referenced attribute group ", xmlSchemaAttributeGroupRef.RefName, " was not found in the corresponding schema."));
						}
						continue;
					}
					if (xmlSchemaAttributeGroup.AttributeGroupRecursionCheck)
					{
						xmlSchemaAttributeGroup.error(h, "Attribute group recursion was found: " + xmlSchemaAttributeGroupRef.RefName);
						continue;
					}
					try
					{
						xmlSchemaAttributeGroup.AttributeGroupRecursionCheck = true;
						num += xmlSchemaAttributeGroup.Validate(h, schema);
					}
					finally
					{
						xmlSchemaAttributeGroup.AttributeGroupRecursionCheck = false;
					}
					if (xmlSchemaAttributeGroup.AnyAttributeUse != null && anyAttribute == null)
					{
						anyAttributeUse = xmlSchemaAttributeGroup.AnyAttributeUse;
					}
					foreach (DictionaryEntry attributeUse in xmlSchemaAttributeGroup.AttributeUses)
					{
						XmlSchemaAttribute xmlSchemaAttribute = (XmlSchemaAttribute)attributeUse.Value;
						if (!StrictMsCompliant || xmlSchemaAttribute.Use != XmlSchemaUse.Prohibited)
						{
							if (xmlSchemaAttribute.RefName != null && xmlSchemaAttribute.RefName != XmlQualifiedName.Empty && (!skipEquivalent || !AreAttributesEqual(xmlSchemaAttribute, attributesResolved[xmlSchemaAttribute.RefName] as XmlSchemaAttribute)))
							{
								AddToTable(attributesResolved, xmlSchemaAttribute, xmlSchemaAttribute.RefName, h);
							}
							else if (!skipEquivalent || !AreAttributesEqual(xmlSchemaAttribute, attributesResolved[xmlSchemaAttribute.QualifiedName] as XmlSchemaAttribute))
							{
								AddToTable(attributesResolved, xmlSchemaAttribute, xmlSchemaAttribute.QualifiedName, h);
							}
						}
					}
					continue;
				}
				XmlSchemaAttribute xmlSchemaAttribute2 = attribute as XmlSchemaAttribute;
				if (xmlSchemaAttribute2 != null)
				{
					num += xmlSchemaAttribute2.Validate(h, schema);
					if (arrayList.Contains(xmlSchemaAttribute2.QualifiedName))
					{
						xmlSchemaAttribute2.error(h, string.Format("Duplicate attributes was found for '{0}'", xmlSchemaAttribute2.QualifiedName));
					}
					arrayList.Add(xmlSchemaAttribute2.QualifiedName);
					if (!StrictMsCompliant || xmlSchemaAttribute2.Use != XmlSchemaUse.Prohibited)
					{
						if (xmlSchemaAttribute2.RefName != null && xmlSchemaAttribute2.RefName != XmlQualifiedName.Empty && (!skipEquivalent || !AreAttributesEqual(xmlSchemaAttribute2, attributesResolved[xmlSchemaAttribute2.RefName] as XmlSchemaAttribute)))
						{
							AddToTable(attributesResolved, xmlSchemaAttribute2, xmlSchemaAttribute2.RefName, h);
						}
						else if (!skipEquivalent || !AreAttributesEqual(xmlSchemaAttribute2, attributesResolved[xmlSchemaAttribute2.QualifiedName] as XmlSchemaAttribute))
						{
							AddToTable(attributesResolved, xmlSchemaAttribute2, xmlSchemaAttribute2.QualifiedName, h);
						}
					}
				}
				else if (anyAttribute == null)
				{
					anyAttributeUse = (XmlSchemaAnyAttribute)attribute;
					anyAttribute.Validate(h, schema);
				}
			}
			return num;
		}

		internal static bool AreAttributesEqual(XmlSchemaAttribute one, XmlSchemaAttribute another)
		{
			if (one == null || another == null)
			{
				return false;
			}
			return one.AttributeType == another.AttributeType && one.Form == another.Form && one.ValidatedUse == another.ValidatedUse && one.ValidatedDefaultValue == another.ValidatedDefaultValue && one.ValidatedFixedValue == another.ValidatedFixedValue;
		}

		public static object ReadTypedValue(XmlReader reader, object type, IXmlNamespaceResolver nsResolver, StringBuilder tmpBuilder)
		{
			if (tmpBuilder == null)
			{
				tmpBuilder = new StringBuilder();
			}
			XmlSchemaDatatype xmlSchemaDatatype = type as XmlSchemaDatatype;
			XmlSchemaSimpleType xmlSchemaSimpleType = type as XmlSchemaSimpleType;
			if (xmlSchemaSimpleType != null)
			{
				xmlSchemaDatatype = xmlSchemaSimpleType.Datatype;
			}
			if (xmlSchemaDatatype == null)
			{
				return null;
			}
			switch (reader.NodeType)
			{
			case XmlNodeType.Element:
			{
				if (reader.IsEmptyElement)
				{
					return null;
				}
				tmpBuilder.Length = 0;
				bool flag = true;
				do
				{
					reader.Read();
					switch (reader.NodeType)
					{
					case XmlNodeType.Text:
					case XmlNodeType.CDATA:
					case XmlNodeType.SignificantWhitespace:
						tmpBuilder.Append(reader.Value);
						break;
					default:
						flag = false;
						break;
					case XmlNodeType.Comment:
						break;
					}
				}
				while (flag && !reader.EOF && reader.ReadState == ReadState.Interactive);
				return xmlSchemaDatatype.ParseValue(tmpBuilder.ToString(), reader.NameTable, nsResolver);
			}
			case XmlNodeType.Attribute:
				return xmlSchemaDatatype.ParseValue(reader.Value, reader.NameTable, nsResolver);
			default:
				return null;
			}
		}

		public static XmlSchemaObject FindAttributeDeclaration(string ns, XmlSchemaSet schemas, XmlSchemaComplexType cType, XmlQualifiedName qname)
		{
			XmlSchemaObject xmlSchemaObject = cType.AttributeUses[qname];
			if (xmlSchemaObject != null)
			{
				return xmlSchemaObject;
			}
			if (cType.AttributeWildcard == null)
			{
				return null;
			}
			if (!AttributeWildcardItemValid(cType.AttributeWildcard, qname, ns))
			{
				return null;
			}
			if (cType.AttributeWildcard.ResolvedProcessContents == XmlSchemaContentProcessing.Skip)
			{
				return cType.AttributeWildcard;
			}
			XmlSchemaAttribute xmlSchemaAttribute = schemas.GlobalAttributes[qname] as XmlSchemaAttribute;
			if (xmlSchemaAttribute != null)
			{
				return xmlSchemaAttribute;
			}
			if (cType.AttributeWildcard.ResolvedProcessContents == XmlSchemaContentProcessing.Lax)
			{
				return cType.AttributeWildcard;
			}
			return null;
		}

		private static bool AttributeWildcardItemValid(XmlSchemaAnyAttribute anyAttr, XmlQualifiedName qname, string ns)
		{
			if (anyAttr.HasValueAny)
			{
				return true;
			}
			if (anyAttr.HasValueOther && (anyAttr.TargetNamespace == string.Empty || ns != anyAttr.TargetNamespace))
			{
				return true;
			}
			if (anyAttr.HasValueTargetNamespace && ns == anyAttr.TargetNamespace)
			{
				return true;
			}
			if (anyAttr.HasValueLocal && ns == string.Empty)
			{
				return true;
			}
			for (int i = 0; i < anyAttr.ResolvedNamespaces.Count; i++)
			{
				if (anyAttr.ResolvedNamespaces[i] == ns)
				{
					return true;
				}
			}
			return false;
		}
	}
}
