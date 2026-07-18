using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XsdToken : XsdNormalizedString
	{
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
				return XmlTypeCode.Token;
			}
		}

		public override Type ValueType
		{
			get
			{
				return typeof(string);
			}
		}

		internal XsdToken()
		{
			WhitespaceValue = XsdWhitespaceFacet.Collapse;
		}
	}
}
