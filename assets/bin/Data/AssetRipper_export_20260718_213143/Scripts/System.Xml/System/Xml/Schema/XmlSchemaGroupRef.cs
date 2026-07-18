using System.Collections;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaGroupRef : XmlSchemaParticle
	{
		private const string xmlname = "group";

		private XmlSchema schema;

		private XmlQualifiedName refName;

		private XmlSchemaGroup referencedGroup;

		private bool busy;

		[XmlAttribute("ref")]
		public XmlQualifiedName RefName
		{
			get
			{
				return refName;
			}
			set
			{
				refName = value;
			}
		}

		[XmlIgnore]
		public XmlSchemaGroupBase Particle
		{
			get
			{
				if (TargetGroup != null)
				{
					return TargetGroup.Particle;
				}
				return null;
			}
		}

		internal XmlSchemaGroup TargetGroup
		{
			get
			{
				if (referencedGroup != null && referencedGroup.IsCircularDefinition)
				{
					return null;
				}
				return referencedGroup;
			}
		}

		public XmlSchemaGroupRef()
		{
			refName = XmlQualifiedName.Empty;
		}

		internal override int Compile(ValidationEventHandler h, XmlSchema schema)
		{
			if (CompilationId == schema.CompilationId)
			{
				return 0;
			}
			this.schema = schema;
			XmlSchemaUtil.CompileID(base.Id, this, schema.IDCollection, h);
			CompileOccurence(h, schema);
			if (refName == null || refName.IsEmpty)
			{
				error(h, "ref must be present");
			}
			else if (!XmlSchemaUtil.CheckQName(RefName))
			{
				error(h, "RefName must be a valid XmlQualifiedName");
			}
			CompilationId = schema.CompilationId;
			return errorCount;
		}

		internal override int Validate(ValidationEventHandler h, XmlSchema schema)
		{
			if (IsValidated(schema.ValidationId))
			{
				return errorCount;
			}
			referencedGroup = schema.Groups[RefName] as XmlSchemaGroup;
			if (referencedGroup == null)
			{
				if (!schema.IsNamespaceAbsent(RefName.Namespace))
				{
					error(h, string.Concat("Referenced group ", RefName, " was not found in the corresponding schema."));
				}
			}
			else if (referencedGroup.Particle is XmlSchemaAll && base.ValidatedMaxOccurs != 1m)
			{
				error(h, "Group reference to -all- particle must have schema component {maxOccurs}=1.");
			}
			if (TargetGroup != null)
			{
				TargetGroup.Validate(h, schema);
			}
			ValidationId = schema.ValidationId;
			return errorCount;
		}

		internal override XmlSchemaParticle GetOptimizedParticle(bool isTop)
		{
			if (busy)
			{
				return XmlSchemaParticle.Empty;
			}
			if (OptimizedParticle != null)
			{
				return OptimizedParticle;
			}
			busy = true;
			XmlSchemaGroup xmlSchemaGroup = ((referencedGroup == null) ? (schema.Groups[RefName] as XmlSchemaGroup) : referencedGroup);
			if (xmlSchemaGroup != null && xmlSchemaGroup.Particle != null)
			{
				OptimizedParticle = xmlSchemaGroup.Particle;
				OptimizedParticle = OptimizedParticle.GetOptimizedParticle(isTop);
				if (OptimizedParticle != XmlSchemaParticle.Empty && (base.ValidatedMinOccurs != 1m || base.ValidatedMaxOccurs != 1m))
				{
					OptimizedParticle = OptimizedParticle.GetShallowClone();
					OptimizedParticle.OptimizedParticle = null;
					OptimizedParticle.MinOccurs = base.MinOccurs;
					OptimizedParticle.MaxOccurs = base.MaxOccurs;
					OptimizedParticle.CompileOccurence(null, null);
				}
			}
			else
			{
				OptimizedParticle = XmlSchemaParticle.Empty;
			}
			busy = false;
			return OptimizedParticle;
		}

		internal override bool ParticleEquals(XmlSchemaParticle other)
		{
			return GetOptimizedParticle(true).ParticleEquals(other);
		}

		internal override bool ValidateDerivationByRestriction(XmlSchemaParticle baseParticle, ValidationEventHandler h, XmlSchema schema, bool raiseError)
		{
			if (TargetGroup != null)
			{
				return TargetGroup.Particle.ValidateDerivationByRestriction(baseParticle, h, schema, raiseError);
			}
			return false;
		}

		internal override void CheckRecursion(int depth, ValidationEventHandler h, XmlSchema schema)
		{
			if (TargetGroup != null)
			{
				if (recursionDepth == -1)
				{
					recursionDepth = depth;
					TargetGroup.Particle.CheckRecursion(depth, h, schema);
					recursionDepth = -2;
				}
				else if (depth == recursionDepth)
				{
					throw new XmlSchemaException("Circular group reference was found.", this, null);
				}
			}
		}

		internal override void ValidateUniqueParticleAttribution(XmlSchemaObjectTable qnames, ArrayList nsNames, ValidationEventHandler h, XmlSchema schema)
		{
			if (TargetGroup != null)
			{
				TargetGroup.Particle.ValidateUniqueParticleAttribution(qnames, nsNames, h, schema);
			}
		}

		internal override void ValidateUniqueTypeAttribution(XmlSchemaObjectTable labels, ValidationEventHandler h, XmlSchema schema)
		{
			if (TargetGroup != null)
			{
				TargetGroup.Particle.ValidateUniqueTypeAttribution(labels, h, schema);
			}
		}

		internal static XmlSchemaGroupRef Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaGroupRef xmlSchemaGroupRef = new XmlSchemaGroupRef();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "group")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaGroup.Read, name=" + reader.Name, null);
				reader.Skip();
				return null;
			}
			xmlSchemaGroupRef.LineNumber = reader.LineNumber;
			xmlSchemaGroupRef.LinePosition = reader.LinePosition;
			xmlSchemaGroupRef.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "id")
				{
					xmlSchemaGroupRef.Id = reader.Value;
				}
				else if (reader.Name == "ref")
				{
					Exception innerEx;
					xmlSchemaGroupRef.refName = XmlSchemaUtil.ReadQNameAttribute(reader, out innerEx);
					if (innerEx != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is not a valid value for ref attribute", innerEx);
					}
				}
				else if (reader.Name == "maxOccurs")
				{
					try
					{
						xmlSchemaGroupRef.MaxOccursString = reader.Value;
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
						xmlSchemaGroupRef.MinOccursString = reader.Value;
					}
					catch (Exception innerException2)
					{
						XmlSchemaObject.error(h, reader.Value + " is an invalid value for minOccurs", innerException2);
					}
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for group", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaGroupRef);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaGroupRef;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "group")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaGroupRef.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaGroupRef.Annotation = xmlSchemaAnnotation;
					}
				}
				else
				{
					reader.RaiseInvalidElementError();
				}
			}
			return xmlSchemaGroupRef;
		}
	}
}
