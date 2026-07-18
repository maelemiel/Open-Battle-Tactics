using System.Text;
using Mono.Xml.Schema;

namespace System.Xml.Schema
{
	public abstract class XmlSchemaDatatype
	{
		internal XsdWhitespaceFacet WhitespaceValue;

		private static char[] wsChars = new char[4] { ' ', '\t', '\n', '\r' };

		private StringBuilder sb = new StringBuilder();

		private static readonly XsdAnySimpleType datatypeAnySimpleType = XsdAnySimpleType.Instance;

		private static readonly XsdString datatypeString = new XsdString();

		private static readonly XsdNormalizedString datatypeNormalizedString = new XsdNormalizedString();

		private static readonly XsdToken datatypeToken = new XsdToken();

		private static readonly XsdLanguage datatypeLanguage = new XsdLanguage();

		private static readonly XsdNMToken datatypeNMToken = new XsdNMToken();

		private static readonly XsdNMTokens datatypeNMTokens = new XsdNMTokens();

		private static readonly XsdName datatypeName = new XsdName();

		private static readonly XsdNCName datatypeNCName = new XsdNCName();

		private static readonly XsdID datatypeID = new XsdID();

		private static readonly XsdIDRef datatypeIDRef = new XsdIDRef();

		private static readonly XsdIDRefs datatypeIDRefs = new XsdIDRefs();

		private static readonly XsdEntity datatypeEntity = new XsdEntity();

		private static readonly XsdEntities datatypeEntities = new XsdEntities();

		private static readonly XsdNotation datatypeNotation = new XsdNotation();

		private static readonly XsdDecimal datatypeDecimal = new XsdDecimal();

		private static readonly XsdInteger datatypeInteger = new XsdInteger();

		private static readonly XsdLong datatypeLong = new XsdLong();

		private static readonly XsdInt datatypeInt = new XsdInt();

		private static readonly XsdShort datatypeShort = new XsdShort();

		private static readonly XsdByte datatypeByte = new XsdByte();

		private static readonly XsdNonNegativeInteger datatypeNonNegativeInteger = new XsdNonNegativeInteger();

		private static readonly XsdPositiveInteger datatypePositiveInteger = new XsdPositiveInteger();

		private static readonly XsdUnsignedLong datatypeUnsignedLong = new XsdUnsignedLong();

		private static readonly XsdUnsignedInt datatypeUnsignedInt = new XsdUnsignedInt();

		private static readonly XsdUnsignedShort datatypeUnsignedShort = new XsdUnsignedShort();

		private static readonly XsdUnsignedByte datatypeUnsignedByte = new XsdUnsignedByte();

		private static readonly XsdNonPositiveInteger datatypeNonPositiveInteger = new XsdNonPositiveInteger();

		private static readonly XsdNegativeInteger datatypeNegativeInteger = new XsdNegativeInteger();

		private static readonly XsdFloat datatypeFloat = new XsdFloat();

		private static readonly XsdDouble datatypeDouble = new XsdDouble();

		private static readonly XsdBase64Binary datatypeBase64Binary = new XsdBase64Binary();

		private static readonly XsdBoolean datatypeBoolean = new XsdBoolean();

		private static readonly XsdAnyURI datatypeAnyURI = new XsdAnyURI();

		private static readonly XsdDuration datatypeDuration = new XsdDuration();

		private static readonly XsdDateTime datatypeDateTime = new XsdDateTime();

		private static readonly XsdDate datatypeDate = new XsdDate();

		private static readonly XsdTime datatypeTime = new XsdTime();

		private static readonly XsdHexBinary datatypeHexBinary = new XsdHexBinary();

		private static readonly XsdQName datatypeQName = new XsdQName();

		private static readonly XsdGYearMonth datatypeGYearMonth = new XsdGYearMonth();

		private static readonly XsdGMonthDay datatypeGMonthDay = new XsdGMonthDay();

		private static readonly XsdGYear datatypeGYear = new XsdGYear();

		private static readonly XsdGMonth datatypeGMonth = new XsdGMonth();

		private static readonly XsdGDay datatypeGDay = new XsdGDay();

		private static readonly XdtAnyAtomicType datatypeAnyAtomicType = new XdtAnyAtomicType();

		private static readonly XdtUntypedAtomic datatypeUntypedAtomic = new XdtUntypedAtomic();

		private static readonly XdtDayTimeDuration datatypeDayTimeDuration = new XdtDayTimeDuration();

		private static readonly XdtYearMonthDuration datatypeYearMonthDuration = new XdtYearMonthDuration();

		internal virtual XsdWhitespaceFacet Whitespace
		{
			get
			{
				return WhitespaceValue;
			}
		}

		public virtual XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.None;
			}
		}

		public virtual XmlSchemaDatatypeVariety Variety
		{
			get
			{
				return XmlSchemaDatatypeVariety.Atomic;
			}
		}

		public abstract XmlTokenizedType TokenizedType { get; }

		public abstract Type ValueType { get; }

		[System.MonoTODO]
		public virtual object ChangeType(object value, Type targetType)
		{
			return ChangeType(value, targetType, null);
		}

		[System.MonoTODO]
		public virtual object ChangeType(object value, Type targetType, IXmlNamespaceResolver nsResolver)
		{
			throw new NotImplementedException();
		}

		public virtual bool IsDerivedFrom(XmlSchemaDatatype datatype)
		{
			return this == datatype;
		}

		public abstract object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr);

		internal virtual ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
		{
			return null;
		}

		internal string Normalize(string s)
		{
			return Normalize(s, Whitespace);
		}

		internal string Normalize(string s, XsdWhitespaceFacet whitespaceFacet)
		{
			int num = s.IndexOfAny(wsChars);
			if (num < 0)
			{
				return s;
			}
			switch (whitespaceFacet)
			{
			case XsdWhitespaceFacet.Collapse:
			{
				string[] array = s.Trim().Split(wsChars);
				foreach (string text in array)
				{
					if (text != string.Empty)
					{
						sb.Append(text);
						sb.Append(" ");
					}
				}
				string result = sb.ToString();
				sb.Length = 0;
				return result.Trim();
			}
			case XsdWhitespaceFacet.Replace:
			{
				sb.Length = 0;
				sb.Append(s);
				for (int i = 0; i < sb.Length; i++)
				{
					switch (sb[i])
					{
					case '\t':
					case '\n':
					case '\r':
						sb[i] = ' ';
						break;
					}
				}
				string result = sb.ToString();
				sb.Length = 0;
				return result;
			}
			default:
				return s;
			}
		}

		internal static XmlSchemaDatatype FromName(XmlQualifiedName qname)
		{
			return FromName(qname.Name, qname.Namespace);
		}

		internal static XmlSchemaDatatype FromName(string localName, string ns)
		{
			switch (ns)
			{
			case "http://www.w3.org/2003/11/xpath-datatypes":
				switch (localName)
				{
				case "anyAtomicType":
					return datatypeAnyAtomicType;
				case "untypedAtomic":
					return datatypeUntypedAtomic;
				case "dayTimeDuration":
					return datatypeDayTimeDuration;
				case "yearMonthDuration":
					return datatypeYearMonthDuration;
				default:
					return null;
				}
			default:
				return null;
			case "http://www.w3.org/2001/XMLSchema":
				switch (localName)
				{
				case "anySimpleType":
					return datatypeAnySimpleType;
				case "string":
					return datatypeString;
				case "normalizedString":
					return datatypeNormalizedString;
				case "token":
					return datatypeToken;
				case "language":
					return datatypeLanguage;
				case "NMTOKEN":
					return datatypeNMToken;
				case "NMTOKENS":
					return datatypeNMTokens;
				case "Name":
					return datatypeName;
				case "NCName":
					return datatypeNCName;
				case "ID":
					return datatypeID;
				case "IDREF":
					return datatypeIDRef;
				case "IDREFS":
					return datatypeIDRefs;
				case "ENTITY":
					return datatypeEntity;
				case "ENTITIES":
					return datatypeEntities;
				case "NOTATION":
					return datatypeNotation;
				case "decimal":
					return datatypeDecimal;
				case "integer":
					return datatypeInteger;
				case "long":
					return datatypeLong;
				case "int":
					return datatypeInt;
				case "short":
					return datatypeShort;
				case "byte":
					return datatypeByte;
				case "nonPositiveInteger":
					return datatypeNonPositiveInteger;
				case "negativeInteger":
					return datatypeNegativeInteger;
				case "nonNegativeInteger":
					return datatypeNonNegativeInteger;
				case "unsignedLong":
					return datatypeUnsignedLong;
				case "unsignedInt":
					return datatypeUnsignedInt;
				case "unsignedShort":
					return datatypeUnsignedShort;
				case "unsignedByte":
					return datatypeUnsignedByte;
				case "positiveInteger":
					return datatypePositiveInteger;
				case "float":
					return datatypeFloat;
				case "double":
					return datatypeDouble;
				case "base64Binary":
					return datatypeBase64Binary;
				case "boolean":
					return datatypeBoolean;
				case "anyURI":
					return datatypeAnyURI;
				case "duration":
					return datatypeDuration;
				case "dateTime":
					return datatypeDateTime;
				case "date":
					return datatypeDate;
				case "time":
					return datatypeTime;
				case "hexBinary":
					return datatypeHexBinary;
				case "QName":
					return datatypeQName;
				case "gYearMonth":
					return datatypeGYearMonth;
				case "gMonthDay":
					return datatypeGMonthDay;
				case "gYear":
					return datatypeGYear;
				case "gMonth":
					return datatypeGMonth;
				case "gDay":
					return datatypeGDay;
				default:
					return null;
				}
			}
		}
	}
}
