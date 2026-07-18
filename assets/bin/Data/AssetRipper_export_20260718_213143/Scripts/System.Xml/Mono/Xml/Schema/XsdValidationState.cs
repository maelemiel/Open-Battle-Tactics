using System.Collections;

namespace Mono.Xml.Schema
{
	internal abstract class XsdValidationState
	{
		private static XsdInvalidValidationState invalid;

		private int occured;

		private readonly XsdParticleStateManager manager;

		public static XsdInvalidValidationState Invalid
		{
			get
			{
				return invalid;
			}
		}

		public XsdParticleStateManager Manager
		{
			get
			{
				return manager;
			}
		}

		public int Occured
		{
			get
			{
				return occured;
			}
		}

		internal int OccuredInternal
		{
			get
			{
				return occured;
			}
			set
			{
				occured = value;
			}
		}

		public XsdValidationState(XsdParticleStateManager manager)
		{
			this.manager = manager;
		}

		static XsdValidationState()
		{
			invalid = new XsdInvalidValidationState(null);
		}

		public abstract XsdValidationState EvaluateStartElement(string localName, string ns);

		public abstract bool EvaluateEndElement();

		internal abstract bool EvaluateIsEmptiable();

		public abstract void GetExpectedParticles(ArrayList al);
	}
}
