using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XdtDayTimeDuration : XsdDuration
	{
		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.DayTimeDuration;
			}
		}

		public override Type ValueType
		{
			get
			{
				return typeof(TimeSpan);
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

		internal XdtDayTimeDuration()
		{
		}

		public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
		{
			return ParseValueType(s, nameTable, nsmgr);
		}

		internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
		{
			return XmlConvert.ToTimeSpan(Normalize(s));
		}
	}
}
