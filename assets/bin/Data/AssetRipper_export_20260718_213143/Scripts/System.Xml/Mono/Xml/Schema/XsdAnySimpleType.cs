using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XsdAnySimpleType : XmlSchemaDatatype
	{
		private static XsdAnySimpleType instance;

		private static readonly char[] whitespaceArray;

		internal static readonly XmlSchemaFacet.Facet booleanAllowedFacets;

		internal static readonly XmlSchemaFacet.Facet decimalAllowedFacets;

		internal static readonly XmlSchemaFacet.Facet durationAllowedFacets;

		internal static readonly XmlSchemaFacet.Facet stringAllowedFacets;

		public static XsdAnySimpleType Instance
		{
			get
			{
				return instance;
			}
		}

		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.AnyAtomicType;
			}
		}

		public virtual bool Bounded
		{
			get
			{
				return false;
			}
		}

		public virtual bool Finite
		{
			get
			{
				return false;
			}
		}

		public virtual bool Numeric
		{
			get
			{
				return false;
			}
		}

		public virtual XsdOrderedFacet Ordered
		{
			get
			{
				return XsdOrderedFacet.False;
			}
		}

		public override Type ValueType
		{
			get
			{
				if (XmlSchemaUtil.StrictMsCompliant)
				{
					return typeof(string);
				}
				return typeof(object);
			}
		}

		public override XmlTokenizedType TokenizedType
		{
			get
			{
				return XmlTokenizedType.None;
			}
		}

		internal virtual XmlSchemaFacet.Facet AllowedFacets
		{
			get
			{
				return XmlSchemaFacet.AllFacets;
			}
		}

		protected XsdAnySimpleType()
		{
		}

		static XsdAnySimpleType()
		{
			whitespaceArray = new char[1] { ' ' };
			booleanAllowedFacets = XmlSchemaFacet.Facet.pattern | XmlSchemaFacet.Facet.whiteSpace;
			decimalAllowedFacets = XmlSchemaFacet.Facet.pattern | XmlSchemaFacet.Facet.enumeration | XmlSchemaFacet.Facet.whiteSpace | XmlSchemaFacet.Facet.maxInclusive | XmlSchemaFacet.Facet.maxExclusive | XmlSchemaFacet.Facet.minExclusive | XmlSchemaFacet.Facet.minInclusive | XmlSchemaFacet.Facet.totalDigits | XmlSchemaFacet.Facet.fractionDigits;
			durationAllowedFacets = XmlSchemaFacet.Facet.pattern | XmlSchemaFacet.Facet.enumeration | XmlSchemaFacet.Facet.whiteSpace | XmlSchemaFacet.Facet.maxInclusive | XmlSchemaFacet.Facet.maxExclusive | XmlSchemaFacet.Facet.minExclusive | XmlSchemaFacet.Facet.minInclusive;
			stringAllowedFacets = XmlSchemaFacet.Facet.length | XmlSchemaFacet.Facet.minLength | XmlSchemaFacet.Facet.maxLength | XmlSchemaFacet.Facet.pattern | XmlSchemaFacet.Facet.enumeration | XmlSchemaFacet.Facet.whiteSpace;
			instance = new XsdAnySimpleType();
		}

		public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
		{
			return Normalize(s);
		}

		internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
		{
			return new StringValueType(Normalize(s));
		}

		internal string[] ParseListValue(string s, XmlNameTable nameTable)
		{
			return Normalize(s, XsdWhitespaceFacet.Collapse).Split(whitespaceArray);
		}

		internal bool AllowsFacet(XmlSchemaFacet xsf)
		{
			return (AllowedFacets & xsf.ThisFacet) != 0;
		}

		internal virtual XsdOrdering Compare(object x, object y)
		{
			return XsdOrdering.Indeterminate;
		}

		internal virtual int Length(string s)
		{
			return s.Length;
		}
	}
}
