using System.Collections;
using System.Globalization;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public abstract class XmlSchemaParticle : XmlSchemaAnnotated
	{
		internal class EmptyParticle : XmlSchemaParticle
		{
			internal EmptyParticle()
			{
			}

			internal override XmlSchemaParticle GetOptimizedParticle(bool isTop)
			{
				return this;
			}

			internal override bool ParticleEquals(XmlSchemaParticle other)
			{
				return other == this || other == Empty;
			}

			internal override bool ValidateDerivationByRestriction(XmlSchemaParticle baseParticle, ValidationEventHandler h, XmlSchema schema, bool raiseError)
			{
				return true;
			}

			internal override void CheckRecursion(int depth, ValidationEventHandler h, XmlSchema schema)
			{
			}

			internal override void ValidateUniqueParticleAttribution(XmlSchemaObjectTable qnames, ArrayList nsNames, ValidationEventHandler h, XmlSchema schema)
			{
			}

			internal override void ValidateUniqueTypeAttribution(XmlSchemaObjectTable labels, ValidationEventHandler h, XmlSchema schema)
			{
			}
		}

		private decimal minOccurs;

		private decimal maxOccurs;

		private string minstr;

		private string maxstr;

		private static XmlSchemaParticle empty;

		private decimal validatedMinOccurs = 1m;

		private decimal validatedMaxOccurs = 1m;

		internal int recursionDepth = -1;

		private decimal minEffectiveTotalRange = -1m;

		internal bool parentIsGroupDefinition;

		internal XmlSchemaParticle OptimizedParticle;

		internal static XmlSchemaParticle Empty
		{
			get
			{
				if (empty == null)
				{
					empty = new EmptyParticle();
				}
				return empty;
			}
		}

		[XmlAttribute("minOccurs")]
		public string MinOccursString
		{
			get
			{
				return minstr;
			}
			set
			{
				if (value == null)
				{
					minOccurs = 1m;
					minstr = value;
					return;
				}
				decimal num = decimal.Parse(value, CultureInfo.InvariantCulture);
				if (num >= 0m && num == decimal.Truncate(num))
				{
					minOccurs = num;
					minstr = num.ToString(CultureInfo.InvariantCulture);
					return;
				}
				throw new XmlSchemaException("MinOccursString must be a non-negative number", null);
			}
		}

		[XmlAttribute("maxOccurs")]
		public string MaxOccursString
		{
			get
			{
				return maxstr;
			}
			set
			{
				if (value == "unbounded")
				{
					maxstr = value;
					maxOccurs = decimal.MaxValue;
					return;
				}
				decimal num = decimal.Parse(value, CultureInfo.InvariantCulture);
				if (num >= 0m && num == decimal.Truncate(num))
				{
					maxOccurs = num;
					maxstr = num.ToString(CultureInfo.InvariantCulture);
					if (num == 0m && minstr == null)
					{
						minOccurs = 0m;
					}
					return;
				}
				throw new XmlSchemaException("MaxOccurs must be a non-negative integer", null);
			}
		}

		[XmlIgnore]
		public decimal MinOccurs
		{
			get
			{
				return minOccurs;
			}
			set
			{
				MinOccursString = value.ToString(CultureInfo.InvariantCulture);
			}
		}

		[XmlIgnore]
		public decimal MaxOccurs
		{
			get
			{
				return maxOccurs;
			}
			set
			{
				if (value == decimal.MaxValue)
				{
					MaxOccursString = "unbounded";
				}
				else
				{
					MaxOccursString = value.ToString(CultureInfo.InvariantCulture);
				}
			}
		}

		internal decimal ValidatedMinOccurs
		{
			get
			{
				return validatedMinOccurs;
			}
		}

		internal decimal ValidatedMaxOccurs
		{
			get
			{
				return validatedMaxOccurs;
			}
		}

		protected XmlSchemaParticle()
		{
			minOccurs = 1m;
			maxOccurs = 1m;
		}

		internal virtual XmlSchemaParticle GetOptimizedParticle(bool isTop)
		{
			return null;
		}

		internal XmlSchemaParticle GetShallowClone()
		{
			return (XmlSchemaParticle)MemberwiseClone();
		}

		internal void CompileOccurence(ValidationEventHandler h, XmlSchema schema)
		{
			if (MinOccurs > MaxOccurs && (!(MaxOccurs == 0m) || MinOccursString != null))
			{
				error(h, "minOccurs must be less than or equal to maxOccurs");
				return;
			}
			if (MaxOccursString == "unbounded")
			{
				validatedMaxOccurs = decimal.MaxValue;
			}
			else
			{
				validatedMaxOccurs = maxOccurs;
			}
			if (validatedMaxOccurs == 0m)
			{
				validatedMinOccurs = 0m;
			}
			else
			{
				validatedMinOccurs = minOccurs;
			}
		}

		internal override void CopyInfo(XmlSchemaParticle obj)
		{
			base.CopyInfo(obj);
			if (MaxOccursString == "unbounded")
			{
				obj.maxOccurs = (obj.validatedMaxOccurs = decimal.MaxValue);
			}
			else
			{
				obj.maxOccurs = (obj.validatedMaxOccurs = ValidatedMaxOccurs);
			}
			if (MaxOccurs == 0m)
			{
				obj.minOccurs = (obj.validatedMinOccurs = 0m);
			}
			else
			{
				obj.minOccurs = (obj.validatedMinOccurs = ValidatedMinOccurs);
			}
			if (MinOccursString != null)
			{
				obj.MinOccursString = MinOccursString;
			}
			if (MaxOccursString != null)
			{
				obj.MaxOccursString = MaxOccursString;
			}
		}

		internal virtual bool ValidateOccurenceRangeOK(XmlSchemaParticle other, ValidationEventHandler h, XmlSchema schema, bool raiseError)
		{
			if (ValidatedMinOccurs < other.ValidatedMinOccurs || (other.ValidatedMaxOccurs != decimal.MaxValue && ValidatedMaxOccurs > other.ValidatedMaxOccurs))
			{
				if (raiseError)
				{
					error(h, "Invalid derivation occurence range was found.");
				}
				return false;
			}
			return true;
		}

		internal virtual decimal GetMinEffectiveTotalRange()
		{
			return ValidatedMinOccurs;
		}

		internal decimal GetMinEffectiveTotalRangeAllAndSequence()
		{
			if (minEffectiveTotalRange >= 0m)
			{
				return minEffectiveTotalRange;
			}
			decimal result = 0m;
			XmlSchemaObjectCollection xmlSchemaObjectCollection = null;
			xmlSchemaObjectCollection = ((!(this is XmlSchemaAll)) ? ((XmlSchemaSequence)this).Items : ((XmlSchemaAll)this).Items);
			foreach (XmlSchemaParticle item in xmlSchemaObjectCollection)
			{
				result += item.GetMinEffectiveTotalRange();
			}
			minEffectiveTotalRange = result;
			return result;
		}

		internal virtual bool ValidateIsEmptiable()
		{
			return validatedMinOccurs == 0m || GetMinEffectiveTotalRange() == 0m;
		}

		internal virtual bool ValidateDerivationByRestriction(XmlSchemaParticle baseParticle, ValidationEventHandler h, XmlSchema schema, bool raiseError)
		{
			return false;
		}

		internal virtual void ValidateUniqueParticleAttribution(XmlSchemaObjectTable qnames, ArrayList nsNames, ValidationEventHandler h, XmlSchema schema)
		{
		}

		internal virtual void ValidateUniqueTypeAttribution(XmlSchemaObjectTable labels, ValidationEventHandler h, XmlSchema schema)
		{
		}

		internal virtual void CheckRecursion(int depth, ValidationEventHandler h, XmlSchema schema)
		{
		}

		internal virtual bool ParticleEquals(XmlSchemaParticle other)
		{
			return false;
		}
	}
}
