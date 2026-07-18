using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XsdHexBinary : XsdAnySimpleType
	{
		internal override XmlSchemaFacet.Facet AllowedFacets
		{
			get
			{
				return XsdAnySimpleType.stringAllowedFacets;
			}
		}

		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.HexBinary;
			}
		}

		public override XmlTokenizedType TokenizedType
		{
			get
			{
				return XmlTokenizedType.None;
			}
		}

		public override Type ValueType
		{
			get
			{
				return typeof(byte[]);
			}
		}

		internal XsdHexBinary()
		{
			WhitespaceValue = XsdWhitespaceFacet.Collapse;
		}

		public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
		{
			return XmlConvert.FromBinHexString(Normalize(s));
		}

		internal override int Length(string s)
		{
			return s.Length / 2 + s.Length % 2;
		}

		internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
		{
			return new StringValueType(ParseValue(s, nameTable, nsmgr) as string);
		}
	}
}
