using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XdtAnyAtomicType : XsdAnySimpleType
	{
		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.AnyAtomicType;
			}
		}

		internal XdtAnyAtomicType()
		{
		}
	}
}
