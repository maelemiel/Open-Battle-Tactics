using System.ComponentModel;
using System.Xml.Serialization;
using Mono.Xml.Schema;

namespace System.Xml.Schema
{
	public class XmlSchemaType : XmlSchemaAnnotated
	{
		private XmlSchemaDerivationMethod final;

		private bool isMixed;

		private string name;

		private bool recursed;

		internal XmlQualifiedName BaseSchemaTypeName;

		internal XmlSchemaType BaseXmlSchemaTypeInternal;

		internal XmlSchemaDatatype DatatypeInternal;

		internal XmlSchemaDerivationMethod resolvedDerivedBy;

		internal XmlSchemaDerivationMethod finalResolved;

		internal XmlQualifiedName QNameInternal;

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

		[DefaultValue(XmlSchemaDerivationMethod.None)]
		[XmlAttribute("final")]
		public XmlSchemaDerivationMethod Final
		{
			get
			{
				return final;
			}
			set
			{
				final = value;
			}
		}

		[XmlIgnore]
		public XmlQualifiedName QualifiedName
		{
			get
			{
				return QNameInternal;
			}
		}

		[XmlIgnore]
		public XmlSchemaDerivationMethod FinalResolved
		{
			get
			{
				return finalResolved;
			}
		}

		[Obsolete("This property is going away. Use BaseXmlSchemaType instead")]
		[XmlIgnore]
		public object BaseSchemaType
		{
			get
			{
				if (BaseXmlSchemaType != null)
				{
					return BaseXmlSchemaType;
				}
				if (this == XmlSchemaComplexType.AnyType)
				{
					return null;
				}
				return Datatype;
			}
		}

		[System.MonoTODO]
		[XmlIgnore]
		public XmlSchemaType BaseXmlSchemaType
		{
			get
			{
				return BaseXmlSchemaTypeInternal;
			}
		}

		[XmlIgnore]
		public XmlSchemaDerivationMethod DerivedBy
		{
			get
			{
				return resolvedDerivedBy;
			}
		}

		[XmlIgnore]
		public XmlSchemaDatatype Datatype
		{
			get
			{
				return DatatypeInternal;
			}
		}

		[XmlIgnore]
		public virtual bool IsMixed
		{
			get
			{
				return isMixed;
			}
			set
			{
				isMixed = value;
			}
		}

		[XmlIgnore]
		public XmlTypeCode TypeCode
		{
			get
			{
				if (this == XmlSchemaComplexType.AnyType)
				{
					return XmlTypeCode.Item;
				}
				if (DatatypeInternal == XmlSchemaSimpleType.AnySimpleType)
				{
					return XmlTypeCode.AnyAtomicType;
				}
				if (this == XmlSchemaSimpleType.XsIDRefs)
				{
					return XmlTypeCode.Idref;
				}
				if (this == XmlSchemaSimpleType.XsEntities)
				{
					return XmlTypeCode.Entity;
				}
				if (this == XmlSchemaSimpleType.XsNMTokens)
				{
					return XmlTypeCode.NmToken;
				}
				if (DatatypeInternal != null)
				{
					return DatatypeInternal.TypeCode;
				}
				return BaseXmlSchemaType.TypeCode;
			}
		}

		public XmlSchemaType()
		{
			final = XmlSchemaDerivationMethod.None;
			QNameInternal = XmlQualifiedName.Empty;
		}

		internal static XmlSchemaType GetBuiltInType(XmlQualifiedName qualifiedName)
		{
			XmlSchemaType xmlSchemaType = GetBuiltInSimpleType(qualifiedName);
			if (xmlSchemaType == null)
			{
				xmlSchemaType = GetBuiltInComplexType(qualifiedName);
			}
			return xmlSchemaType;
		}

		internal static XmlSchemaType GetBuiltInType(XmlTypeCode typecode)
		{
			if (typecode == XmlTypeCode.Item)
			{
				return XmlSchemaComplexType.AnyType;
			}
			return GetBuiltInSimpleType(typecode);
		}

		public static XmlSchemaComplexType GetBuiltInComplexType(XmlQualifiedName qualifiedName)
		{
			if (qualifiedName.Name == "anyType" && qualifiedName.Namespace == "http://www.w3.org/2001/XMLSchema")
			{
				return XmlSchemaComplexType.AnyType;
			}
			return null;
		}

		public static XmlSchemaComplexType GetBuiltInComplexType(XmlTypeCode type)
		{
			if (type == XmlTypeCode.Item)
			{
				return XmlSchemaComplexType.AnyType;
			}
			return null;
		}

		[System.MonoTODO]
		public static XmlSchemaSimpleType GetBuiltInSimpleType(XmlQualifiedName qualifiedName)
		{
			if (qualifiedName.Namespace == "http://www.w3.org/2003/11/xpath-datatypes")
			{
				switch (qualifiedName.Name)
				{
				case "untypedAtomic":
					return XmlSchemaSimpleType.XdtUntypedAtomic;
				case "anyAtomicType":
					return XmlSchemaSimpleType.XdtAnyAtomicType;
				case "yearMonthDuration":
					return XmlSchemaSimpleType.XdtYearMonthDuration;
				case "dayTimeDuration":
					return XmlSchemaSimpleType.XdtDayTimeDuration;
				default:
					return null;
				}
			}
			if (qualifiedName.Namespace != "http://www.w3.org/2001/XMLSchema")
			{
				return null;
			}
			switch (qualifiedName.Name)
			{
			case "anySimpleType":
				return XmlSchemaSimpleType.XsAnySimpleType;
			case "string":
				return XmlSchemaSimpleType.XsString;
			case "boolean":
				return XmlSchemaSimpleType.XsBoolean;
			case "decimal":
				return XmlSchemaSimpleType.XsDecimal;
			case "float":
				return XmlSchemaSimpleType.XsFloat;
			case "double":
				return XmlSchemaSimpleType.XsDouble;
			case "duration":
				return XmlSchemaSimpleType.XsDuration;
			case "dateTime":
				return XmlSchemaSimpleType.XsDateTime;
			case "time":
				return XmlSchemaSimpleType.XsTime;
			case "date":
				return XmlSchemaSimpleType.XsDate;
			case "gYearMonth":
				return XmlSchemaSimpleType.XsGYearMonth;
			case "gYear":
				return XmlSchemaSimpleType.XsGYear;
			case "gMonthDay":
				return XmlSchemaSimpleType.XsGMonthDay;
			case "gDay":
				return XmlSchemaSimpleType.XsGDay;
			case "gMonth":
				return XmlSchemaSimpleType.XsGMonth;
			case "hexBinary":
				return XmlSchemaSimpleType.XsHexBinary;
			case "base64Binary":
				return XmlSchemaSimpleType.XsBase64Binary;
			case "anyURI":
				return XmlSchemaSimpleType.XsAnyUri;
			case "QName":
				return XmlSchemaSimpleType.XsQName;
			case "NOTATION":
				return XmlSchemaSimpleType.XsNotation;
			case "normalizedString":
				return XmlSchemaSimpleType.XsNormalizedString;
			case "token":
				return XmlSchemaSimpleType.XsToken;
			case "language":
				return XmlSchemaSimpleType.XsLanguage;
			case "NMTOKEN":
				return XmlSchemaSimpleType.XsNMToken;
			case "NMTOKENS":
				return XmlSchemaSimpleType.XsNMTokens;
			case "Name":
				return XmlSchemaSimpleType.XsName;
			case "NCName":
				return XmlSchemaSimpleType.XsNCName;
			case "ID":
				return XmlSchemaSimpleType.XsID;
			case "IDREF":
				return XmlSchemaSimpleType.XsIDRef;
			case "IDREFS":
				return XmlSchemaSimpleType.XsIDRefs;
			case "ENTITY":
				return XmlSchemaSimpleType.XsEntity;
			case "ENTITIES":
				return XmlSchemaSimpleType.XsEntities;
			case "integer":
				return XmlSchemaSimpleType.XsInteger;
			case "nonPositiveInteger":
				return XmlSchemaSimpleType.XsNonPositiveInteger;
			case "negativeInteger":
				return XmlSchemaSimpleType.XsNegativeInteger;
			case "long":
				return XmlSchemaSimpleType.XsLong;
			case "int":
				return XmlSchemaSimpleType.XsInt;
			case "short":
				return XmlSchemaSimpleType.XsShort;
			case "byte":
				return XmlSchemaSimpleType.XsByte;
			case "nonNegativeInteger":
				return XmlSchemaSimpleType.XsNonNegativeInteger;
			case "positiveInteger":
				return XmlSchemaSimpleType.XsPositiveInteger;
			case "unsignedLong":
				return XmlSchemaSimpleType.XsUnsignedLong;
			case "unsignedInt":
				return XmlSchemaSimpleType.XsUnsignedInt;
			case "unsignedShort":
				return XmlSchemaSimpleType.XsUnsignedShort;
			case "unsignedByte":
				return XmlSchemaSimpleType.XsUnsignedByte;
			default:
				return null;
			}
		}

		internal static XmlSchemaSimpleType GetBuiltInSimpleType(XmlSchemaDatatype type)
		{
			if (type is XsdEntities)
			{
				return XmlSchemaSimpleType.XsEntities;
			}
			if (type is XsdNMTokens)
			{
				return XmlSchemaSimpleType.XsNMTokens;
			}
			if (type is XsdIDRefs)
			{
				return XmlSchemaSimpleType.XsIDRefs;
			}
			return GetBuiltInSimpleType(type.TypeCode);
		}

		[System.MonoTODO]
		public static XmlSchemaSimpleType GetBuiltInSimpleType(XmlTypeCode type)
		{
			switch (type)
			{
			case XmlTypeCode.None:
			case XmlTypeCode.Item:
			case XmlTypeCode.Node:
			case XmlTypeCode.Document:
			case XmlTypeCode.Element:
			case XmlTypeCode.Attribute:
			case XmlTypeCode.Namespace:
			case XmlTypeCode.ProcessingInstruction:
			case XmlTypeCode.Comment:
			case XmlTypeCode.Text:
				return null;
			case XmlTypeCode.AnyAtomicType:
				return XmlSchemaSimpleType.XdtAnyAtomicType;
			case XmlTypeCode.UntypedAtomic:
				return XmlSchemaSimpleType.XdtUntypedAtomic;
			case XmlTypeCode.String:
				return XmlSchemaSimpleType.XsString;
			case XmlTypeCode.Boolean:
				return XmlSchemaSimpleType.XsBoolean;
			case XmlTypeCode.Decimal:
				return XmlSchemaSimpleType.XsDecimal;
			case XmlTypeCode.Float:
				return XmlSchemaSimpleType.XsFloat;
			case XmlTypeCode.Double:
				return XmlSchemaSimpleType.XsDouble;
			case XmlTypeCode.Duration:
				return XmlSchemaSimpleType.XsDuration;
			case XmlTypeCode.DateTime:
				return XmlSchemaSimpleType.XsDateTime;
			case XmlTypeCode.Time:
				return XmlSchemaSimpleType.XsTime;
			case XmlTypeCode.Date:
				return XmlSchemaSimpleType.XsDate;
			case XmlTypeCode.GYearMonth:
				return XmlSchemaSimpleType.XsGYearMonth;
			case XmlTypeCode.GYear:
				return XmlSchemaSimpleType.XsGYear;
			case XmlTypeCode.GMonthDay:
				return XmlSchemaSimpleType.XsGMonthDay;
			case XmlTypeCode.GDay:
				return XmlSchemaSimpleType.XsGDay;
			case XmlTypeCode.GMonth:
				return XmlSchemaSimpleType.XsGMonth;
			case XmlTypeCode.HexBinary:
				return XmlSchemaSimpleType.XsHexBinary;
			case XmlTypeCode.Base64Binary:
				return XmlSchemaSimpleType.XsBase64Binary;
			case XmlTypeCode.AnyUri:
				return XmlSchemaSimpleType.XsAnyUri;
			case XmlTypeCode.QName:
				return XmlSchemaSimpleType.XsQName;
			case XmlTypeCode.Notation:
				return XmlSchemaSimpleType.XsNotation;
			case XmlTypeCode.NormalizedString:
				return XmlSchemaSimpleType.XsNormalizedString;
			case XmlTypeCode.Token:
				return XmlSchemaSimpleType.XsToken;
			case XmlTypeCode.Language:
				return XmlSchemaSimpleType.XsLanguage;
			case XmlTypeCode.NmToken:
				return XmlSchemaSimpleType.XsNMToken;
			case XmlTypeCode.Name:
				return XmlSchemaSimpleType.XsName;
			case XmlTypeCode.NCName:
				return XmlSchemaSimpleType.XsNCName;
			case XmlTypeCode.Id:
				return XmlSchemaSimpleType.XsID;
			case XmlTypeCode.Idref:
				return XmlSchemaSimpleType.XsIDRef;
			case XmlTypeCode.Entity:
				return XmlSchemaSimpleType.XsEntity;
			case XmlTypeCode.Integer:
				return XmlSchemaSimpleType.XsInteger;
			case XmlTypeCode.NonPositiveInteger:
				return XmlSchemaSimpleType.XsNonPositiveInteger;
			case XmlTypeCode.NegativeInteger:
				return XmlSchemaSimpleType.XsNegativeInteger;
			case XmlTypeCode.Long:
				return XmlSchemaSimpleType.XsLong;
			case XmlTypeCode.Int:
				return XmlSchemaSimpleType.XsInt;
			case XmlTypeCode.Short:
				return XmlSchemaSimpleType.XsShort;
			case XmlTypeCode.Byte:
				return XmlSchemaSimpleType.XsByte;
			case XmlTypeCode.NonNegativeInteger:
				return XmlSchemaSimpleType.XsNonNegativeInteger;
			case XmlTypeCode.UnsignedLong:
				return XmlSchemaSimpleType.XsUnsignedLong;
			case XmlTypeCode.UnsignedInt:
				return XmlSchemaSimpleType.XsUnsignedInt;
			case XmlTypeCode.UnsignedShort:
				return XmlSchemaSimpleType.XsUnsignedShort;
			case XmlTypeCode.UnsignedByte:
				return XmlSchemaSimpleType.XsUnsignedByte;
			case XmlTypeCode.PositiveInteger:
				return XmlSchemaSimpleType.XsPositiveInteger;
			case XmlTypeCode.YearMonthDuration:
				return XmlSchemaSimpleType.XdtYearMonthDuration;
			case XmlTypeCode.DayTimeDuration:
				return XmlSchemaSimpleType.XdtDayTimeDuration;
			default:
				return null;
			}
		}

		public static bool IsDerivedFrom(XmlSchemaType derivedType, XmlSchemaType baseType, XmlSchemaDerivationMethod except)
		{
			if (derivedType.BaseXmlSchemaType == null)
			{
				return false;
			}
			if ((derivedType.DerivedBy & except) != XmlSchemaDerivationMethod.Empty)
			{
				return false;
			}
			if (derivedType.BaseXmlSchemaType == baseType)
			{
				return true;
			}
			return IsDerivedFrom(derivedType.BaseXmlSchemaType, baseType, except);
		}

		internal bool ValidateRecursionCheck()
		{
			if (recursed)
			{
				return this != XmlSchemaComplexType.AnyType;
			}
			recursed = true;
			XmlSchemaType baseXmlSchemaType = BaseXmlSchemaType;
			bool result = false;
			if (baseXmlSchemaType != null)
			{
				result = baseXmlSchemaType.ValidateRecursionCheck();
			}
			recursed = false;
			return result;
		}
	}
}
