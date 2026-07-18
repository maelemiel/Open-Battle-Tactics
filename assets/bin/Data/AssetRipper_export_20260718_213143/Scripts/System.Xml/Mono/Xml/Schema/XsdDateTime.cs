using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XsdDateTime : XsdAnySimpleType
	{
		internal override XmlSchemaFacet.Facet AllowedFacets
		{
			get
			{
				return XsdAnySimpleType.durationAllowedFacets;
			}
		}

		public override XmlTokenizedType TokenizedType
		{
			get
			{
				return XmlTokenizedType.CDATA;
			}
		}

		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.DateTime;
			}
		}

		public override Type ValueType
		{
			get
			{
				return typeof(DateTime);
			}
		}

		public override bool Bounded
		{
			get
			{
				return false;
			}
		}

		public override bool Finite
		{
			get
			{
				return false;
			}
		}

		public override bool Numeric
		{
			get
			{
				return false;
			}
		}

		public override XsdOrderedFacet Ordered
		{
			get
			{
				return XsdOrderedFacet.Partial;
			}
		}

		internal XsdDateTime()
		{
			WhitespaceValue = XsdWhitespaceFacet.Collapse;
		}

		public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
		{
			return ParseValueType(s, nameTable, nsmgr);
		}

		internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
		{
			return XmlConvert.ToDateTime(Normalize(s));
		}

		internal override XsdOrdering Compare(object x, object y)
		{
			if (x is DateTime && y is DateTime)
			{
				int num = DateTime.Compare((DateTime)x, (DateTime)y);
				if (num < 0)
				{
					return XsdOrdering.LessThan;
				}
				if (num > 0)
				{
					return XsdOrdering.GreaterThan;
				}
				return XsdOrdering.Equal;
			}
			return XsdOrdering.Indeterminate;
		}
	}
}
