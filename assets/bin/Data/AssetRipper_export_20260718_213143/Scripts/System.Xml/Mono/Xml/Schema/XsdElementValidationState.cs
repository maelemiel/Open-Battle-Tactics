using System.Collections;
using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XsdElementValidationState : XsdValidationState
	{
		private readonly XmlSchemaElement element;

		private string Name
		{
			get
			{
				return element.QualifiedName.Name;
			}
		}

		private string NS
		{
			get
			{
				return element.QualifiedName.Namespace;
			}
		}

		public XsdElementValidationState(XmlSchemaElement element, XsdParticleStateManager manager)
			: base(manager)
		{
			this.element = element;
		}

		public override void GetExpectedParticles(ArrayList al)
		{
			XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)MemberwiseClone();
			decimal num = element.ValidatedMinOccurs - (decimal)base.Occured;
			xmlSchemaElement.MinOccurs = ((!(num > 0m)) ? 0m : num);
			if (element.ValidatedMaxOccurs == decimal.MaxValue)
			{
				xmlSchemaElement.MaxOccursString = "unbounded";
			}
			else
			{
				xmlSchemaElement.MaxOccurs = element.ValidatedMaxOccurs - (decimal)base.Occured;
			}
			al.Add(xmlSchemaElement);
		}

		public override XsdValidationState EvaluateStartElement(string name, string ns)
		{
			if (Name == name && NS == ns && !element.IsAbstract)
			{
				return CheckOccurence(element);
			}
			for (int i = 0; i < element.SubstitutingElements.Count; i++)
			{
				XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)element.SubstitutingElements[i];
				if (xmlSchemaElement.QualifiedName.Name == name && xmlSchemaElement.QualifiedName.Namespace == ns)
				{
					return CheckOccurence(xmlSchemaElement);
				}
			}
			return XsdValidationState.Invalid;
		}

		private XsdValidationState CheckOccurence(XmlSchemaElement maybeSubstituted)
		{
			base.OccuredInternal++;
			base.Manager.CurrentElement = maybeSubstituted;
			if ((decimal)base.Occured > element.ValidatedMaxOccurs)
			{
				return XsdValidationState.Invalid;
			}
			if ((decimal)base.Occured == element.ValidatedMaxOccurs)
			{
				return base.Manager.Create(XmlSchemaParticle.Empty);
			}
			return this;
		}

		public override bool EvaluateEndElement()
		{
			return EvaluateIsEmptiable();
		}

		internal override bool EvaluateIsEmptiable()
		{
			return element.ValidatedMinOccurs <= (decimal)base.Occured && element.ValidatedMaxOccurs >= (decimal)base.Occured;
		}
	}
}
