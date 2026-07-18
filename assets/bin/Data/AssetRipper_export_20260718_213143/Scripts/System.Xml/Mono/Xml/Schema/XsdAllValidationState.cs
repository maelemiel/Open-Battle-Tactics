using System.Collections;
using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XsdAllValidationState : XsdValidationState
	{
		private readonly XmlSchemaAll all;

		private ArrayList consumed = new ArrayList();

		public XsdAllValidationState(XmlSchemaAll all, XsdParticleStateManager manager)
			: base(manager)
		{
			this.all = all;
		}

		public override void GetExpectedParticles(ArrayList al)
		{
			foreach (XmlSchemaParticle compiledItem in all.CompiledItems)
			{
				if (!consumed.Contains(compiledItem))
				{
					al.Add(compiledItem);
				}
			}
		}

		public override XsdValidationState EvaluateStartElement(string localName, string ns)
		{
			if (all.CompiledItems.Count == 0)
			{
				return XsdValidationState.Invalid;
			}
			for (int i = 0; i < all.CompiledItems.Count; i++)
			{
				XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)all.CompiledItems[i];
				if (xmlSchemaElement.QualifiedName.Name == localName && xmlSchemaElement.QualifiedName.Namespace == ns)
				{
					if (consumed.Contains(xmlSchemaElement))
					{
						return XsdValidationState.Invalid;
					}
					consumed.Add(xmlSchemaElement);
					base.Manager.CurrentElement = xmlSchemaElement;
					base.OccuredInternal = 1;
					return this;
				}
			}
			return XsdValidationState.Invalid;
		}

		public override bool EvaluateEndElement()
		{
			if (all.Emptiable || all.ValidatedMinOccurs == 0m)
			{
				return true;
			}
			if (all.ValidatedMinOccurs > 0m && consumed.Count == 0)
			{
				return false;
			}
			if (all.CompiledItems.Count == consumed.Count)
			{
				return true;
			}
			for (int i = 0; i < all.CompiledItems.Count; i++)
			{
				XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)all.CompiledItems[i];
				if (xmlSchemaElement.ValidatedMinOccurs > 0m && !consumed.Contains(xmlSchemaElement))
				{
					return false;
				}
			}
			return true;
		}

		internal override bool EvaluateIsEmptiable()
		{
			if (all.Emptiable || all.ValidatedMinOccurs == 0m)
			{
				return true;
			}
			for (int i = 0; i < all.CompiledItems.Count; i++)
			{
				XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)all.CompiledItems[i];
				if (xmlSchemaElement.ValidatedMinOccurs > 0m && !consumed.Contains(xmlSchemaElement))
				{
					return false;
				}
			}
			return true;
		}
	}
}
