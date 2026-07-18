using System.Collections;
using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XsdAnyValidationState : XsdValidationState
	{
		private readonly XmlSchemaAny any;

		public XsdAnyValidationState(XmlSchemaAny any, XsdParticleStateManager manager)
			: base(manager)
		{
			this.any = any;
		}

		public override void GetExpectedParticles(ArrayList al)
		{
			al.Add(any);
		}

		public override XsdValidationState EvaluateStartElement(string name, string ns)
		{
			if (!MatchesNamespace(ns))
			{
				return XsdValidationState.Invalid;
			}
			base.OccuredInternal++;
			base.Manager.SetProcessContents(any.ResolvedProcessContents);
			if ((decimal)base.Occured > any.ValidatedMaxOccurs)
			{
				return XsdValidationState.Invalid;
			}
			if ((decimal)base.Occured == any.ValidatedMaxOccurs)
			{
				return base.Manager.Create(XmlSchemaParticle.Empty);
			}
			return this;
		}

		private bool MatchesNamespace(string ns)
		{
			if (any.HasValueAny)
			{
				return true;
			}
			if (any.HasValueLocal && ns == string.Empty)
			{
				return true;
			}
			if (any.HasValueOther && (any.TargetNamespace == string.Empty || any.TargetNamespace != ns))
			{
				return true;
			}
			if (any.HasValueTargetNamespace && any.TargetNamespace == ns)
			{
				return true;
			}
			for (int i = 0; i < any.ResolvedNamespaces.Count; i++)
			{
				if (any.ResolvedNamespaces[i] == ns)
				{
					return true;
				}
			}
			return false;
		}

		public override bool EvaluateEndElement()
		{
			return EvaluateIsEmptiable();
		}

		internal override bool EvaluateIsEmptiable()
		{
			return any.ValidatedMinOccurs <= (decimal)base.Occured && any.ValidatedMaxOccurs >= (decimal)base.Occured;
		}
	}
}
