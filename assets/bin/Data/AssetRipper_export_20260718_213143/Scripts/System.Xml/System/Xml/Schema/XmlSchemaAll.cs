using System.Collections;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaAll : XmlSchemaGroupBase
	{
		private const string xmlname = "all";

		private XmlSchema schema;

		private XmlSchemaObjectCollection items;

		private bool emptiable;

		[XmlElement("element", typeof(XmlSchemaElement))]
		public override XmlSchemaObjectCollection Items
		{
			get
			{
				return items;
			}
		}

		internal bool Emptiable
		{
			get
			{
				return emptiable;
			}
		}

		public XmlSchemaAll()
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
			this.schema = schema;
			if (base.MaxOccurs != 1m)
			{
				error(h, "maxOccurs must be 1");
			}
			if (base.MinOccurs != 1m && base.MinOccurs != 0m)
			{
				error(h, "minOccurs must be 0 or 1");
			}
			XmlSchemaUtil.CompileID(base.Id, this, schema.IDCollection, h);
			CompileOccurence(h, schema);
			foreach (XmlSchemaObject item in Items)
			{
				XmlSchemaElement xmlSchemaElement = item as XmlSchemaElement;
				if (xmlSchemaElement != null)
				{
					if (xmlSchemaElement.ValidatedMaxOccurs != 1m && xmlSchemaElement.ValidatedMaxOccurs != 0m)
					{
						xmlSchemaElement.error(h, "The {max occurs} of all the elements of 'all' must be 0 or 1. ");
					}
					errorCount += xmlSchemaElement.Compile(h, schema);
				}
				else
				{
					error(h, "XmlSchemaAll can only contain Items of type Element");
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
			if (Items.Count == 1 && base.ValidatedMinOccurs == 1m && base.ValidatedMaxOccurs == 1m)
			{
				XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
				CopyInfo(xmlSchemaSequence);
				XmlSchemaParticle xmlSchemaParticle = (XmlSchemaParticle)Items[0];
				xmlSchemaParticle = xmlSchemaParticle.GetOptimizedParticle(false);
				if (xmlSchemaParticle == XmlSchemaParticle.Empty)
				{
					OptimizedParticle = xmlSchemaParticle;
				}
				else
				{
					xmlSchemaSequence.Items.Add(xmlSchemaParticle);
					xmlSchemaSequence.CompiledItems.Add(xmlSchemaParticle);
					xmlSchemaSequence.Compile(null, schema);
					OptimizedParticle = xmlSchemaSequence;
				}
				return OptimizedParticle;
			}
			XmlSchemaAll xmlSchemaAll = new XmlSchemaAll();
			CopyInfo(xmlSchemaAll);
			CopyOptimizedItems(xmlSchemaAll);
			OptimizedParticle = xmlSchemaAll;
			xmlSchemaAll.ComputeEmptiable();
			return OptimizedParticle;
		}

		internal override int Validate(ValidationEventHandler h, XmlSchema schema)
		{
			if (IsValidated(schema.CompilationId))
			{
				return errorCount;
			}
			if (!parentIsGroupDefinition && base.ValidatedMaxOccurs != 1m)
			{
				error(h, "-all- group is limited to be content of a model group, or that of a complex type with maxOccurs to be 1.");
			}
			base.CompiledItems.Clear();
			foreach (XmlSchemaParticle item in Items)
			{
				errorCount += item.Validate(h, schema);
				if (item.ValidatedMaxOccurs != 0m && item.ValidatedMaxOccurs != 1m)
				{
					error(h, "MaxOccurs of a particle inside -all- compositor must be either 0 or 1.");
				}
				base.CompiledItems.Add(item);
			}
			ComputeEmptiable();
			ValidationId = schema.ValidationId;
			return errorCount;
		}

		private void ComputeEmptiable()
		{
			emptiable = true;
			for (int i = 0; i < Items.Count; i++)
			{
				if (((XmlSchemaParticle)Items[i]).ValidatedMinOccurs > 0m)
				{
					emptiable = false;
					break;
				}
			}
		}

		internal override bool ValidateDerivationByRestriction(XmlSchemaParticle baseParticle, ValidationEventHandler h, XmlSchema schema, bool raiseError)
		{
			XmlSchemaAny xmlSchemaAny = baseParticle as XmlSchemaAny;
			XmlSchemaAll xmlSchemaAll = baseParticle as XmlSchemaAll;
			if (xmlSchemaAny != null)
			{
				return ValidateNSRecurseCheckCardinality(xmlSchemaAny, h, schema, raiseError);
			}
			if (xmlSchemaAll != null)
			{
				if (!ValidateOccurenceRangeOK(xmlSchemaAll, h, schema, raiseError))
				{
					return false;
				}
				return ValidateRecurse(xmlSchemaAll, h, schema, raiseError);
			}
			if (raiseError)
			{
				error(h, "Invalid -all- particle derivation was found.");
			}
			return false;
		}

		internal override decimal GetMinEffectiveTotalRange()
		{
			return GetMinEffectiveTotalRangeAllAndSequence();
		}

		internal override void ValidateUniqueParticleAttribution(XmlSchemaObjectTable qnames, ArrayList nsNames, ValidationEventHandler h, XmlSchema schema)
		{
			foreach (XmlSchemaElement item in Items)
			{
				item.ValidateUniqueParticleAttribution(qnames, nsNames, h, schema);
			}
		}

		internal override void ValidateUniqueTypeAttribution(XmlSchemaObjectTable labels, ValidationEventHandler h, XmlSchema schema)
		{
			foreach (XmlSchemaElement item in Items)
			{
				item.ValidateUniqueTypeAttribution(labels, h, schema);
			}
		}

		internal static XmlSchemaAll Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaAll xmlSchemaAll = new XmlSchemaAll();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "all")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaAll.Read, name=" + reader.Name, null);
				reader.SkipToEnd();
				return null;
			}
			xmlSchemaAll.LineNumber = reader.LineNumber;
			xmlSchemaAll.LinePosition = reader.LinePosition;
			xmlSchemaAll.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "id")
				{
					xmlSchemaAll.Id = reader.Value;
				}
				else if (reader.Name == "maxOccurs")
				{
					try
					{
						xmlSchemaAll.MaxOccursString = reader.Value;
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
						xmlSchemaAll.MinOccursString = reader.Value;
					}
					catch (Exception innerException2)
					{
						XmlSchemaObject.error(h, reader.Value + " is an invalid value for minOccurs", innerException2);
					}
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for all", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaAll);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaAll;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "all")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaAll.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaAll.Annotation = xmlSchemaAnnotation;
					}
				}
				else if (num <= 2 && reader.LocalName == "element")
				{
					num = 2;
					XmlSchemaElement xmlSchemaElement = XmlSchemaElement.Read(reader, h);
					if (xmlSchemaElement != null)
					{
						xmlSchemaAll.items.Add(xmlSchemaElement);
					}
				}
				else
				{
					reader.RaiseInvalidElementError();
				}
			}
			return xmlSchemaAll;
		}
	}
}
