using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XsdBoolean : XsdAnySimpleType
	{
		internal override XmlSchemaFacet.Facet AllowedFacets
		{
			get
			{
				return XsdAnySimpleType.booleanAllowedFacets;
			}
		}

		public override XmlTokenizedType TokenizedType
		{
			get
			{
				if (XmlSchemaUtil.StrictMsCompliant)
				{
					return XmlTokenizedType.None;
				}
				return XmlTokenizedType.CDATA;
			}
		}

		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.Boolean;
			}
		}

		public override Type ValueType
		{
			get
			{
				return typeof(bool);
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
				return true;
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
				return XsdOrderedFacet.Total;
			}
		}

		internal XsdBoolean()
		{
			WhitespaceValue = XsdWhitespaceFacet.Collapse;
		}

		public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
		{
			return ParseValueType(s, nameTable, nsmgr);
		}

		internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
		{
			return XmlConvert.ToBoolean(Normalize(s));
		}
	}
}
