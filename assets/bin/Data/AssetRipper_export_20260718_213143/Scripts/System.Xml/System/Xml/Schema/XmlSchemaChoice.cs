using System.Collections;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaChoice : XmlSchemaGroupBase
	{
		private const string xmlname = "choice";

		private XmlSchemaObjectCollection items;

		private decimal minEffectiveTotalRange = -1m;

		[XmlElement("sequence", typeof(XmlSchemaSequence))]
		[XmlElement("any", typeof(XmlSchemaAny))]
		[XmlElement("choice", typeof(XmlSchemaChoice))]
		[XmlElement("group", typeof(XmlSchemaGroupRef))]
		[XmlElement("element", typeof(XmlSchemaElement))]
		public override XmlSchemaObjectCollection Items
		{
			get
			{
				return items;
			}
		}

		public XmlSchemaChoice()
		{
			items = new XmlSchemaObjectCollection();
		}

		internal override void SetParent(XmlSchemaObject parent)
		{
			base.SetParent(parent);
			foreach (XmlSchemaObject item in Items)
			{
				item.SetParent(this);
			}
		}

		internal override int Compile(ValidationEventHandler h, XmlSchema schema)
		{
			if (CompilationId == schema.CompilationId)
			{
				return 0;
			}
			XmlSchemaUtil.CompileID(base.Id, this, schema.IDCollection, h);
			CompileOccurence(h, schema);
			if (Items.Count == 0)
			{
				warn(h, "Empty choice is unsatisfiable if minOccurs not equals to 0");
			}
			foreach (XmlSchemaObject item in Items)
			{
				if (item is XmlSchemaElement || item is XmlSchemaGroupRef || item is XmlSchemaChoice || item is XmlSchemaSequence || item is XmlSchemaAny)
				{
					errorCount += item.Compile(h, schema);
				}
				else
				{
					error(h, "Invalid schema object was specified in the particles of the choice model group.");
				}
			}
			CompilationId = schema.CompilationId;
			return errorCount;
		}

		internal override XmlSchemaParticle GetOptimizedParticle(bool isTop)
		{
			if (OptimizedParticle != null)
			{
				return OptimizedParticle;
			}
			if (Items.Count == 0 || base.ValidatedMaxOccurs == 0m)
			{
				OptimizedParticle = XmlSchemaParticle.Empty;
			}
			else if (!isTop && Items.Count == 1 && base.ValidatedMinOccurs == 1m && base.ValidatedMaxOccurs == 1m)
			{
				OptimizedParticle = ((XmlSchemaParticle)Items[0]).GetOptimizedParticle(false);
			}
			else
			{
				XmlSchemaChoice xmlSchemaChoice = new XmlSchemaChoice();
				CopyInfo(xmlSchemaChoice);
				for (int i = 0; i < Items.Count; i++)
				{
					XmlSchemaParticle xmlSchemaParticle = Items[i] as XmlSchemaParticle;
					xmlSchemaParticle = xmlSchemaParticle.GetOptimizedParticle(false);
					if (xmlSchemaParticle == XmlSchemaParticle.Empty)
					{
						continue;
					}
					if (xmlSchemaParticle is XmlSchemaChoice && xmlSchemaParticle.ValidatedMinOccurs == 1m && xmlSchemaParticle.ValidatedMaxOccurs == 1m)
					{
						XmlSchemaChoice xmlSchemaChoice2 = xmlSchemaParticle as XmlSchemaChoice;
						for (int j = 0; j < xmlSchemaChoice2.Items.Count; j++)
						{
							xmlSchemaChoice.Items.Add(xmlSchemaChoice2.Items[j]);
							xmlSchemaChoice.CompiledItems.Add(xmlSchemaChoice2.Items[j]);
						}
					}
					else
					{
						xmlSchemaChoice.Items.Add(xmlSchemaParticle);
						xmlSchemaChoice.CompiledItems.Add(xmlSchemaParticle);
					}
				}
				if (xmlSchemaChoice.Items.Count == 0)
				{
					OptimizedParticle = XmlSchemaParticle.Empty;
				}
				else
				{
					OptimizedParticle = xmlSchemaChoice;
				}
			}
			return OptimizedParticle;
		}

		internal override int Validate(ValidationEventHandler h, XmlSchema schema)
		{
			if (IsValidated(schema.CompilationId))
			{
				return errorCount;
			}
			base.CompiledItems.Clear();
			foreach (XmlSchemaParticle item in Items)
			{
				errorCount += item.Validate(h, schema);
				base.CompiledItems.Add(item);
			}
			ValidationId = schema.ValidationId;
			return errorCount;
		}

		internal override bool ValidateDerivationByRestriction(XmlSchemaParticle baseParticle, ValidationEventHandler h, XmlSchema schema, bool raiseError)
		{
			XmlSchemaAny xmlSchemaAny = baseParticle as XmlSchemaAny;
			if (xmlSchemaAny != null)
			{
				return ValidateNSRecurseCheckCardinality(xmlSchemaAny, h, schema, raiseError);
			}
			XmlSchemaChoice xmlSchemaChoice = baseParticle as XmlSchemaChoice;
			if (xmlSchemaChoice != null)
			{
				if (!ValidateOccurenceRangeOK(xmlSchemaChoice, h, schema, raiseError))
				{
					return false;
				}
				if (xmlSchemaChoice.ValidatedMinOccurs == 0m && xmlSchemaChoice.ValidatedMaxOccurs == 0m && base.ValidatedMinOccurs == 0m && base.ValidatedMaxOccurs == 0m)
				{
					return true;
				}
				return ValidateSeqRecurseMapSumCommon(xmlSchemaChoice, h, schema, true, false, raiseError);
			}
			if (raiseError)
			{
				error(h, "Invalid choice derivation by restriction was found.");
			}
			return false;
		}

		internal override decimal GetMinEffectiveTotalRange()
		{
			if (minEffectiveTotalRange >= 0m)
			{
				return minEffectiveTotalRange;
			}
			decimal num = 0m;
			if (Items.Count == 0)
			{
				num = 0m;
			}
			else
			{
				foreach (XmlSchemaParticle item in Items)
				{
					decimal num2 = item.GetMinEffectiveTotalRange();
					if (num > num2)
					{
						num = num2;
					}
				}
			}
			minEffectiveTotalRange = num;
			return num;
		}

		internal override void ValidateUniqueParticleAttribution(XmlSchemaObjectTable qnames, ArrayList nsNames, ValidationEventHandler h, XmlSchema schema)
		{
			foreach (XmlSchemaParticle item in Items)
			{
				item.ValidateUniqueParticleAttribution(qnames, nsNames, h, schema);
			}
		}

		internal override void ValidateUniqueTypeAttribution(XmlSchemaObjectTable labels, ValidationEventHandler h, XmlSchema schema)
		{
			foreach (XmlSchemaParticle item in Items)
			{
				item.ValidateUniqueTypeAttribution(labels, h, schema);
			}
		}

		internal static XmlSchemaChoice Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaChoice xmlSchemaChoice = new XmlSchemaChoice();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "choice")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaChoice.Read, name=" + reader.Name, null);
				reader.SkipToEnd();
				return null;
			}
			xmlSchemaChoice.LineNumber = reader.LineNumber;
			xmlSchemaChoice.LinePosition = reader.LinePosition;
			xmlSchemaChoice.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "id")
				{
					xmlSchemaChoice.Id = reader.Value;
				}
				else if (reader.Name == "maxOccurs")
				{
					try
					{
						xmlSchemaChoice.MaxOccursString = reader.Value;
					}
					catch (Exception innerException)
					{
						XmlSchemaObject.error(h, reader.Value + " is an invalid value for maxOccurs", innerException);
					}
				}
				else if (reader.Name == "minOccurs")
				{
					try
					{
						xmlSchemaChoice.MinOccursString = reader.Value;
					}
					catch (Exception innerException2)
					{
						XmlSchemaObject.error(h, reader.Value + " is an invalid value for minOccurs", innerException2);
					}
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for choice", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaChoice);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaChoice;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "choice")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaChoice.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaChoice.Annotation = xmlSchemaAnnotation;
					}
					continue;
				}
				if (num <= 2)
				{
					if (reader.LocalName == "element")
					{
						num = 2;
						XmlSchemaElement xmlSchemaElement = XmlSchemaElement.Read(reader, h);
						if (xmlSchemaElement != null)
						{
							xmlSchemaChoice.items.Add(xmlSchemaElement);
						}
						continue;
					}
					if (reader.LocalName == "group")
					{
						num = 2;
						XmlSchemaGroupRef xmlSchemaGroupRef = XmlSchemaGroupRef.Read(reader, h);
						if (xmlSchemaGroupRef != null)
						{
							xmlSchemaChoice.items.Add(xmlSchemaGroupRef);
						}
						continue;
					}
					if (reader.LocalName == "choice")
					{
						num = 2;
						XmlSchemaChoice xmlSchemaChoice2 = Read(reader, h);
						if (xmlSchemaChoice2 != null)
						{
							xmlSchemaChoice.items.Add(xmlSchemaChoice2);
						}
						continue;
					}
					if (reader.LocalName == "sequence")
					{
						num = 2;
						XmlSchemaSequence xmlSchemaSequence = XmlSchemaSequence.Read(reader, h);
						if (xmlSchemaSequence != null)
						{
							xmlSchemaChoice.items.Add(xmlSchemaSequence);
						}
						continue;
					}
					if (reader.LocalName == "any")
					{
						num = 2;
						XmlSchemaAny xmlSchemaAny = XmlSchemaAny.Read(reader, h);
						if (xmlSchemaAny != null)
						{
							xmlSchemaChoice.items.Add(xmlSchemaAny);
						}
						continue;
					}
				}
				reader.RaiseInvalidElementError();
			}
			return xmlSchemaChoice;
		}
	}
}
