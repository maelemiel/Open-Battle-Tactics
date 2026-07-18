using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XsdLong : XsdInteger
	{
		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.Long;
			}
		}

		public override Type ValueType
		{
			get
			{
				return typeof(long);
			}
		}

		public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
		{
			return ParseValueType(s, nameTable, nsmgr);
		}

		internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
		{
			return XmlConvert.ToInt64(Normalize(s));
		}

		internal override XsdOrdering Compare(object x, object y)
		{
			if (x is long && y is long)
			{
				if ((long)x == (long)y)
				{
					return XsdOrdering.Equal;
				}
				if ((long)x < (long)y)
				{
					return XsdOrdering.LessThan;
				}
				return XsdOrdering.GreaterThan;
			}
			return XsdOrdering.Indeterminate;
		}
	}
}
