using System.Xml.Serialization;
using Mono.Xml.Schema;

namespace System.Xml.Schema
{
	public class XmlSchemaSimpleType : XmlSchemaType
	{
		private const string xmlname = "simpleType";

		private static XmlSchemaSimpleType schemaLocationType;

		private XmlSchemaSimpleTypeContent content;

		internal bool islocal = true;

		private bool recursed;

		private XmlSchemaDerivationMethod variety;

		internal static readonly XmlSchemaSimpleType XsAnySimpleType;

		internal static readonly XmlSchemaSimpleType XsString;

		internal static readonly XmlSchemaSimpleType XsBoolean;

		internal static readonly XmlSchemaSimpleType XsDecimal;

		internal static readonly XmlSchemaSimpleType XsFloat;

		internal static readonly XmlSchemaSimpleType XsDouble;

		internal static readonly XmlSchemaSimpleType XsDuration;

		internal static readonly XmlSchemaSimpleType XsDateTime;

		internal static readonly XmlSchemaSimpleType XsTime;

		internal static readonly XmlSchemaSimpleType XsDate;

		internal static readonly XmlSchemaSimpleType XsGYearMonth;

		internal static readonly XmlSchemaSimpleType XsGYear;

		internal static readonly XmlSchemaSimpleType XsGMonthDay;

		internal static readonly XmlSchemaSimpleType XsGDay;

		internal static readonly XmlSchemaSimpleType XsGMonth;

		internal static readonly XmlSchemaSimpleType XsHexBinary;

		internal static readonly XmlSchemaSimpleType XsBase64Binary;

		internal static readonly XmlSchemaSimpleType XsAnyUri;

		internal static readonly XmlSchemaSimpleType XsQName;

		internal static readonly XmlSchemaSimpleType XsNotation;

		internal static readonly XmlSchemaSimpleType XsNormalizedString;

		internal static readonly XmlSchemaSimpleType XsToken;

		internal static readonly XmlSchemaSimpleType XsLanguage;

		internal static readonly XmlSchemaSimpleType XsNMToken;

		internal static readonly XmlSchemaSimpleType XsNMTokens;

		internal static readonly XmlSchemaSimpleType XsName;

		internal static readonly XmlSchemaSimpleType XsNCName;

		internal static readonly XmlSchemaSimpleType XsID;

		internal static readonly XmlSchemaSimpleType XsIDRef;

		internal static readonly XmlSchemaSimpleType XsIDRefs;

		internal static readonly XmlSchemaSimpleType XsEntity;

		internal static readonly XmlSchemaSimpleType XsEntities;

		internal static readonly XmlSchemaSimpleType XsInteger;

		internal static readonly XmlSchemaSimpleType XsNonPositiveInteger;

		internal static readonly XmlSchemaSimpleType XsNegativeInteger;

		internal static readonly XmlSchemaSimpleType XsLong;

		internal static readonly XmlSchemaSimpleType XsInt;

		internal static readonly XmlSchemaSimpleType XsShort;

		internal static readonly XmlSchemaSimpleType XsByte;

		internal static readonly XmlSchemaSimpleType XsNonNegativeInteger;

		internal static readonly XmlSchemaSimpleType XsUnsignedLong;

		internal static readonly XmlSchemaSimpleType XsUnsignedInt;

		internal static readonly XmlSchemaSimpleType XsUnsignedShort;

		internal static readonly XmlSchemaSimpleType XsUnsignedByte;

		internal static readonly XmlSchemaSimpleType XsPositiveInteger;

		internal static readonly XmlSchemaSimpleType XdtUntypedAtomic;

		internal static readonly XmlSchemaSimpleType XdtAnyAtomicType;

		internal static readonly XmlSchemaSimpleType XdtYearMonthDuration;

		internal static readonly XmlSchemaSimpleType XdtDayTimeDuration;

		internal static XsdAnySimpleType AnySimpleType
		{
			get
			{
				return XsdAnySimpleType.Instance;
			}
		}

		internal static XmlSchemaSimpleType SchemaLocationType
		{
			get
			{
				return schemaLocationType;
			}
		}

		[XmlElement("list", typeof(XmlSchemaSimpleTypeList))]
		[XmlElement("restriction", typeof(XmlSchemaSimpleTypeRestriction))]
		[XmlElement("union", typeof(XmlSchemaSimpleTypeUnion))]
		public XmlSchemaSimpleTypeContent Content
		{
			get
			{
				return content;
			}
			set
			{
				content = value;
			}
		}

		internal XmlSchemaDerivationMethod Variety
		{
			get
			{
				return variety;
			}
		}

		static XmlSchemaSimpleType()
		{
			schemaLocationType = new XmlSchemaSimpleType
			{
				Content = new XmlSchemaSimpleTypeList
				{
					ItemTypeName = new XmlQualifiedName("anyURI", "http://www.w3.org/2001/XMLSchema")
				},
				BaseXmlSchemaTypeInternal = null,
				variety = XmlSchemaDerivationMethod.List
			};
			XsAnySimpleType = BuildSchemaType("anySimpleType", null);
			XsString = BuildSchemaType("string", "anySimpleType");
			XsBoolean = BuildSchemaType("boolean", "anySimpleType");
			XsDecimal = BuildSchemaType("decimal", "anySimpleType");
			XsFloat = BuildSchemaType("float", "anySimpleType");
			XsDouble = BuildSchemaType("double", "anySimpleType");
			XsDuration = BuildSchemaType("duration", "anySimpleType");
			XsDateTime = BuildSchemaType("dateTime", "anySimpleType");
			XsTime = BuildSchemaType("time", "anySimpleType");
			XsDate = BuildSchemaType("date", "anySimpleType");
			XsGYearMonth = BuildSchemaType("gYearMonth", "anySimpleType");
			XsGYear = BuildSchemaType("gYear", "anySimpleType");
			XsGMonthDay = BuildSchemaType("gMonthDay", "anySimpleType");
			XsGDay = BuildSchemaType("gDay", "anySimpleType");
			XsGMonth = BuildSchemaType("gMonth", "anySimpleType");
			XsHexBinary = BuildSchemaType("hexBinary", "anySimpleType");
			XsBase64Binary = BuildSchemaType("base64Binary", "anySimpleType");
			XsAnyUri = BuildSchemaType("anyURI", "anySimpleType");
			XsQName = BuildSchemaType("QName", "anySimpleType");
			XsNotation = BuildSchemaType("NOTATION", "anySimpleType");
			XsNormalizedString = BuildSchemaType("normalizedString", "string");
			XsToken = BuildSchemaType("token", "normalizedString");
			XsLanguage = BuildSchemaType("language", "token");
			XsNMToken = BuildSchemaType("NMTOKEN", "token");
			XsName = BuildSchemaType("Name", "token");
			XsNCName = BuildSchemaType("NCName", "Name");
			XsID = BuildSchemaType("ID", "NCName");
			XsIDRef = BuildSchemaType("IDREF", "NCName");
			XsEntity = BuildSchemaType("ENTITY", "NCName");
			XsInteger = BuildSchemaType("integer", "decimal");
			XsNonPositiveInteger = BuildSchemaType("nonPositiveInteger", "integer");
			XsNegativeInteger = BuildSchemaType("negativeInteger", "nonPositiveInteger");
			XsLong = BuildSchemaType("long", "integer");
			XsInt = BuildSchemaType("int", "long");
			XsShort = BuildSchemaType("short", "int");
			XsByte = BuildSchemaType("byte", "short");
			XsNonNegativeInteger = BuildSchemaType("nonNegativeInteger", "integer");
			XsUnsignedLong = BuildSchemaType("unsignedLong", "nonNegativeInteger");
			XsUnsignedInt = BuildSchemaType("unsignedInt", "unsignedLong");
			XsUnsignedShort = BuildSchemaType("unsignedShort", "unsignedInt");
			XsUnsignedByte = BuildSchemaType("unsignedByte", "unsignedShort");
			XsPositiveInteger = BuildSchemaType("positiveInteger", "nonNegativeInteger");
			XdtAnyAtomicType = BuildSchemaType("anyAtomicType", "anySimpleType", true, false);
			XdtUntypedAtomic = BuildSchemaType("untypedAtomic", "anyAtomicType", true, true);
			XdtDayTimeDuration = BuildSchemaType("dayTimeDuration", "duration", true, false);
			XdtYearMonthDuration = BuildSchemaType("yearMonthDuration", "duration", true, false);
			XsIDRefs = new XmlSchemaSimpleType();
			XmlSchemaSimpleTypeList xmlSchemaSimpleTypeList = new XmlSchemaSimpleTypeList
			{
				ItemType = XsIDRef
			};
			XsIDRefs.Content = xmlSchemaSimpleTypeList;
			XsEntities = new XmlSchemaSimpleType();
			xmlSchemaSimpleTypeList = new XmlSchemaSimpleTypeList
			{
				ItemType = XsEntity
			};
			XsEntities.Content = xmlSchemaSimpleTypeList;
			XsNMTokens = new XmlSchemaSimpleType();
			xmlSchemaSimpleTypeList = new XmlSchemaSimpleTypeList
			{
				ItemType = XsNMToken
			};
			XsNMTokens.Content = xmlSchemaSimpleTypeList;
		}

		private static XmlSchemaSimpleType BuildSchemaType(string name, string baseName)
		{
			return BuildSchemaType(name, baseName, false, false);
		}

		private static XmlSchemaSimpleType BuildSchemaType(string name, string baseName, bool xdt, bool baseXdt)
		{
			string ns = ((!xdt) ? "http://www.w3.org/2001/XMLSchema" : "http://www.w3.org/2003/11/xpath-datatypes");
			string ns2 = ((!baseXdt) ? "http://www.w3.org/2001/XMLSchema" : "http://www.w3.org/2003/11/xpath-datatypes");
			XmlSchemaSimpleType xmlSchemaSimpleType = new XmlSchemaSimpleType();
			xmlSchemaSimpleType.QNameInternal = new XmlQualifiedName(name, ns);
			if (baseName != null)
			{
				xmlSchemaSimpleType.BaseXmlSchemaTypeInternal = XmlSchemaType.GetBuiltInSimpleType(new XmlQualifiedName(baseName, ns2));
			}
			xmlSchemaSimpleType.DatatypeInternal = XmlSchemaDatatype.FromName(xmlSchemaSimpleType.QualifiedName);
			return xmlSchemaSimpleType;
		}

		internal override void SetParent(XmlSchemaObject parent)
		{
			base.SetParent(parent);
			if (Content != null)
			{
				Content.SetParent(this);
			}
		}

		internal override int Compile(ValidationEventHandler h, XmlSchema schema)
		{
			if (CompilationId == schema.CompilationId)
			{
				return 0;
			}
			errorCount = 0;
			if (islocal)
			{
				if (base.Name != null)
				{
					error(h, "Name is prohibited in a local simpletype");
				}
				else
				{
					QNameInternal = new XmlQualifiedName(base.Name, base.AncestorSchema.TargetNamespace);
				}
				if (base.Final != XmlSchemaDerivationMethod.None)
				{
					error(h, "Final is prohibited in a local simpletype");
				}
			}
			else
			{
				if (base.Name == null)
				{
					error(h, "Name is required in top level simpletype");
				}
				else if (!XmlSchemaUtil.CheckNCName(base.Name))
				{
					error(h, "name attribute of a simpleType must be NCName");
				}
				else
				{
					QNameInternal = new XmlQualifiedName(base.Name, base.AncestorSchema.TargetNamespace);
				}
				XmlSchemaDerivationMethod xmlSchemaDerivationMethod = base.Final;
				if (xmlSchemaDerivationMethod != XmlSchemaDerivationMethod.All)
				{
					if (xmlSchemaDerivationMethod != XmlSchemaDerivationMethod.None)
					{
						if (xmlSchemaDerivationMethod == XmlSchemaDerivationMethod.Restriction || xmlSchemaDerivationMethod == XmlSchemaDerivationMethod.List || xmlSchemaDerivationMethod == XmlSchemaDerivationMethod.Union)
						{
							finalResolved = base.Final;
							goto IL_019f;
						}
						error(h, "The value of final attribute is not valid for simpleType");
					}
					XmlSchemaDerivationMethod xmlSchemaDerivationMethod2 = XmlSchemaDerivationMethod.Extension | XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.List | XmlSchemaDerivationMethod.Union;
					switch (schema.FinalDefault)
					{
					case XmlSchemaDerivationMethod.All:
						finalResolved = XmlSchemaDerivationMethod.All;
						break;
					case XmlSchemaDerivationMethod.None:
						finalResolved = XmlSchemaDerivationMethod.Empty;
						break;
					default:
						finalResolved = schema.FinalDefault & xmlSchemaDerivationMethod2;
						break;
					}
				}
				else
				{
					finalResolved = XmlSchemaDerivationMethod.All;
				}
			}
			goto IL_019f;
			IL_019f:
			XmlSchemaUtil.CompileID(base.Id, this, schema.IDCollection, h);
			if (Content != null)
			{
				Content.OwnerType = this;
			}
			if (Content == null)
			{
				error(h, "Content is required in a simpletype");
			}
			else if (Content is XmlSchemaSimpleTypeRestriction)
			{
				resolvedDerivedBy = XmlSchemaDerivationMethod.Restriction;
				errorCount += ((XmlSchemaSimpleTypeRestriction)Content).Compile(h, schema);
			}
			else if (Content is XmlSchemaSimpleTypeList)
			{
				resolvedDerivedBy = XmlSchemaDerivationMethod.List;
				errorCount += ((XmlSchemaSimpleTypeList)Content).Compile(h, schema);
			}
			else if (Content is XmlSchemaSimpleTypeUnion)
			{
				resolvedDerivedBy = XmlSchemaDerivationMethod.Union;
				errorCount += ((XmlSchemaSimpleTypeUnion)Content).Compile(h, schema);
			}
			CompilationId = schema.CompilationId;
			return errorCount;
		}

		internal void CollectBaseType(ValidationEventHandler h, XmlSchema schema)
		{
			if (Content is XmlSchemaSimpleTypeRestriction)
			{
				object actualType = ((XmlSchemaSimpleTypeRestriction)Content).GetActualType(h, schema, false);
				BaseXmlSchemaTypeInternal = actualType as XmlSchemaSimpleType;
				if (BaseXmlSchemaTypeInternal != null)
				{
					DatatypeInternal = BaseXmlSchemaTypeInternal.Datatype;
				}
				else
				{
					DatatypeInternal = actualType as XmlSchemaDatatype;
				}
			}
			else
			{
				DatatypeInternal = AnySimpleType;
			}
		}

		internal override int Validate(ValidationEventHandler h, XmlSchema schema)
		{
			if (IsValidated(schema.ValidationId))
			{
				return errorCount;
			}
			if (recursed)
			{
				error(h, "Circular type reference was found.");
				return errorCount;
			}
			recursed = true;
			CollectBaseType(h, schema);
			if (content != null)
			{
				errorCount += content.Validate(h, schema);
			}
			XmlSchemaSimpleType xmlSchemaSimpleType = base.BaseXmlSchemaType as XmlSchemaSimpleType;
			if (xmlSchemaSimpleType != null)
			{
				DatatypeInternal = xmlSchemaSimpleType.Datatype;
			}
			XmlSchemaSimpleType xmlSchemaSimpleType2 = base.BaseXmlSchemaType as XmlSchemaSimpleType;
			if (xmlSchemaSimpleType2 != null && (xmlSchemaSimpleType2.FinalResolved & resolvedDerivedBy) != XmlSchemaDerivationMethod.Empty)
			{
				error(h, "Specified derivation is prohibited by the base simple type.");
			}
			if (resolvedDerivedBy == XmlSchemaDerivationMethod.Restriction && xmlSchemaSimpleType2 != null)
			{
				variety = xmlSchemaSimpleType2.Variety;
			}
			else
			{
				variety = resolvedDerivedBy;
			}
			XmlSchemaSimpleTypeRestriction xmlSchemaSimpleTypeRestriction = Content as XmlSchemaSimpleTypeRestriction;
			object baseType = ((base.BaseXmlSchemaType == null) ? ((object)base.Datatype) : ((object)base.BaseXmlSchemaType));
			if (xmlSchemaSimpleTypeRestriction != null)
			{
				ValidateDerivationValid(baseType, xmlSchemaSimpleTypeRestriction.Facets, h, schema);
			}
			XmlSchemaSimpleTypeList xmlSchemaSimpleTypeList = Content as XmlSchemaSimpleTypeList;
			if (xmlSchemaSimpleTypeList != null)
			{
				XmlSchemaSimpleType xmlSchemaSimpleType3 = xmlSchemaSimpleTypeList.ValidatedListItemType as XmlSchemaSimpleType;
				if (xmlSchemaSimpleType3 != null && xmlSchemaSimpleType3.Content is XmlSchemaSimpleTypeList)
				{
					error(h, "List type must not be derived from another list type.");
				}
			}
			recursed = false;
			ValidationId = schema.ValidationId;
			return errorCount;
		}

		internal void ValidateDerivationValid(object baseType, XmlSchemaObjectCollection facets, ValidationEventHandler h, XmlSchema schema)
		{
			XmlSchemaSimpleType xmlSchemaSimpleType = baseType as XmlSchemaSimpleType;
			switch (Variety)
			{
			case XmlSchemaDerivationMethod.Restriction:
				if (xmlSchemaSimpleType != null && xmlSchemaSimpleType.resolvedDerivedBy != XmlSchemaDerivationMethod.Restriction)
				{
					error(h, "Base schema type is not either atomic type or primitive type.");
				}
				if (xmlSchemaSimpleType != null && (xmlSchemaSimpleType.FinalResolved & XmlSchemaDerivationMethod.Restriction) != XmlSchemaDerivationMethod.Empty)
				{
					error(h, "Derivation by restriction is prohibited by the base simple type.");
				}
				break;
			case XmlSchemaDerivationMethod.List:
				if (facets == null)
				{
					break;
				}
				{
					foreach (XmlSchemaFacet facet in facets)
					{
						if (!(facet is XmlSchemaLengthFacet) && !(facet is XmlSchemaMaxLengthFacet) && !(facet is XmlSchemaMinLengthFacet) && !(facet is XmlSchemaEnumerationFacet) && !(facet is XmlSchemaPatternFacet))
						{
							error(h, "Not allowed facet was found on this simple type which derives list type.");
						}
					}
					break;
				}
			case XmlSchemaDerivationMethod.Union:
				if (facets == null)
				{
					break;
				}
				{
					foreach (XmlSchemaFacet facet2 in facets)
					{
						if (!(facet2 is XmlSchemaEnumerationFacet) && !(facet2 is XmlSchemaPatternFacet))
						{
							error(h, "Not allowed facet was found on this simple type which derives list type.");
						}
					}
					break;
				}
			}
		}

		internal bool ValidateTypeDerivationOK(object baseType, ValidationEventHandler h, XmlSchema schema, bool raiseError)
		{
			if (this == baseType || baseType == AnySimpleType || baseType == XmlSchemaComplexType.AnyType)
			{
				return true;
			}
			XmlSchemaSimpleType xmlSchemaSimpleType = baseType as XmlSchemaSimpleType;
			if (xmlSchemaSimpleType != null && (xmlSchemaSimpleType.FinalResolved & resolvedDerivedBy) != XmlSchemaDerivationMethod.Empty)
			{
				if (raiseError)
				{
					error(h, "Specified derivation is prohibited by the base type.");
				}
				return false;
			}
			if (base.BaseXmlSchemaType == baseType || base.Datatype == baseType)
			{
				return true;
			}
			XmlSchemaSimpleType xmlSchemaSimpleType2 = base.BaseXmlSchemaType as XmlSchemaSimpleType;
			if (xmlSchemaSimpleType2 != null && xmlSchemaSimpleType2.ValidateTypeDerivationOK(baseType, h, schema, false))
			{
				return true;
			}
			XmlSchemaDerivationMethod xmlSchemaDerivationMethod = Variety;
			if ((xmlSchemaDerivationMethod == XmlSchemaDerivationMethod.List || xmlSchemaDerivationMethod == XmlSchemaDerivationMethod.Union) && baseType == AnySimpleType)
			{
				return true;
			}
			if (xmlSchemaSimpleType != null && xmlSchemaSimpleType.Variety == XmlSchemaDerivationMethod.Union)
			{
				object[] validatedTypes = ((XmlSchemaSimpleTypeUnion)xmlSchemaSimpleType.Content).ValidatedTypes;
				foreach (object baseType2 in validatedTypes)
				{
					if (ValidateTypeDerivationOK(baseType2, h, schema, false))
					{
						return true;
					}
				}
			}
			if (raiseError)
			{
				error(h, "Invalid simple type derivation was found.");
			}
			return false;
		}

		internal string Normalize(string s, XmlNameTable nt, XmlNamespaceManager nsmgr)
		{
			return Content.Normalize(s, nt, nsmgr);
		}

		internal static XmlSchemaSimpleType Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaSimpleType xmlSchemaSimpleType = new XmlSchemaSimpleType();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "simpleType")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaGroup.Read, name=" + reader.Name, null);
				reader.Skip();
				return null;
			}
			xmlSchemaSimpleType.LineNumber = reader.LineNumber;
			xmlSchemaSimpleType.LinePosition = reader.LinePosition;
			xmlSchemaSimpleType.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "final")
				{
					Exception innerExcpetion;
					xmlSchemaSimpleType.Final = XmlSchemaUtil.ReadDerivationAttribute(reader, out innerExcpetion, "final", XmlSchemaUtil.FinalAllowed);
					if (innerExcpetion != null)
					{
						XmlSchemaObject.error(h, "some invalid values not a valid value for final", innerExcpetion);
					}
				}
				else if (reader.Name == "id")
				{
					xmlSchemaSimpleType.Id = reader.Value;
				}
				else if (reader.Name == "name")
				{
					xmlSchemaSimpleType.Name = reader.Value;
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for simpleType", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaSimpleType);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaSimpleType;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "simpleType")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaSimpleType.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaSimpleType.Annotation = xmlSchemaAnnotation;
					}
					continue;
				}
				if (num <= 2)
				{
					if (reader.LocalName == "restriction")
					{
						num = 3;
						XmlSchemaSimpleTypeRestriction xmlSchemaSimpleTypeRestriction = XmlSchemaSimpleTypeRestriction.Read(reader, h);
						if (xmlSchemaSimpleTypeRestriction != null)
						{
							xmlSchemaSimpleType.content = xmlSchemaSimpleTypeRestriction;
						}
						continue;
					}
					if (reader.LocalName == "list")
					{
						num = 3;
						XmlSchemaSimpleTypeList xmlSchemaSimpleTypeList = XmlSchemaSimpleTypeList.Read(reader, h);
						if (xmlSchemaSimpleTypeList != null)
						{
							xmlSchemaSimpleType.content = xmlSchemaSimpleTypeList;
						}
						continue;
					}
					if (reader.LocalName == "union")
					{
						num = 3;
						XmlSchemaSimpleTypeUnion xmlSchemaSimpleTypeUnion = XmlSchemaSimpleTypeUnion.Read(reader, h);
						if (xmlSchemaSimpleTypeUnion != null)
						{
							xmlSchemaSimpleType.content = xmlSchemaSimpleTypeUnion;
						}
						continue;
					}
				}
				reader.RaiseInvalidElementError();
			}
			return xmlSchemaSimpleType;
		}
	}
}
