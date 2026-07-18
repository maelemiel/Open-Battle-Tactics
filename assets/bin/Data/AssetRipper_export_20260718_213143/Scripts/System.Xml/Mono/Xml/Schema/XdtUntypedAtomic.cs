using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XdtUntypedAtomic : XdtAnyAtomicType
	{
		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.UntypedAtomic;
			}
		}

		internal XdtUntypedAtomic()
		{
		}
	}
}
