using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XsdNotation : XsdAnySimpleType
	{
		internal override XmlSchemaFacet.Facet AllowedFacets
		{
			get
			{
				return XsdAnySimpleType.stringAllowedFacets;
			}
		}

		public override XmlTokenizedType TokenizedType
		{
			get
			{
				return XmlTokenizedType.NOTATION;
			}
		}

		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.Notation;
			}
		}

		public override Type ValueType
		{
			get
			{
				return typeof(string);
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
				return XsdOrderedFacet.False;
			}
		}

		internal XsdNotation()
		{
		}

		public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
		{
			return Normalize(s);
		}
	}
}
