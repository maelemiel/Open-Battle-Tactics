using System.Collections;

namespace Mono.Xml.Schema
{
	internal class XsdAppendedValidationState : XsdValidationState
	{
		private XsdValidationState head;

		private XsdValidationState rest;

		public XsdAppendedValidationState(XsdParticleStateManager manager, XsdValidationState head, XsdValidationState rest)
			: base(manager)
		{
			this.head = head;
			this.rest = rest;
		}

		public override void GetExpectedParticles(ArrayList al)
		{
			head.GetExpectedParticles(al);
			rest.GetExpectedParticles(al);
		}

		public override XsdValidationState EvaluateStartElement(string name, string ns)
		{
			XsdValidationState xsdValidationState = head.EvaluateStartElement(name, ns);
			if (xsdValidationState != XsdValidationState.Invalid)
			{
				head = xsdValidationState;
				return (!(xsdValidationState is XsdEmptyValidationState)) ? this : rest;
			}
			if (!head.EvaluateIsEmptiable())
			{
				return XsdValidationState.Invalid;
			}
			return rest.EvaluateStartElement(name, ns);
		}

		public override bool EvaluateEndElement()
		{
			if (head.EvaluateEndElement())
			{
				return rest.EvaluateIsEmptiable();
			}
			if (!head.EvaluateIsEmptiable())
			{
				return false;
			}
			return rest.EvaluateEndElement();
		}

		internal override bool EvaluateIsEmptiable()
		{
			if (head.EvaluateIsEmptiable())
			{
				return rest.EvaluateIsEmptiable();
			}
			return false;
		}
	}
}
