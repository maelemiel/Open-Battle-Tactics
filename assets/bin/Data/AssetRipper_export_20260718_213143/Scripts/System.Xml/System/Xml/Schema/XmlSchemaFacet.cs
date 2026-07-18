using System.ComponentModel;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public abstract class XmlSchemaFacet : XmlSchemaAnnotated
	{
		[Flags]
		protected internal enum Facet
		{
			None = 0,
			length = 1,
			minLength = 2,
			maxLength = 4,
			pattern = 8,
			enumeration = 0x10,
			whiteSpace = 0x20,
			maxInclusive = 0x40,
			maxExclusive = 0x80,
			minExclusive = 0x100,
			minInclusive = 0x200,
			totalDigits = 0x400,
			fractionDigits = 0x800
		}

		internal static readonly Facet AllFacets = Facet.length | Facet.minLength | Facet.maxLength | Facet.pattern | Facet.enumeration | Facet.whiteSpace | Facet.maxInclusive | Facet.maxExclusive | Facet.minExclusive | Facet.minInclusive | Facet.totalDigits | Facet.fractionDigits;

		private bool isFixed;

		private string val;

		internal virtual Facet ThisFacet
		{
			get
			{
				return Facet.None;
			}
		}

		[XmlAttribute("value")]
		public string Value
		{
			get
			{
				return val;
			}
			set
			{
				val = value;
			}
		}

		[DefaultValue(false)]
		[XmlAttribute("fixed")]
		public virtual bool IsFixed
		{
			get
			{
				return isFixed;
			}
			set
			{
				isFixed = value;
			}
		}
	}
}
