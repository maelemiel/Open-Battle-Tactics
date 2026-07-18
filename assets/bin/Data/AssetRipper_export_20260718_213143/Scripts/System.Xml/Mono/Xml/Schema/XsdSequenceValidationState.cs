using System.Collections;
using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XsdSequenceValidationState : XsdValidationState
	{
		private readonly XmlSchemaSequence seq;

		private int current;

		private XsdValidationState currentAutomata;

		private bool emptiable;

		public XsdSequenceValidationState(XmlSchemaSequence sequence, XsdParticleStateManager manager)
			: base(manager)
		{
			seq = sequence;
			current = -1;
		}

		public override void GetExpectedParticles(ArrayList al)
		{
			if (currentAutomata == null)
			{
				foreach (XmlSchemaParticle compiledItem in seq.CompiledItems)
				{
					al.Add(compiledItem);
					if (!compiledItem.ValidateIsEmptiable())
					{
						break;
					}
				}
				return;
			}
			if (currentAutomata != null)
			{
				currentAutomata.GetExpectedParticles(al);
				if (!currentAutomata.EvaluateIsEmptiable())
				{
					return;
				}
				for (int i = current + 1; i < seq.CompiledItems.Count; i++)
				{
					XmlSchemaParticle xmlSchemaParticle2 = seq.CompiledItems[i] as XmlSchemaParticle;
					al.Add(xmlSchemaParticle2);
					if (!xmlSchemaParticle2.ValidateIsEmptiable())
					{
						break;
					}
				}
			}
			if (!((decimal)(base.Occured + 1) == seq.ValidatedMaxOccurs))
			{
				for (int j = 0; j <= current; j++)
				{
					al.Add(seq.CompiledItems[j]);
				}
			}
		}

		public override XsdValidationState EvaluateStartElement(string name, string ns)
		{
			if (seq.CompiledItems.Count == 0)
			{
				return XsdValidationState.Invalid;
			}
			int num = ((current >= 0) ? current : 0);
			XsdValidationState xsdValidationState = currentAutomata;
			bool flag = false;
			while (true)
			{
				if (xsdValidationState == null)
				{
					xsdValidationState = base.Manager.Create(seq.CompiledItems[num] as XmlSchemaParticle);
					flag = true;
				}
				if (xsdValidationState is XsdEmptyValidationState && seq.CompiledItems.Count == num + 1 && (decimal)base.Occured == seq.ValidatedMaxOccurs)
				{
					return XsdValidationState.Invalid;
				}
				XsdValidationState xsdValidationState2 = xsdValidationState.EvaluateStartElement(name, ns);
				if (xsdValidationState2 == XsdValidationState.Invalid)
				{
					if (!xsdValidationState.EvaluateIsEmptiable())
					{
						emptiable = false;
						return XsdValidationState.Invalid;
					}
					num++;
					if (num > current && flag && current >= 0)
					{
						return XsdValidationState.Invalid;
					}
					if (seq.CompiledItems.Count > num)
					{
						xsdValidationState = base.Manager.Create(seq.CompiledItems[num] as XmlSchemaParticle);
						continue;
					}
					if (current < 0)
					{
						break;
					}
					num = 0;
					xsdValidationState = null;
					continue;
				}
				current = num;
				currentAutomata = xsdValidationState2;
				if (flag)
				{
					base.OccuredInternal++;
					if ((decimal)base.Occured > seq.ValidatedMaxOccurs)
					{
						return XsdValidationState.Invalid;
					}
				}
				return this;
			}
			return XsdValidationState.Invalid;
		}

		public override bool EvaluateEndElement()
		{
			if (seq.ValidatedMinOccurs > (decimal)(base.Occured + 1))
			{
				return false;
			}
			if (seq.CompiledItems.Count == 0)
			{
				return true;
			}
			if (currentAutomata == null && seq.ValidatedMinOccurs <= (decimal)base.Occured)
			{
				return true;
			}
			int num = ((current >= 0) ? current : 0);
			XsdValidationState xsdValidationState = currentAutomata;
			if (xsdValidationState == null)
			{
				xsdValidationState = base.Manager.Create(seq.CompiledItems[num] as XmlSchemaParticle);
			}
			while (xsdValidationState != null)
			{
				if (!xsdValidationState.EvaluateEndElement() && !xsdValidationState.EvaluateIsEmptiable())
				{
					return false;
				}
				num++;
				xsdValidationState = ((seq.CompiledItems.Count <= num) ? null : base.Manager.Create(seq.CompiledItems[num] as XmlSchemaParticle));
			}
			if (current < 0)
			{
				base.OccuredInternal++;
			}
			return seq.ValidatedMinOccurs <= (decimal)base.Occured && seq.ValidatedMaxOccurs >= (decimal)base.Occured;
		}

		internal override bool EvaluateIsEmptiable()
		{
			if (seq.ValidatedMinOccurs > (decimal)(base.Occured + 1))
			{
				return false;
			}
			if (seq.ValidatedMinOccurs == 0m && currentAutomata == null)
			{
				return true;
			}
			if (emptiable)
			{
				return true;
			}
			if (seq.CompiledItems.Count == 0)
			{
				return true;
			}
			int num = ((current >= 0) ? current : 0);
			XsdValidationState xsdValidationState = currentAutomata;
			if (xsdValidationState == null)
			{
				xsdValidationState = base.Manager.Create(seq.CompiledItems[num] as XmlSchemaParticle);
			}
			while (xsdValidationState != null)
			{
				if (!xsdValidationState.EvaluateIsEmptiable())
				{
					return false;
				}
				num++;
				xsdValidationState = ((seq.CompiledItems.Count <= num) ? null : base.Manager.Create(seq.CompiledItems[num] as XmlSchemaParticle));
			}
			emptiable = true;
			return true;
		}
	}
}
