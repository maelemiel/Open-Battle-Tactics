using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public abstract class XmlSchemaGroupBase : XmlSchemaParticle
	{
		private XmlSchemaObjectCollection compiledItems;

		[XmlIgnore]
		public abstract XmlSchemaObjectCollection Items { get; }

		internal XmlSchemaObjectCollection CompiledItems
		{
			get
			{
				return compiledItems;
			}
		}

		protected XmlSchemaGroupBase()
		{
			compiledItems = new XmlSchemaObjectCollection();
		}

		internal void CopyOptimizedItems(XmlSchemaGroupBase gb)
		{
			for (int i = 0; i < Items.Count; i++)
			{
				XmlSchemaParticle xmlSchemaParticle = Items[i] as XmlSchemaParticle;
				xmlSchemaParticle = xmlSchemaParticle.GetOptimizedParticle(false);
				if (xmlSchemaParticle != XmlSchemaParticle.Empty)
				{
					gb.Items.Add(xmlSchemaParticle);
					gb.CompiledItems.Add(xmlSchemaParticle);
				}
			}
		}

		internal override bool ParticleEquals(XmlSchemaParticle other)
		{
			XmlSchemaGroupBase xmlSchemaGroupBase = other as XmlSchemaGroupBase;
			if (xmlSchemaGroupBase == null)
			{
				return false;
			}
			if (GetType() != xmlSchemaGroupBase.GetType())
			{
				return false;
			}
			if (base.ValidatedMaxOccurs != xmlSchemaGroupBase.ValidatedMaxOccurs || base.ValidatedMinOccurs != xmlSchemaGroupBase.ValidatedMinOccurs)
			{
				return false;
			}
			if (CompiledItems.Count != xmlSchemaGroupBase.CompiledItems.Count)
			{
				return false;
			}
			for (int i = 0; i < CompiledItems.Count; i++)
			{
				XmlSchemaParticle xmlSchemaParticle = CompiledItems[i] as XmlSchemaParticle;
				XmlSchemaParticle other2 = xmlSchemaGroupBase.CompiledItems[i] as XmlSchemaParticle;
				if (!xmlSchemaParticle.ParticleEquals(other2))
				{
					return false;
				}
			}
			return true;
		}

		internal override void CheckRecursion(int depth, ValidationEventHandler h, XmlSchema schema)
		{
			foreach (XmlSchemaParticle item in Items)
			{
				item.CheckRecursion(depth, h, schema);
			}
		}

		internal bool ValidateNSRecurseCheckCardinality(XmlSchemaAny any, ValidationEventHandler h, XmlSchema schema, bool raiseError)
		{
			foreach (XmlSchemaParticle item in Items)
			{
				if (!item.ValidateDerivationByRestriction(any, h, schema, raiseError))
				{
					return false;
				}
			}
			return ValidateOccurenceRangeOK(any, h, schema, raiseError);
		}

		internal bool ValidateRecurse(XmlSchemaGroupBase baseGroup, ValidationEventHandler h, XmlSchema schema, bool raiseError)
		{
			return ValidateSeqRecurseMapSumCommon(baseGroup, h, schema, false, false, raiseError);
		}

		internal bool ValidateSeqRecurseMapSumCommon(XmlSchemaGroupBase baseGroup, ValidationEventHandler h, XmlSchema schema, bool isLax, bool isMapAndSum, bool raiseError)
		{
			int i = 0;
			int num = 0;
			decimal num2 = 0m;
			if (baseGroup.CompiledItems.Count == 0 && CompiledItems.Count > 0)
			{
				if (raiseError)
				{
					error(h, "Invalid particle derivation by restriction was found. base particle does not contain particles.");
				}
				return false;
			}
			for (int j = 0; j < CompiledItems.Count; j++)
			{
				XmlSchemaParticle xmlSchemaParticle = null;
				for (; CompiledItems.Count > i; i++)
				{
					xmlSchemaParticle = (XmlSchemaParticle)CompiledItems[i];
					if (xmlSchemaParticle != XmlSchemaParticle.Empty)
					{
						break;
					}
				}
				if (i >= CompiledItems.Count)
				{
					if (raiseError)
					{
						error(h, "Invalid particle derivation by restriction was found. Cannot be mapped to base particle.");
					}
					return false;
				}
				XmlSchemaParticle xmlSchemaParticle2 = null;
				while (baseGroup.CompiledItems.Count > num)
				{
					xmlSchemaParticle2 = (XmlSchemaParticle)baseGroup.CompiledItems[num];
					if (xmlSchemaParticle2 == XmlSchemaParticle.Empty && xmlSchemaParticle2.ValidatedMaxOccurs > 0m)
					{
						continue;
					}
					if (!xmlSchemaParticle.ValidateDerivationByRestriction(xmlSchemaParticle2, h, schema, false))
					{
						if (!isLax && !isMapAndSum && xmlSchemaParticle2.MinOccurs > num2 && !xmlSchemaParticle2.ValidateIsEmptiable())
						{
							if (raiseError)
							{
								error(h, "Invalid particle derivation by restriction was found. Invalid sub-particle derivation was found.");
							}
							return false;
						}
						num2 = 0m;
						num++;
						continue;
					}
					num2 += xmlSchemaParticle2.ValidatedMinOccurs;
					if (num2 >= baseGroup.ValidatedMaxOccurs)
					{
						num2 = 0m;
						num++;
					}
					i++;
					break;
				}
			}
			if (CompiledItems.Count > 0 && i != CompiledItems.Count)
			{
				if (raiseError)
				{
					error(h, "Invalid particle derivation by restriction was found. Extraneous derived particle was found.");
				}
				return false;
			}
			if (!isLax && !isMapAndSum)
			{
				if (num2 > 0m)
				{
					num++;
				}
				for (int k = num; k < baseGroup.CompiledItems.Count; k++)
				{
					XmlSchemaParticle xmlSchemaParticle3 = baseGroup.CompiledItems[k] as XmlSchemaParticle;
					if (!xmlSchemaParticle3.ValidateIsEmptiable())
					{
						if (raiseError)
						{
							error(h, "Invalid particle derivation by restriction was found. There is a base particle which does not have mapped derived particle and is not emptiable.");
						}
						return false;
					}
				}
			}
			return true;
		}
	}
}
