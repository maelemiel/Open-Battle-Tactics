using System.Collections;
using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XsdChoiceValidationState : XsdValidationState
	{
		private readonly XmlSchemaChoice choice;

		private bool emptiable;

		private bool emptiableComputed;

		public XsdChoiceValidationState(XmlSchemaChoice choice, XsdParticleStateManager manager)
			: base(manager)
		{
			this.choice = choice;
		}

		public override void GetExpectedParticles(ArrayList al)
		{
			if (!((decimal)base.Occured < choice.ValidatedMaxOccurs))
			{
				return;
			}
			foreach (XmlSchemaParticle compiledItem in choice.CompiledItems)
			{
				al.Add(compiledItem);
			}
		}

		public override XsdValidationState EvaluateStartElement(string localName, string ns)
		{
			emptiableComputed = false;
			bool flag = true;
			for (int i = 0; i < choice.CompiledItems.Count; i++)
			{
				XmlSchemaParticle xsobj = (XmlSchemaParticle)choice.CompiledItems[i];
				XsdValidationState xsdValidationState = base.Manager.Create(xsobj);
				XsdValidationState xsdValidationState2 = xsdValidationState.EvaluateStartElement(localName, ns);
				if (xsdValidationState2 != XsdValidationState.Invalid)
				{
					base.OccuredInternal++;
					if ((decimal)base.Occured > choice.ValidatedMaxOccurs)
					{
						return XsdValidationState.Invalid;
					}
					if ((decimal)base.Occured == choice.ValidatedMaxOccurs)
					{
						return xsdValidationState2;
					}
					return base.Manager.MakeSequence(xsdValidationState2, this);
				}
				if (!emptiableComputed)
				{
					flag &= xsdValidationState.EvaluateIsEmptiable();
				}
			}
			if (!emptiableComputed)
			{
				if (flag)
				{
					emptiable = true;
				}
				if (!emptiable)
				{
					emptiable = choice.ValidatedMinOccurs <= (decimal)base.Occured;
				}
				emptiableComputed = true;
			}
			return XsdValidationState.Invalid;
		}

		public override bool EvaluateEndElement()
		{
			emptiableComputed = false;
			if (choice.ValidatedMinOccurs > (decimal)(base.Occured + 1))
			{
				return false;
			}
			if (choice.ValidatedMinOccurs <= (decimal)base.Occured)
			{
				return true;
			}
			for (int i = 0; i < choice.CompiledItems.Count; i++)
			{
				XmlSchemaParticle xsobj = (XmlSchemaParticle)choice.CompiledItems[i];
				if (base.Manager.Create(xsobj).EvaluateIsEmptiable())
				{
					return true;
				}
			}
			return false;
		}

		internal override bool EvaluateIsEmptiable()
		{
			if (emptiableComputed)
			{
				return emptiable;
			}
			if (choice.ValidatedMaxOccurs < (decimal)base.Occured)
			{
				return false;
			}
			if (choice.ValidatedMinOccurs > (decimal)(base.Occured + 1))
			{
				return false;
			}
			for (int i = base.Occured; (decimal)i < choice.ValidatedMinOccurs; i++)
			{
				bool flag = false;
				for (int j = 0; j < choice.CompiledItems.Count; j++)
				{
					XmlSchemaParticle xsobj = (XmlSchemaParticle)choice.CompiledItems[j];
					if (base.Manager.Create(xsobj).EvaluateIsEmptiable())
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}
	}
}
