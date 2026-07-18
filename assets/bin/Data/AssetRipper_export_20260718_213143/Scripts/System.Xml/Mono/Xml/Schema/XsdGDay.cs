using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XsdGDay : XsdAnySimpleType
	{
		internal override XmlSchemaFacet.Facet AllowedFacets
		{
			get
			{
				return XsdAnySimpleType.durationAllowedFacets;
			}
		}

		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.GDay;
			}
		}

		public override Type ValueType
		{
			get
			{
				return typeof(DateTime);
			}
		}

		internal XsdGDay()
		{
			WhitespaceValue = XsdWhitespaceFacet.Collapse;
		}

		public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
		{
			return ParseValueType(s, nameTable, nsmgr);
		}

		internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
		{
			return DateTime.ParseExact(Normalize(s), "---dd", null);
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
