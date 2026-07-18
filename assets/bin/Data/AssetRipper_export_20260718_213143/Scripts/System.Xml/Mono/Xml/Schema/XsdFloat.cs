using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XsdFloat : XsdAnySimpleType
	{
		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.Float;
			}
		}

		internal override XmlSchemaFacet.Facet AllowedFacets
		{
			get
			{
				return XsdAnySimpleType.durationAllowedFacets;
			}
		}

		public override bool Bounded
		{
			get
			{
				return true;
			}
		}

		public override bool Finite
		{
			get
			{
				return true;
			}
		}

		public override bool Numeric
		{
			get
			{
				return true;
			}
		}

		public override XsdOrderedFacet Ordered
		{
			get
			{
				return XsdOrderedFacet.Total;
			}
		}

		public override Type ValueType
		{
			get
			{
				return typeof(float);
			}
		}

		internal XsdFloat()
		{
			WhitespaceValue = XsdWhitespaceFacet.Collapse;
		}

		public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
		{
			return ParseValueType(s, nameTable, nsmgr);
		}

		internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
		{
			return XmlConvert.ToSingle(Normalize(s));
		}

		internal override XsdOrdering Compare(object x, object y)
		{
			if (x is float && y is float)
			{
				if ((float)x == (float)y)
				{
					return XsdOrdering.Equal;
				}
				if ((float)x < (float)y)
				{
					return XsdOrdering.LessThan;
				}
				return XsdOrdering.GreaterThan;
			}
			return XsdOrdering.Indeterminate;
		}
	}
}
