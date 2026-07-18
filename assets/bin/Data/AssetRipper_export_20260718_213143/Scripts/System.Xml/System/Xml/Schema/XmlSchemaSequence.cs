using System.Collections;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaSequence : XmlSchemaGroupBase
	{
		private const string xmlname = "sequence";

		private XmlSchemaObjectCollection items;

		[XmlElement("sequence", typeof(XmlSchemaSequence))]
		[XmlElement("any", typeof(XmlSchemaAny))]
		[XmlElement("choice", typeof(XmlSchemaChoice))]
		[XmlElement("element", typeof(XmlSchemaElement))]
		[XmlElement("group", typeof(XmlSchemaGroupRef))]
		public override XmlSchemaObjectCollection Items
		{
			get
			{
				return items;
			}
		}

		public XmlSchemaSequence()
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
			foreach (XmlSchemaObject item in Items)
			{
				if (item is XmlSchemaElement || item is XmlSchemaGroupRef || item is XmlSchemaChoice || item is XmlSchemaSequence || item is XmlSchemaAny)
				{
					errorCount += item.Compile(h, schema);
				}
				else
				{
					error(h, "Invalid schema object was specified in the particles of the sequence model group.");
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
				return OptimizedParticle;
			}
			if (!isTop && base.ValidatedMinOccurs == 1m && base.ValidatedMaxOccurs == 1m && Items.Count == 1)
			{
				return ((XmlSchemaParticle)Items[0]).GetOptimizedParticle(false);
			}
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			CopyInfo(xmlSchemaSequence);
			for (int i = 0; i < Items.Count; i++)
			{
				XmlSchemaParticle xmlSchemaParticle = Items[i] as XmlSchemaParticle;
				xmlSchemaParticle = xmlSchemaParticle.GetOptimizedParticle(false);
				if (xmlSchemaParticle == XmlSchemaParticle.Empty)
				{
					continue;
				}
				if (xmlSchemaParticle is XmlSchemaSequence && xmlSchemaParticle.ValidatedMinOccurs == 1m && xmlSchemaParticle.ValidatedMaxOccurs == 1m)
				{
					XmlSchemaSequence xmlSchemaSequence2 = xmlSchemaParticle as XmlSchemaSequence;
					for (int j = 0; j < xmlSchemaSequence2.Items.Count; j++)
					{
						xmlSchemaSequence.Items.Add(xmlSchemaSequence2.Items[j]);
						xmlSchemaSequence.CompiledItems.Add(xmlSchemaSequence2.Items[j]);
					}
				}
				else
				{
					xmlSchemaSequence.Items.Add(xmlSchemaParticle);
					xmlSchemaSequence.CompiledItems.Add(xmlSchemaParticle);
				}
			}
			if (xmlSchemaSequence.Items.Count == 0)
			{
				OptimizedParticle = XmlSchemaParticle.Empty;
			}
			else
			{
				OptimizedParticle = xmlSchemaSequence;
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
			if (this == baseParticle)
			{
				return true;
			}
			XmlSchemaElement xmlSchemaElement = baseParticle as XmlSchemaElement;
			if (xmlSchemaElement != null)
			{
				if (raiseError)
				{
					error(h, "Invalid sequence paricle derivation.");
				}
				return false;
			}
			XmlSchemaSequence xmlSchemaSequence = baseParticle as XmlSchemaSequence;
			if (xmlSchemaSequence != null)
			{
				if (!ValidateOccurenceRangeOK(xmlSchemaSequence, h, schema, raiseError))
				{
					return false;
				}
				if (xmlSchemaSequence.ValidatedMinOccurs == 0m && xmlSchemaSequence.ValidatedMaxOccurs == 0m && base.ValidatedMinOccurs == 0m && base.ValidatedMaxOccurs == 0m)
				{
					return true;
				}
				return ValidateRecurse(xmlSchemaSequence, h, schema, raiseError);
			}
			XmlSchemaAll xmlSchemaAll = baseParticle as XmlSchemaAll;
			if (xmlSchemaAll != null)
			{
				XmlSchemaObjectCollection xmlSchemaObjectCollection = new XmlSchemaObjectCollection();
				for (int i = 0; i < Items.Count; i++)
				{
					XmlSchemaElement xmlSchemaElement2 = Items[i] as XmlSchemaElement;
					if (xmlSchemaElement2 == null)
					{
						if (raiseError)
						{
							error(h, "Invalid sequence particle derivation by restriction from all.");
						}
						return false;
					}
					foreach (XmlSchemaElement item in xmlSchemaAll.Items)
					{
						if (!(item.QualifiedName == xmlSchemaElement2.QualifiedName))
						{
							continue;
						}
						if (xmlSchemaObjectCollection.Contains(item))
						{
							if (raiseError)
							{
								error(h, "Base element particle is mapped to the derived element particle in a sequence two or more times.");
							}
							return false;
						}
						xmlSchemaObjectCollection.Add(item);
						if (!xmlSchemaElement2.ValidateDerivationByRestriction(item, h, schema, raiseError))
						{
							return false;
						}
					}
				}
				foreach (XmlSchemaElement item2 in xmlSchemaAll.Items)
				{
					if (!xmlSchemaObjectCollection.Contains(item2) && !item2.ValidateIsEmptiable())
					{
						if (raiseError)
						{
							error(h, "In base -all- particle, mapping-skipped base element which is not emptiable was found.");
						}
						return false;
					}
				}
				return true;
			}
			XmlSchemaAny xmlSchemaAny = baseParticle as XmlSchemaAny;
			if (xmlSchemaAny != null)
			{
				return ValidateNSRecurseCheckCardinality(xmlSchemaAny, h, schema, raiseError);
			}
			XmlSchemaChoice xmlSchemaChoice = baseParticle as XmlSchemaChoice;
			if (xmlSchemaChoice != null)
			{
				return ValidateSeqRecurseMapSumCommon(xmlSchemaChoice, h, schema, false, true, raiseError);
			}
			return true;
		}

		internal override decimal GetMinEffectiveTotalRange()
		{
			return GetMinEffectiveTotalRangeAllAndSequence();
		}

		internal override void ValidateUniqueParticleAttribution(XmlSchemaObjectTable qnames, ArrayList nsNames, ValidationEventHandler h, XmlSchema schema)
		{
			ValidateUPAOnHeadingOptionalComponents(qnames, nsNames, h, schema);
			ValidateUPAOnItems(qnames, nsNames, h, schema);
		}

		private void ValidateUPAOnHeadingOptionalComponents(XmlSchemaObjectTable qnames, ArrayList nsNames, ValidationEventHandler h, XmlSchema schema)
		{
			foreach (XmlSchemaParticle item in Items)
			{
				item.ValidateUniqueParticleAttribution(qnames, nsNames, h, schema);
				if (item.ValidatedMinOccurs != 0m)
				{
					break;
				}
			}
		}

		private void ValidateUPAOnItems(XmlSchemaObjectTable qnames, ArrayList nsNames, ValidationEventHandler h, XmlSchema schema)
		{
			XmlSchemaObjectTable xmlSchemaObjectTable = new XmlSchemaObjectTable();
			ArrayList arrayList = new ArrayList();
			XmlSchemaObjectTable xmlSchemaObjectTable2 = new XmlSchemaObjectTable();
			ArrayList arrayList2 = new ArrayList();
			for (int i = 0; i < Items.Count; i++)
			{
				XmlSchemaParticle xmlSchemaParticle = Items[i] as XmlSchemaParticle;
				xmlSchemaParticle.ValidateUniqueParticleAttribution(xmlSchemaObjectTable, arrayList, h, schema);
				if (xmlSchemaParticle.ValidatedMinOccurs == xmlSchemaParticle.ValidatedMaxOccurs)
				{
					xmlSchemaObjectTable.Clear();
					arrayList.Clear();
					continue;
				}
				if (xmlSchemaParticle.ValidatedMinOccurs != 0m)
				{
					foreach (XmlQualifiedName name3 in xmlSchemaObjectTable2.Names)
					{
						xmlSchemaObjectTable.Set(name3, null);
					}
					foreach (object item in arrayList2)
					{
						arrayList.Remove(item);
					}
				}
				foreach (XmlQualifiedName name4 in xmlSchemaObjectTable.Names)
				{
					xmlSchemaObjectTable2.Set(name4, xmlSchemaObjectTable[name4]);
				}
				arrayList2.Clear();
				arrayList2.AddRange(arrayList);
			}
		}

		internal override void ValidateUniqueTypeAttribution(XmlSchemaObjectTable labels, ValidationEventHandler h, XmlSchema schema)
		{
			foreach (XmlSchemaParticle item in Items)
			{
				item.ValidateUniqueTypeAttribution(labels, h, schema);
			}
		}

		internal static XmlSchemaSequence Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "sequence")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaSequence.Read, name=" + reader.Name, null);
				reader.Skip();
				return null;
			}
			xmlSchemaSequence.LineNumber = reader.LineNumber;
			xmlSchemaSequence.LinePosition = reader.LinePosition;
			xmlSchemaSequence.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "id")
				{
					xmlSchemaSequence.Id = reader.Value;
				}
				else if (reader.Name == "maxOccurs")
				{
					try
					{
						xmlSchemaSequence.MaxOccursString = reader.Value;
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
						xmlSchemaSequence.MinOccursString = reader.Value;
					}
					catch (Exception innerException2)
					{
						XmlSchemaObject.error(h, reader.Value + " is an invalid value for minOccurs", innerException2);
					}
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for sequence", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaSequence);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaSequence;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "sequence")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaSequence.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaSequence.Annotation = xmlSchemaAnnotation;
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
							xmlSchemaSequence.items.Add(xmlSchemaElement);
						}
						continue;
					}
					if (reader.LocalName == "group")
					{
						num = 2;
						XmlSchemaGroupRef xmlSchemaGroupRef = XmlSchemaGroupRef.Read(reader, h);
						if (xmlSchemaGroupRef != null)
						{
							xmlSchemaSequence.items.Add(xmlSchemaGroupRef);
						}
						continue;
					}
					if (reader.LocalName == "choice")
					{
						num = 2;
						XmlSchemaChoice xmlSchemaChoice = XmlSchemaChoice.Read(reader, h);
						if (xmlSchemaChoice != null)
						{
							xmlSchemaSequence.items.Add(xmlSchemaChoice);
						}
						continue;
					}
					if (reader.LocalName == "sequence")
					{
						num = 2;
						XmlSchemaSequence xmlSchemaSequence2 = Read(reader, h);
						if (xmlSchemaSequence2 != null)
						{
							xmlSchemaSequence.items.Add(xmlSchemaSequence2);
						}
						continue;
					}
					if (reader.LocalName == "any")
					{
						num = 2;
						XmlSchemaAny xmlSchemaAny = XmlSchemaAny.Read(reader, h);
						if (xmlSchemaAny != null)
						{
							xmlSchemaSequence.items.Add(xmlSchemaAny);
						}
						continue;
					}
				}
				reader.RaiseInvalidElementError();
			}
			return xmlSchemaSequence;
		}
	}
}
